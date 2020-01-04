using Mono.Cecil;
using Mono.Cecil.Cil;
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
            method.Log();

            // 最後の命令が throw 命令の場合、return 命令を追加します。
            var processor = method.Body.GetILProcessor();
            if (processor.Body.Instructions.Last().OpCode == OpCodes.Throw)
            {
                processor.Append(processor.Create(OpCodes.Ret));
            }

            // エントリーハンドラーを注入します。
            var (aspectIndex, eventArgsIndex, tryStart) = InjectEntryHandler(method);

            // リターンハンドラーを注入します。
            InjectReturnHandler(method, aspectIndex, eventArgsIndex);

            // 例外ハンドラーを注入します。
            InjectExceptionHandler(method, aspectIndex, eventArgsIndex, tryStart);

            // IL コードを最適化します。
            method.OptimizeIL();

            method.Log();
        }

        /// <summary>
        /// エントリーハンドラーを注入します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <returns>アスペクト変数のインデックス, イベントデータ変数のインデックス, TryStart 命令。</returns>
        private (int AspectIndex, int EventArgsIndex, Instruction TryStart) InjectEntryHandler(MethodDefinition method)
        {
            var module       = method.DeclaringType.Module.Assembly.MainModule;                     // モジュール。
            var parameters   = method.Parameters;                                                   // パラメーターコレクション。
            var processor    = method.Body.GetILProcessor();                                        // IL プロセッサー。
            var instructions = method.Body.Instructions;                                            // 命令コレクション。
            var variables    = method.Body.Variables;                                               // ローカル変数コレクション。

            // ローカル変数にアスペクトとイベントデータを追加する。
            var aspectIndex = variables.Count();                                                    // アスペクト変数のインデックス。
            variables.Add(new VariableDefinition(module.ImportReference(GetType())));

            var eventArgsIndex = variables.Count();                                                 // イベントデータ変数のインデックス。
            variables.Add(new VariableDefinition(module.ImportReference(typeof(MethodExecutionArgs))));

            // アスペクト変数を生成します。
            var first = instructions.First();
            processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(GetType().GetConstructor(new Type[] { }))));
            processor.InsertBefore(first, processor.Create(OpCodes.Stloc, aspectIndex));

            // イベントデータをローカル変数にストアします。
            {
                // アスペクトのインスタンス (第 1 引数) をロードします。
                processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));

                // パラメーターコレクション (第 2 引数) を生成し、ロードします。
                processor.InsertBefore(first, processor.Create(OpCodes.Ldc_I4, parameters.Count));
                processor.InsertBefore(first, processor.Create(OpCodes.Newarr, module.ImportReference(typeof(object))));
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter = parameters[parameterIndex];
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

                // イベントデータを生成し、ローカル変数にストアします。
                processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(typeof(MethodExecutionArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) }))));
                processor.InsertBefore(first, processor.Create(OpCodes.Stloc, eventArgsIndex));
            }

            // メソッド情報をイベントデータに設定します。
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(first, processor.Create(OpCodes.Call, module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { }))));
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
        /// リターンハンドラーを注入します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <param name="aspectIndex">アスペクト変数のインデックス。</param>
        /// <param name="eventArgsIndex">イベントデータ変数のインデックス。</param>
        private void InjectReturnHandler(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var module    = method.DeclaringType.Module.Assembly.MainModule;
            var processor = method.Body.GetILProcessor();
            var returns   = processor.Body.Instructions.Where(i => i.OpCode == OpCodes.Ret).ToArray();

            // 新たなリターン命令を追加します。
            var newReturn = processor.Body.Instructions.Last();                                     // 新たなリターン命令。
            if (method.HasReturnValue())
            {
                var resultIndex = processor.Body.Variables.Count();
                processor.Body.Variables.Add(new VariableDefinition(method.ReturnType));
                processor.Append(newReturn = processor.Create(OpCodes.Ldloc, resultIndex));
                processor.Append(processor.Create(OpCodes.Ret));

                // 新たなリターン命令へのジャンプ命令を追加します。
                // ※以降はこのジャンプ命令の前にコードを追加します。
                Instruction leave;                                                                  // 新たなリターン命令へのジャンプ命令。
                processor.InsertBefore(newReturn, leave = processor.Create(OpCodes.Leave_S, newReturn));

                //
                if (processor.Body.HasExceptionHandlers)
                {
                    var handler = processor.Body.ExceptionHandlers.Last();
                    if (handler.HandlerEnd == null)
                    {
                        handler.HandlerEnd = leave;
                    }
                }

                foreach (var @return in returns)
                {
                    // リターン命令を書き換えて、戻り値をローカル変数にストアします。
                    @return.OpCode = OpCodes.Stloc;
                    @return.Operand = processor.Body.Variables[resultIndex];

                    // 新たなリターン命令へのジャンプ命令を追加します。
                    // ※以降はこのジャンプ命令の前にコードを追加します。
                    Instruction branch;
                    processor.InsertAfter(@return, branch = processor.Create(OpCodes.Br_S, leave));

                    // 戻り値をイベントデータに設定します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, resultIndex));
                    if (method.ReturnType.IsValueType)
                    {
                        processor.InsertBefore(branch, processor.Create(OpCodes.Box, method.ReturnType));
                    }
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetSetMethod())));

                    // OnSuccess メソッドを呼び出します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, aspectIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess)))));

                    // OnExit メソッドを呼び出します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, aspectIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));
                }
            }
            else
            {
                processor.Append(newReturn = processor.Create(OpCodes.Ret));

                // 新たなリターン命令へのジャンプ命令を追加します。
                // ※以降はこのジャンプ命令の前にコードを追加します。
                Instruction leave;                                                                  // 新たなリターン命令へのジャンプ命令。
                processor.InsertBefore(newReturn, leave = processor.Create(OpCodes.Leave_S, newReturn));

                //
                if (processor.Body.HasExceptionHandlers)
                {
                    var handler = processor.Body.ExceptionHandlers.Last();
                    if (handler.HandlerEnd == null)
                    {
                        handler.HandlerEnd = leave;
                    }
                }

                foreach (var @return in returns)
                {
                    // リターン命令を Nop に書き換えます。
                    @return.OpCode = OpCodes.Nop;
                    @return.Operand = null;

                    // 新たなリターン命令へのジャンプ命令を追加します。
                    // ※以降はこのジャンプ命令の前にコードを追加します。
                    Instruction branch;
                    processor.InsertAfter(@return, branch = processor.Create(OpCodes.Br_S, leave));

                    // OnSuccess メソッドを呼び出します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, aspectIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess)))));

                    // OnExit メソッドを呼び出します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, aspectIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));
                }
            }
        }

        /// <summary>
        /// 例外ハンドラーを注入します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <param name="aspectIndex">アスペクト変数のインデックス。</param>
        /// <param name="eventArgsIndex">イベントデータ変数のインデックス。</param>
        /// <param name="tryStart">TryStart 命令。</param>
        private void InjectExceptionHandler(MethodDefinition method, int aspectIndex, int eventArgsIndex, Instruction tryStart)
        {
            var module = method.DeclaringType.Module.Assembly.MainModule;
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
