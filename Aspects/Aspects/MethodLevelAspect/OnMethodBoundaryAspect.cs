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

            // OnEntry コードを注入します。
            var (aspectIndex, eventArgsIndex, tryStart) = InjectEntryCode(method);

            // OnSuccess コードを注入します。
            InjectSuccessCode(method, aspectIndex, eventArgsIndex);

            // OnException コードを注入します。
            InjectExceptionHandlerCode(method, aspectIndex, eventArgsIndex, tryStart);

            Logger.Trace($"-------------------");
            foreach (var instruction in processor.Body.Instructions)
            {
                Logger.Trace($"{instruction}");
            }
            Logger.Trace($"-------------------");
        }

        /// <summary>
        /// OnEntry コードを注入します。
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
        /// OnSuccess コードを注入します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <param name="aspectIndex">アスペクト変数のインデックス。</param>
        /// <param name="eventArgsIndex">イベントデータ変数のインデックス。</param>
        private void InjectSuccessCode(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var module    = method.DeclaringType.Module.Assembly.MainModule;
            var processor = method.Body.GetILProcessor();
            var @return   = processor.Body.Instructions.Last();

            // 以下のコードは特に複雑なので、コードの考え方を説明します。
            // OnSuccess コードはリターン命令の後ろに注入する必要があります。
            // これは制御文のコードがリターン命令へのジャンプ命令を含む可能性があるためです。
            // ただし、リターン命令をそのままにしておくと、Success コード以降を実行せずにリターンしてしまいます。
            // そこで、リターン命令を Success コードの最初の命令に書き換えます。
            // さらに書き換えたリターン命令の代わりに新たなリターン命令を末尾に追加します。
            // OnSuccess コードと OnException コードは、元々のリターン命令と新たなリターン命令の間に注入します。
            Instruction jump;                                                                       // 新たなリターン命令へのジャンプ命令。
            if (method.HasReturnValue())
            {
                // リターン命令を書き換えて、戻り値をローカル変数にストアします。
                int resultIndex = processor.Body.Variables.Count();
                processor.Body.Variables.Add(new VariableDefinition(method.ReturnType));

                @return.OpCode  = OpCodes.Stloc;
                @return.Operand = processor.Body.Variables[resultIndex];

                // 新たなリターン命令を追加します。
                processor.Append(@return = processor.Create(OpCodes.Ldloc, resultIndex));
                processor.Append(processor.Create(OpCodes.Ret));

                // 新たなリターン命令へのジャンプ命令を追加します。
                // ※以降はこのジャンプ命令の前にコードを追加します。
                processor.InsertBefore(@return, jump = processor.Create(OpCodes.Leave_S, @return));

                // 戻り値をイベントデータに設定します。
                processor.InsertBefore(jump, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                processor.InsertBefore(jump, processor.Create(OpCodes.Ldloc, resultIndex));
                if (method.ReturnType.IsValueType)
                {
                    processor.InsertBefore(jump, processor.Create(OpCodes.Box, method.ReturnType));
                }
                processor.InsertBefore(jump, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetSetMethod())));
            }
            else
            {
                // リターン命令を Nop 書き換えます。
                @return.OpCode  = OpCodes.Nop;
                @return.Operand = null;

                // 新たなリターン命令を追加します。
                processor.Append(@return = processor.Create(OpCodes.Ret));

                // 新たなリターン命令へのジャンプ命令を追加します。
                // ※以降はこのジャンプ命令の前にコードを追加します。
                processor.InsertBefore(@return, jump = processor.Create(OpCodes.Leave_S, @return));
            }

            // OnSuccess メソッドを呼び出します。
            processor.InsertBefore(jump, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(jump, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(jump, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess)))));

            // OnExit メソッドを呼び出します。
            processor.InsertBefore(jump, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(jump, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(jump, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));
        }

        /// <summary>
        /// OnException コードを注入します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <param name="aspectIndex">アスペクト変数のインデックス。</param>
        /// <param name="eventArgsIndex">イベントデータ変数のインデックス。</param>
        /// <param name="tryStart">TryStart 命令。</param>
        private void InjectExceptionHandlerCode(MethodDefinition method, int aspectIndex, int eventArgsIndex, Instruction tryStart)
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

            // OnException メソッドを呼び出します。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnException)))));

            // OnExit メソッドを呼び出します (例外時)。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));

            // 例外を再スローします。
            processor.InsertBefore(@return, processor.Create(OpCodes.Rethrow));

            // 例外ハンドラーを追加します。
            var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType    = module.ImportReference(typeof(Exception)),
                TryStart     = tryStart,
                TryEnd       = handlerStart,
                HandlerStart = handlerStart,
                HandlerEnd   = @return,
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
