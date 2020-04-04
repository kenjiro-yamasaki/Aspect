using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Asserts;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// イテレーターステートマシンの書き換え。
    /// </summary>
    public class IteratorStateMachineRewriter : StateMachineRewriter
    {
        #region プロパティ

        /// <summary>
        /// ステートマシン属性。
        /// </summary>
        public override CustomAttribute StateMachineAttribute => TargetMethod.CustomAttributes.Single(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.IteratorStateMachineAttribute");

        #region フィールド

        /// <summary>
        /// Current フィールド。
        /// </summary>
        public FieldDefinition CurrentField => StateMachineType.Fields.SingleOrDefault(f => f.Name == "<>2__current");

        /// <summary>
        /// ExitFlag フィールド。
        /// </summary>
        private FieldDefinition ExitFlagField { get; }

        /// <summary>
        /// IsDisposing フィールド。
        /// </summary>
        private FieldDefinition IsDisposingField { get; }

        #endregion

        #region メソッド

        /// <summary>
        /// Dispose メソッド。
        /// </summary>
        public MethodDefinition DisposeMethod => StateMachineType.Methods.Single(m => m.Name == "System.IDisposable.Dispose");

        /// <summary>
        /// オリジナル Dispose メソッド。
        /// </summary>
        /// <remarks>
        /// Dispose メソッドの元々のコードをコピーしたメソッド。
        /// </remarks>
        public MethodDefinition OriginalDisposeMethod { get; set; }

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        /// <param name="aspectArgsType">アスペクト引数の型。</param>
        public IteratorStateMachineRewriter(MethodDefinition targetMethod, CustomAttribute aspectAttribute, Type aspectArgsType)
            : base(targetMethod, aspectAttribute, aspectArgsType)
        {
            ExitFlagField    = CreateField("*exitFlag", FieldAttributes.Private, Module.TypeSystem.Boolean);
            IsDisposingField = CreateField("*isDisposing", FieldAttributes.Private, Module.TypeSystem.Int32, reuseIfExist: true);

            RewriteTargetMethod();
            RewriteDisposeMethod();
        }

        #endregion

        #region メソッド

        /// <summary>
        /// MoveNext メソッドを書き換えます。
        /// </summary>
        /// <param name="onEntry">OnEntory アドバイスの注入処理。</param>
        /// <param name="onResume">OnResume アドバイスの注入処理。</param>
        /// <param name="onYield">OnYield アドバイスの注入処理。</param>
        /// <param name="onSuccess">OnSuccess アドバイスの注入処理。</param>
        /// <param name="onException">OnException アドバイスの注入処理。</param>
        /// <param name="onExit">OnExit アドバイスの注入処理。</param>
        public void RewriteMoveNextMethod(Action<ILProcessor> onEntry, Action<ILProcessor> onResume, Action<ILProcessor> onYield, Action<ILProcessor> onSuccess, Action<ILProcessor> onException, Action<ILProcessor> onExit)
        {
            // MoveNext メソッドを複製します。
            var clonedMoveNextMethod = MoveNextMethod.Clone();

            // 元々の MoveNext メソッドを書き換えます。
            {
                var moveNextMethod = MoveNextMethod;
                moveNextMethod.Body = new MethodBody(moveNextMethod);

                // ローカル変数を追加します。
                var variables = moveNextMethod.Body.Variables;

                int exitVariable = variables.Count;
                variables.Add(new VariableDefinition(Module.TypeSystem.Boolean));

                // 例外ハンドラーを追加します。
                var handlers = moveNextMethod.Body.ExceptionHandlers;
                var @catch   = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = Module.ImportReference(typeof(Exception)) };
                var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
                handlers.Add(@catch);
                handlers.Add(@finally);

                var processor = moveNextMethod.Body.GetILProcessor();

                // if (!_exitFlag && _isDisposing == 0)
                // {
                //     if (!_resumeFlag)
                //     {
                //         onEntry();
                //         _resumeFlag = true;
                //     }
                //     else
                //     {
                //         onResume();
                //     }
                // }
                {
                    var branch = new Instruction[4];

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, ExitFlagField);
                    branch[0] = processor.EmitBranch(OpCodes.Brtrue_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, IsDisposingField);
                    branch[1] = processor.EmitBranch(OpCodes.Brtrue_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, ResumeFlagField);
                    branch[2] = processor.EmitBranch(OpCodes.Brtrue_S);
                    onEntry(processor);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stfld, ResumeFlagField);
                    branch[3] = processor.EmitBranch(OpCodes.Br_S);

                    branch[2].Operand = processor.EmitNop();
                    onResume(processor);

                    branch[0].Operand = branch[1].Operand = branch[3].Operand = processor.EmitNop();
                }

                // bool exit;
                // try
                // {
                //     exit = true;
                //     bool result;
                //     if (_isDisposing != 2)
                //     {
                //         result = OriginalMoveNext();
                //     }
                //     exit = _isDisposing != 0 || !result;
                //     if (!_exitFlag && _resumeFlag)
                //     {
                //         if (exit)
                //         {
                //             onSuccess();
                //         }
                //         else
                //         {
                //             onYield();
                //         }
                //     }
                Instruction leave;
                {
                    var branch = new Instruction[8];

                    @catch.TryStart = @finally.TryStart = processor.EmitNop();

                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stloc, exitVariable);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, IsDisposingField);
                    processor.Emit(OpCodes.Ldc_I4_2);
                    branch[0] = processor.EmitBranch(OpCodes.Beq);

                    int resultVariable = variables.Count;
                    variables.Add(new VariableDefinition(Module.TypeSystem.Boolean));

                    processor.Append(clonedMoveNextMethod);
                    processor.Emit(OpCodes.Stloc, resultVariable);

                    branch[0].Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, IsDisposingField);
                    branch[1] = processor.EmitBranch(OpCodes.Brtrue_S);

                    processor.Emit(OpCodes.Ldloc, resultVariable);
                    branch[2] = processor.EmitBranch(OpCodes.Brfalse_S);

                    processor.Emit(OpCodes.Ldc_I4_0);
                    processor.Emit(OpCodes.Stloc, exitVariable);
                    branch[3] = processor.EmitBranch(OpCodes.Br_S);

                    branch[1].Operand = branch[2].Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stloc, exitVariable);

                    branch[3].Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, ExitFlagField);
                    branch[4] = processor.EmitBranch(OpCodes.Brtrue_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, ResumeFlagField);
                    branch[5] = processor.EmitBranch(OpCodes.Brfalse_S);

                    processor.Emit(OpCodes.Ldloc, exitVariable);
                    branch[6] = processor.EmitBranch(OpCodes.Brfalse_S);
                    onSuccess(processor);
                    branch[7] = processor.EmitBranch(OpCodes.Br_S);

                    branch[6].Operand = processor.EmitNop();
                    onYield(processor);
                    branch[4].Operand = branch[5].Operand = branch[7].Operand = leave = processor.EmitLeave(OpCodes.Leave);
                }

                // }
                // catch (Exception exception)
                // {
                //     onException();
                //     throw;
                {
                    @catch.TryEnd = @catch.HandlerStart = processor.EmitNop();
                    onException(processor);
                    processor.Emit(OpCodes.Rethrow);
                }

                // }
                // finally
                // {
                //     if (!exitFlag && resumeFlag && exit)
                //     {
                //         exitFlag = true;
                //         onExit();
                //     }
                {
                    var branch = new Instruction[3];

                    @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, ExitFlagField);
                    branch[0] = processor.EmitBranch(OpCodes.Brtrue_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, ResumeFlagField);
                    branch[1] = processor.EmitBranch(OpCodes.Brfalse_S);

                    processor.Emit(OpCodes.Ldloc, exitVariable);
                    branch[2] = processor.EmitBranch(OpCodes.Brfalse_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stfld, ExitFlagField);
                    onExit(processor);

                    branch[0].Operand = branch[1].Operand = branch[2].Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Endfinally);
                }

                // }
                // return !exit;
                {
                    int resultVariable = variables.Count;
                    variables.Add(new VariableDefinition(Module.TypeSystem.Boolean));

                    leave.Operand = @finally.HandlerEnd = processor.EmitNop();
                    processor.Emit(OpCodes.Ldloc, exitVariable);
                    processor.Emit(OpCodes.Ldc_I4_0);
                    processor.Emit(OpCodes.Ceq);
                    processor.Emit(OpCodes.Stloc, resultVariable);
                    processor.Emit(OpCodes.Ldloc, resultVariable);
                    processor.Emit(OpCodes.Ret);
                }

                // IL コードを最適化します。
                moveNextMethod.Optimize();
            }
        }

        /// <summary>
        /// ターゲットメソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">イテレーターステートマシンの書き換え。</param>
        /// <remarks>
        /// ソリューション構成が Release の場合、ステートマシンの <>4__this フィールドが生成されません。
        /// この問題を解決するためにステートマシンに <>4__this フィールドを追加して、ターゲットメソッド内で値を設定するように書き換えます。
        /// </remarks>
        private void RewriteTargetMethod()
        {
            TargetMethod.Body = new MethodBody(TargetMethod);
            var processor = TargetMethod.Body.GetILProcessor();

            processor.Emit(OpCodes.Ldc_I4, -2);
            processor.New(StateMachineType);

            if (!TargetMethod.IsStatic)
            {
                processor.Emit(OpCodes.Dup);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Store(ThisField);
            }

            processor.Emit(OpCodes.Dup);
            processor.CallStatic(typeof(System.Reflection.MethodBase), nameof(System.Reflection.MethodBase.GetCurrentMethod));
            processor.Store(MethodField);

            var parameters = TargetMethod.Parameters;
            for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
            {
                var parameter = parameters[parameterIndex];

                processor.Emit(OpCodes.Dup);

                if (TargetMethod.IsStatic)
                {
                    processor.Emit(OpCodes.Ldarg, parameterIndex);
                }
                else
                {
                    processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                }
                processor.Store(GetField("<>3__" + parameter.Name));
            }

            processor.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Dispose メソッドを書き換えます。
        /// </summary>
        private void RewriteDisposeMethod()
        {
            var originalMethodName = "*" + DisposeMethod.Name;
            OriginalDisposeMethod = StateMachineType.Methods.FirstOrDefault(m => m.Name == originalMethodName);
            if (OriginalDisposeMethod != null)
            {
                return;
            }

            // オリジナル Dispose メソッド (Dispose メソッドの元々のコード) を生成します。
            Assert.NotNull(DisposeMethod);
            Assert.Null(OriginalDisposeMethod);
            OriginalDisposeMethod = new MethodDefinition(originalMethodName, DisposeMethod.Attributes, DisposeMethod.ReturnType);
            OriginalDisposeMethod.Body = DisposeMethod.Body;
            foreach (var parameter in DisposeMethod.Parameters)
            {
                OriginalDisposeMethod.Parameters.Add(parameter);
            }
            foreach (var sequencePoint in DisposeMethod.DebugInformation.SequencePoints)
            {
                OriginalDisposeMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }
            StateMachineType.Methods.Add(OriginalDisposeMethod);

            // Dispose のメソッドのコードを書き換えます。
            {
                DisposeMethod.Body = new MethodBody(DisposeMethod);

                // 例外ハンドラーを追加します。
                var handlers = DisposeMethod.Body.ExceptionHandlers;
                var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
                handlers.Add(@finally);

                var processor = DisposeMethod.Body.GetILProcessor();

                // if (isDisposing == 0)
                // {
                //     isDisposing = 1;
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, IsDisposingField);
                    var branch = processor.EmitBranch(OpCodes.Brfalse_S);
                    processor.Emit(OpCodes.Ret);

                    branch.Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stfld, IsDisposingField);
                }

                //     try
                //     {
                //         System.IDisposable.Dispose<Original>();
                Instruction leave;
                {
                    @finally.TryStart = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Call, OriginalDisposeMethod);
                    leave = processor.EmitLeave(OpCodes.Leave_S);
                }

                //     }
                //     finally
                //     {
                //         isDisposing = 2;
                //         MoveNext();
                //         isDisposing = 0;
                {
                    @finally.HandlerStart = @finally.TryEnd = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_2);
                    processor.Emit(OpCodes.Stfld, IsDisposingField);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Callvirt, MoveNextMethod);

                    processor.Emit(OpCodes.Pop);
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_0);
                    processor.Emit(OpCodes.Stfld, IsDisposingField);
                    processor.Emit(OpCodes.Endfinally);
                }

                //     }
                // }
                {
                    leave.Operand = @finally.HandlerEnd = processor.EmitNop();
                    processor.Emit(OpCodes.Ret);
                }
            }
        }

        #endregion
    }
}
