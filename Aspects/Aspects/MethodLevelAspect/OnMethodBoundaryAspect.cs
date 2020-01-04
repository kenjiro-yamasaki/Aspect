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
            // アスペクト注入前のメソッド定義の内部状態をログ出力します (デバッグ用、削除可)。
            method.Log();

            // 最後の命令が Throw 命令の場合、Return 命令を追加します。
            var processor = method.Body.GetILProcessor();
            if (processor.Body.Instructions.Last().OpCode == OpCodes.Throw)
            {
                processor.Append(processor.Create(OpCodes.Ret));
            }

            // ローカル変数にアスペクトとイベントデータを追加します。
            var module       = method.DeclaringType.Module.Assembly.MainModule;                     // モジュール。
            var variables    = method.Body.Variables;                                               // ローカル変数コレクション。

            var aspectIndex = variables.Count();                                                    // アスペクトの変数インデックス。
            variables.Add(new VariableDefinition(module.ImportReference(GetType())));

            var eventArgsIndex = variables.Count();                                                 // イベントデータの変数インデックス。
            variables.Add(new VariableDefinition(module.ImportReference(typeof(MethodExecutionArgs))));

            // エントリーハンドラーを注入します。
            InjectEntryHandler(method, aspectIndex, eventArgsIndex);

            // リターンハンドラーを注入します。
            InjectReturnHandler(method, aspectIndex, eventArgsIndex);

            // 例外ハンドラーを注入します。
            InjectExceptionHandler(method, aspectIndex, eventArgsIndex);

            // IL コードを最適化します。
            method.OptimizeIL();

            // アスペクト注入後のメソッド定義の内部状態をログ出力します (デバッグ用、削除可)。
            method.Log();
        }

        /// <summary>
        /// エントリーハンドラーを注入します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <param name="aspectIndex">アスペクトの変数インデックス。</param>
        /// <param name="eventArgsIndex">イベントデータの変数インデックス。</param>
        private void InjectEntryHandler(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var instructions = method.Body.Instructions;                                            // 命令コレクション。
            var module       = method.DeclaringType.Module.Assembly.MainModule;                     // モジュール。
            var parameters   = method.Parameters;                                                   // パラメーターコレクション。
            var processor    = method.Body.GetILProcessor();                                        // IL プロセッサー。
            var variables    = method.Body.Variables;                                               // ローカル変数コレクション。

            // アスペクトをローカル変数にストアします。
            var first = instructions.First();
            processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(GetType().GetConstructor(new Type[] { }))));
            processor.InsertBefore(first, processor.Create(OpCodes.Stloc, aspectIndex));

            // イベントデータを生成し、ローカル変数にストアします。
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

            // OnEntry を呼び出します。
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnEntry)))));
        }

        /// <summary>
        /// リターンハンドラーを注入します。
        /// </summary>
        /// <param name="method">メソッド定義。</param>
        /// <param name="aspectIndex">アスペクトの変数インデックス。</param>
        /// <param name="eventArgsIndex">イベントデータの変数インデックス。</param>
        private void InjectReturnHandler(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var exceptionHandlers = method.Body.ExceptionHandlers;                                  // 例外ハンドラーコレクション。
            var instructions      = method.Body.Instructions;                                       // 命令コレクション。
            var module            = method.DeclaringType.Module.Assembly.MainModule;                // モジュール。
            var processor         = method.Body.GetILProcessor();                                   // IL プロセッサー。
            var variables         = method.Body.Variables;                                          // ローカル変数コレクション。

            var returns = instructions.Where(i => i.OpCode == OpCodes.Ret).ToArray();               // Return 命令コレクション。

            if (method.HasReturnValue())
            {
                var resultIndex = variables.Count();                                                // 戻り値の変数インデックス。
                variables.Add(new VariableDefinition(method.ReturnType));

                // 新たな Return 命令を追加します。
                Instruction newReturn;                                                              // 新たな Return 命令。
                processor.Append(newReturn = processor.Create(OpCodes.Ldloc, resultIndex));
                processor.Append(processor.Create(OpCodes.Ret));

                // 新たなリターン命令へのジャンプ命令 (Leave 命令) を追加します。
                Instruction leave;                                                                  // Leave 命令。
                processor.InsertBefore(newReturn, leave = processor.Create(OpCodes.Leave_S, newReturn));

                // 元々の例外ハンドラーの終了位置が明示されていない場合、終了位置を Leave 命令 に変更します。
                foreach (var handler in exceptionHandlers.Where(eh => eh.HandlerEnd == null))
                {
                    handler.HandlerEnd = leave;
                }

                foreach (var @return in returns)
                {
                    // Return 命令を書き換えて、戻り値をローカル変数にストアします。
                    @return.OpCode = OpCodes.Stloc;
                    @return.Operand = processor.Body.Variables[resultIndex];

                    // Leave 命令へのジャンプ命令 (Branch 命令) を追加します。
                    // 以降は Branch 命令の前にコードを追加します。
                    Instruction branch;                                                             // Branch 命令。
                    processor.InsertAfter(@return, branch = processor.Create(OpCodes.Br_S, leave));

                    // 戻り値をイベントデータに設定します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, resultIndex));
                    if (method.ReturnType.IsValueType)
                    {
                        processor.InsertBefore(branch, processor.Create(OpCodes.Box, method.ReturnType));
                    }
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetSetMethod())));

                    // OnSuccess を呼び出します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, aspectIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess)))));

                    // OnExit を呼び出します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, aspectIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));
                }
            }
            else
            {
                // 新たな Return 命令を追加します。
                Instruction newReturn;                                                              // 新たな Return 命令。
                processor.Append(newReturn = processor.Create(OpCodes.Ret));

                // 新たなリターン命令へのジャンプ命令 (Leave 命令) を追加します。
                Instruction leave;                                                                  // Leave 命令。
                processor.InsertBefore(newReturn, leave = processor.Create(OpCodes.Leave_S, newReturn));

                // 元々の例外ハンドラーの終了位置が明示されていない場合、終了位置を Leave 命令 に変更します。
                foreach (var handler in exceptionHandlers.Where(eh => eh.HandlerEnd == null))
                {
                    handler.HandlerEnd = leave;
                }

                foreach (var @return in returns)
                {
                    // Return 命令を Nop に書き換えます。
                    @return.OpCode = OpCodes.Nop;
                    @return.Operand = null;

                    // Leave 命令へのジャンプ命令 (Branch 命令) を追加します。
                    // 以降は Branch 命令の前にコードを追加します。
                    Instruction branch;                                                             // Branch 命令。
                    processor.InsertAfter(@return, branch = processor.Create(OpCodes.Br_S, leave));

                    // OnSuccess を呼び出します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, aspectIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess)))));

                    // OnExit を呼び出します。
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
        /// <param name="aspectIndex">アスペクトの変数インデックス。</param>
        /// <param name="eventArgsIndex">イベントデータの変数インデックス。</param>
        private void InjectExceptionHandler(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var instructions = method.Body.Instructions;                                            // 命令コレクション。
            var module       = method.DeclaringType.Module.Assembly.MainModule;                     // モジュール。
            var parameters   = method.Parameters;                                                   // パラメーターコレクション。
            var processor    = method.Body.GetILProcessor();                                        // IL プロセッサー。
            var variables    = method.Body.Variables;                                               // ローカル変数コレクション。

            Instruction @return;
            if (method.HasReturnValue())
            {
                @return = instructions.Last().Previous;
            }
            else
            {
                @return = instructions.Last();
            }

            // 例外オブジェクトをローカル変数にストアします。
            var exceptionIndex = variables.Count();                                                 // 例外オブジェクトの変数インデックス。
            processor.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(Exception))));

            Instruction handlerStart;
            processor.InsertBefore(@return, handlerStart = processor.Create(OpCodes.Stloc, exceptionIndex));

            // 例外オブジェクトをイベントデータに設定します。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, exceptionIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Exception)).GetSetMethod())));

            // OnException を呼び出します。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnException)))));

            // OnExit を呼び出します (例外時)。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));

            // 例外を再スローします。
            processor.InsertBefore(@return, processor.Create(OpCodes.Rethrow));

            // 例外ハンドラーを追加します。
            var first = instructions.First();
            var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType    = module.ImportReference(typeof(Exception)),
                TryStart     = first,
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
