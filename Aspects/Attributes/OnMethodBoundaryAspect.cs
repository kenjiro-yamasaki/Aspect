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
        protected OnMethodBoundaryAspect()
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
            var module    = method.DeclaringType.Module.Assembly.MainModule;                        // モジュール。
            var variables = method.Body.Variables;                                                  // ローカル変数コレクション。

            var aspectIndex = variables.Count();                                                    // アスペクトの変数インデックス。
            variables.Add(new VariableDefinition(module.ImportReference(GetType())));

            var eventArgsIndex = variables.Count();                                                 // イベントデータの変数インデックス。
            variables.Add(new VariableDefinition(module.ImportReference(typeof(MethodExecutionArgs))));

            // Entry ハンドラーを注入します。
            var tryStart = InjectEntryHandler(method, aspectIndex, eventArgsIndex);

            // Return ハンドラーを注入します。
            var tryLast = InjectReturnHandler(method, aspectIndex, eventArgsIndex);

            // Excetpion ハンドラーを注入します。
            var catchLast = InjectExceptionHandler(method, aspectIndex, eventArgsIndex);

            // Exit ハンドラーを注入します。
            var endLast = InjectExitHandler(method, aspectIndex, eventArgsIndex);

            // 命令アドレスを修正します。
            var exceptionHandlers = method.Body.ExceptionHandlers;
            {
                // Try の最終命令のジャンプ先アドレスを修正します。
                tryLast.OpCode  = OpCodes.Leave_S;
                tryLast.Operand = catchLast;

                // 元々の例外ハンドラーの終了位置が明示されていない場合、終了位置を Leave 命令 に変更します。
                foreach (var handler in exceptionHandlers.Where(eh => eh.HandlerEnd == null))
                {
                    handler.HandlerEnd = tryLast;
                }
            }

            // Catch ハンドラーを追加します。
            {
                var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    CatchType    = module.ImportReference(typeof(Exception)),
                    TryStart     = tryStart,
                    TryEnd       = tryLast.Next,
                    HandlerStart = tryLast.Next,
                    HandlerEnd   = catchLast,
                };
                exceptionHandlers.Add(handler);
            }

            // Finally ハンドラーを追加します。
            {
                var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
                {
                    TryStart     = tryStart,
                    TryEnd       = catchLast.Next,
                    HandlerStart = catchLast.Next,
                    HandlerEnd   = endLast.Next,
                };
                exceptionHandlers.Add(handler);
            }

            // IL を最適化します。
            method.OptimizeIL();

            // アスペクト注入後のメソッド定義の内部状態をログ出力します (デバッグ用、削除可)。
            method.Log();
        }

        /// <summary>
        /// Entry ハンドラーを注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <param name="aspectIndex">アスペクトの変数インデックス。</param>
        /// <param name="eventArgsIndex">イベントデータの変数インデックス。</param>
        /// <returns>Try の先頭命令。</returns>
        private Instruction InjectEntryHandler(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var instructions = method.Body.Instructions;                                            // 命令コレクション。
            var module       = method.DeclaringType.Module.Assembly.MainModule;                     // モジュール。
            var processor    = method.Body.GetILProcessor();                                        // IL プロセッサー。

            // アスペクトをローカル変数にストアします。
            var first = instructions.First();                                                       // 最初の命令。
            processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(GetType().GetConstructor(new Type[] { }))));
            processor.InsertBefore(first, processor.Create(OpCodes.Stloc, aspectIndex));

            // イベントデータを生成し、ローカル変数にストアします。
            {
                // アスペクトのインスタンス (第 1 引数) をロードします。
                processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));

                // パラメーターコレクション (第 2 引数) を生成し、ロードします。
                var parameters = method.Parameters;                                                 // パラメーターコレクション。
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

            // Try の先頭命令を挿入します。
            Instruction tryStart;
            processor.InsertBefore(first, tryStart = processor.Create(OpCodes.Nop));
            return tryStart;
        }

        /// <summary>
        /// Return ハンドラーを注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <param name="aspectIndex">アスペクトの変数インデックス。</param>
        /// <param name="eventArgsIndex">イベントデータの変数インデックス。</param>
        /// <returns>
        /// Try の最終命令 (Catch の最終命令への Leave 命令)。
        /// ジャンプ先の命令が不明であるため、Nop 命令をプレースフォルダとして追加しています。
        /// 正しい Leave 命令に書き換える必要があります。
        /// </returns>
        private Instruction InjectReturnHandler(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var instructions = method.Body.Instructions;                                            // 命令コレクション。
            var module       = method.DeclaringType.Module.Assembly.MainModule;                     // モジュール。
            var processor    = method.Body.GetILProcessor();                                        // IL プロセッサー。

            var returns = instructions.Where(i => i.OpCode == OpCodes.Ret).ToArray();               // Return 命令コレクション。

            if (method.HasReturnValue())
            {
                var variables   = method.Body.Variables;                                            // ローカル変数コレクション。
                var resultIndex = variables.Count();                                                // 戻り値の変数インデックス。
                variables.Add(new VariableDefinition(method.ReturnType));

                // 新たな Return 命令を追加します。
                Instruction newReturn;                                                              // 新たな Return 命令。
                processor.Append(newReturn = processor.Create(OpCodes.Ldloc, resultIndex));
                processor.Append(processor.Create(OpCodes.Ret));

                // Catch の最終命令への転送命令 (Leave 命令) を挿入します。
                // 転送先の命令が不明であるため、Nop 命令をプレースフォルダとして挿入しています。
                // 後処理にて、正しい Leave 命令に書き換える必要があります。
                Instruction leave;                                                                  // Leave 命令。
                processor.InsertBefore(newReturn, leave = processor.Create(OpCodes.Nop));

                foreach (var @return in returns)
                {
                    // Return 命令を書き換えて、戻り値をローカル変数にストアします。
                    @return.OpCode = OpCodes.Stloc;
                    @return.Operand = processor.Body.Variables[resultIndex];

                    // Leave 命令への転送命令 (Branch 命令) を挿入します。
                    // 以降は Branch 命令の前にコードを挿入します。
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
                }

                return leave;
            }
            else
            {
                // 新たな Return 命令を追加します。
                Instruction newReturn;                                                              // 新たな Return 命令。
                processor.Append(newReturn = processor.Create(OpCodes.Ret));

                // Catch の最終命令への転送命令 (Leave 命令) を挿入します。
                // 転送先の命令が不明であるため、Nop 命令をプレースフォルダとして挿入しています。
                // 後処理にて、正しい Leave 命令に書き換える必要があります。
                Instruction leave;                                                                  // Leave 命令。
                processor.InsertBefore(newReturn, leave = processor.Create(OpCodes.Nop));

                foreach (var @return in returns)
                {
                    // Return 命令を Nop に書き換えます。
                    @return.OpCode = OpCodes.Nop;
                    @return.Operand = null;

                    // Leave 命令への転送命令 (Branch 命令) を挿入します。
                    // 以降は Branch 命令の前にコードを挿入します。
                    Instruction branch;                                                             // Branch 命令。
                    processor.InsertAfter(@return, branch = processor.Create(OpCodes.Br_S, leave));

                    // OnSuccess を呼び出します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, aspectIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess)))));
                }

                return leave;
            }
        }

        /// <summary>
        /// Exception ハンドラーを注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <param name="aspectIndex">アスペクトの変数インデックス。</param>
        /// <param name="eventArgsIndex">イベントデータの変数インデックス。</param>
        /// <returns>Catch の最終命令 (Return 命令への Leave 命令)。</returns>
        private Instruction InjectExceptionHandler(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var instructions = method.Body.Instructions;                                            // 命令コレクション。
            var module       = method.DeclaringType.Module.Assembly.MainModule;                     // モジュール。
            var processor    = method.Body.GetILProcessor();                                        // IL プロセッサー。

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
            var variables      = method.Body.Variables;                                             // ローカル変数コレクション。
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

            // 例外を再スローします。
            processor.InsertBefore(@return, processor.Create(OpCodes.Rethrow));

            // Return 命令への転送命令 (Leave 命令) を挿入します。
            Instruction leave;                                                                      // Leave 命令。
            processor.InsertBefore(@return, leave = processor.Create(OpCodes.Leave_S, @return));

            return leave;
        }

        /// <summary>
        /// Exit ハンドラーを注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <param name="aspectIndex">アスペクトの変数インデックス。</param>
        /// <param name="eventArgsIndex">イベントデータの変数インデックス。</param>
        /// <returns>Finally の最終命令。</returns>
        private Instruction InjectExitHandler(MethodDefinition method, int aspectIndex, int eventArgsIndex)
        {
            var instructions = method.Body.Instructions;                                            // 命令コレクション。
            var module       = method.DeclaringType.Module.Assembly.MainModule;                     // モジュール。
            var processor    = method.Body.GetILProcessor();                                        // IL プロセッサー。

            Instruction @return;
            if (method.HasReturnValue())
            {
                @return = instructions.Last().Previous;
            }
            else
            {
                @return = instructions.Last();
            }

            // Finally ハンドラーの先頭命令 (Nop) を挿入します。
            processor.InsertBefore(@return, processor.Create(OpCodes.Nop));

            // OnExit を呼び出します。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));

            // EndFinally 命令を挿入します。
            Instruction endFinally;
            processor.InsertBefore(@return, endFinally = processor.Create(OpCodes.Endfinally));

            return endFinally;
        }

        #endregion

        #region イベントハンドラー

        /// <summary>
        /// メッソドが開始されたときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnEntry(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドが正常終了したときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnSuccess(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドが例外終了したときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnException(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドが終了したときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnExit(MethodExecutionArgs args)
        {
        }

        #endregion

        #endregion
    }
}
