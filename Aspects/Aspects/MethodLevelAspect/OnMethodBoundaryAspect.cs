using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Log;
using System;
using System.Linq;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッド境界アスペクト。
    /// </summary>
    public abstract class OnMethodBoundaryAspect : MethodLevelAspect
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public OnMethodBoundaryAspect()
        {
        }

        #endregion

        #region メソッド

        #region アスペクト (カスタムコード) の注入

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        protected override void OnInject(MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            Logger.Trace($"{method.FullName}-------------------");
            foreach (var instruction in processor.Body.Instructions)
            {
                Logger.Trace($"{instruction}");
            }

            // 最後の命令が throw 命令の場合、return 命令を追加します。
            if (processor.Body.Instructions.Last().OpCode == OpCodes.Throw)
            {
                processor.Append(processor.Create(OpCodes.Ret));
            }

            // Entry コードを注入します。
            var (aspectIndex, eventArgsIndex, tryStart) = InjectEntryCode(method);

            // Success コードを注入します。
            InjectSuccessCode(method, aspectIndex, eventArgsIndex);

            // Exception コードを注入します。
            var (handlerStart, handlerEnd) = InjectExceptionCode(method, aspectIndex, eventArgsIndex);

            // 例外ハンドラーを追加します。
            AddExceptionHandler(method, tryStart, handlerStart, handlerStart, handlerEnd);

            Logger.Trace($"-------------------");
            foreach (var instruction in processor.Body.Instructions)
            {
                Logger.Trace($"{instruction}");
            }
            Logger.Trace($"-------------------");
        }

        /// <summary>
        /// Entry コードを注入します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <returns>アスペクト変数のインデックス, イベントデータ変数のインデックス, TryStart 命令。</returns>
        private (int AspectIndex, int EventArgsIndex, Instruction TryStart) InjectEntryCode(MethodDefinition method)
        {
            var module    = method.DeclaringType.Module.Assembly.MainModule;
            var processor = method.Body.GetILProcessor();
            var first     = processor.Body.Instructions.First();

            var aspectIndex = processor.Body.Variables.Count();
            processor.Body.Variables.Add(new VariableDefinition(module.ImportReference(GetType())));

            var eventArgsIndex = processor.Body.Variables.Count();
            processor.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(MethodExecutionArgs))));

            // OnMethodBoundaryAspect の派生クラスを生成します。
            processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(GetType().GetConstructor(new Type[] { }))));
            processor.InsertBefore(first, processor.Create(OpCodes.Stloc, aspectIndex));

            // MethodExecutionArgs を生成します。
            processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(first, processor.Create(OpCodes.Ldc_I4, method.Parameters.Count));
            processor.InsertBefore(first, processor.Create(OpCodes.Newarr, module.ImportReference(typeof(object))));
            for (int parameterIndex = 0; parameterIndex < method.Parameters.Count; parameterIndex++)
            {
                var parameter = method.Parameters[parameterIndex];
                processor.InsertBefore(first, processor.Create(OpCodes.Dup));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldc_I4, parameterIndex));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldarg, parameterIndex + 1));
                if (parameter.ParameterType.IsValueType)
                {
                    processor.InsertBefore(first, processor.Create(OpCodes.Box, parameter.ParameterType));
                }
                processor.InsertBefore(first, processor.Create(OpCodes.Stelem_Ref));
            }
            processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(typeof(Arguments).GetConstructor(new Type[] { typeof(object[]) }))));
            processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(typeof(MethodExecutionArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) }))));
            processor.InsertBefore(first, processor.Create(OpCodes.Stloc, eventArgsIndex));

            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(first, processor.Create(OpCodes.Call, module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[]{ }))));
            processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Method)).GetSetMethod())));

            // OnEntry メソッドを呼び出します。
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnEntry)))));

            // Try 節の開始命令を挿入します。
            Instruction tryStart;
            processor.InsertBefore(first, tryStart = processor.Create(OpCodes.Nop));
            return (aspectIndex, eventArgsIndex, tryStart);
        }

        /// <summary>
        /// Success コードを注入します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <param name="aspectIndex">アスペクト変数のインデックス。</param>
        /// <param name="eventArgsIndex">イベントデータ変数のインデックス。</param>
        private void InjectSuccessCode(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var module    = method.DeclaringType.Module.Assembly.MainModule;
            var processor = method.Body.GetILProcessor();
            var @return   = processor.Body.Instructions.Last();

            Instruction exit;
            if (method.HasReturnValue())
            {
                // 戻り値をローカル変数にストアします。
                int resultIndex = processor.Body.Variables.Count();
                processor.Body.Variables.Add(new VariableDefinition(method.ReturnType));

                @return.OpCode  = OpCodes.Stloc;
                @return.Operand = processor.Body.Variables[resultIndex];

                processor.Append(@return = processor.Create(OpCodes.Ldloc, resultIndex));
                processor.Append(processor.Create(OpCodes.Ret));

                processor.InsertBefore(@return, @exit = processor.Create(OpCodes.Leave_S, @return));

                //
                processor.InsertBefore(@exit, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                processor.InsertBefore(@exit, processor.Create(OpCodes.Ldloc, resultIndex));
                if (method.ReturnType.IsValueType)
                {
                    processor.InsertBefore(@exit, processor.Create(OpCodes.Box, method.ReturnType));
                }
                processor.InsertBefore(@exit, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetSetMethod())));
            }
            else
            {
                @return.OpCode  = OpCodes.Nop;
                @return.Operand = null;
                processor.Append(@return = processor.Create(OpCodes.Ret));

                processor.InsertBefore(@return, @exit = processor.Create(OpCodes.Leave_S, @return));
            }

            // OnSuccess メソッドを呼び出します。
            processor.InsertBefore(@exit, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(@exit, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@exit, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess)))));

            // OnExit メソッドを呼び出す。
            processor.InsertBefore(@exit, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(@exit, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@exit, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));
        }

        /// <summary>
        /// Exception コードを注入します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <param name="aspectIndex">アスペクト変数のインデックス。</param>
        /// <param name="eventArgsIndex">イベントデータ変数のインデックス。</param>
        /// <returns>HandlerStart 命令, HandlerEnd 命令。</returns>
        private (Instruction HandlerStart, Instruction HandlerEnd) InjectExceptionCode(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var module    = method.DeclaringType.Module.Assembly.MainModule;
            var processor = method.Body.GetILProcessor();
            Instruction @return;
            if (method.HasReturnValue())
            {
                @return = processor.Body.Instructions.Last().Previous;
            }
            else
            {
                @return = processor.Body.Instructions.Last();
            }

            // 例外オブジェクトをローカル変数にストアします。
            var exceptionIndex = processor.Body.Variables.Count();
            processor.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(Exception))));
            Instruction handlerStart;
            processor.InsertBefore(@return, handlerStart = processor.Create(OpCodes.Stloc, exceptionIndex));

            // 例外オブジェクトをイベントデータに設定します。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, exceptionIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Exception)).GetSetMethod())));

            // OnException メソッドを呼び出す。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnException)))));

            // OnExit メソッドを呼び出す(例外時)。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));

            //
            processor.InsertBefore(@return, processor.Create(OpCodes.Rethrow));

            return (handlerStart, @return);
        }

        /// <summary>
        /// 例外ハンドラーを追加します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <param name="tryStart">TryStart 命令。</param>
        /// <param name="tryEnd">TryEnd 命令。</param>
        /// <param name="handlerStart">HandlerStart 命令。</param>
        /// <param name="handlerEnd">HandlerEnd 命令。</param>
        private void AddExceptionHandler(MethodDefinition method, Instruction tryStart, Instruction tryEnd, Instruction handlerStart, Instruction handlerEnd)
        {
            var module    = method.DeclaringType.Module.Assembly.MainModule;
            var processor = method.Body.GetILProcessor();

            var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType    = module.ImportReference(typeof(Exception)),
                TryStart     = tryStart,
                TryEnd       = handlerStart,
                HandlerStart = handlerStart,
                HandlerEnd   = handlerEnd,
            };

            processor.Body.ExceptionHandlers.Add(handler);
        }

        #endregion

        #region イベントハンドラー

        /// <summary>
        /// メッソドの開始イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnEntry(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドの正常終了イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnSuccess(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドの例外終了イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnException(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドの終了イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnExit(MethodExecutionArgs args)
        {
        }

        #endregion

        #endregion
    }
}
