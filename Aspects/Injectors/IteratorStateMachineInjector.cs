using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Asserts;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// イテレーターステートマシンへの注入。
    /// </summary>
    public class IteratorStateMachineInjector : StateMachineInjector
    {
        #region プロパティ

        /// <summary>
        /// ステートマシンの属性。
        /// </summary>
        public override CustomAttribute StateMachineAttribute => TargetMethod.CustomAttributes.Single(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.IteratorStateMachineAttribute");

        #region フィールド

        /// <summary>
        /// Current フィールド。
        /// </summary>
        public FieldDefinition CurrentField => StateMachineType.Fields.Single(f => f.Name == "<>2__current");

        /// <summary>
        /// ExitFlag フィールド。
        /// </summary>
        public FieldDefinition ExitFlagField { get; }

        /// <summary>
        /// IsDisposing フィールド。
        /// </summary>
        public FieldDefinition IsDisposingField { get; }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="IDisposable.Dispose"/> メソッド。
        /// </summary>
        public MethodDefinition DisposeMethod => StateMachineType.Methods.Single(m => m.Name == "System.IDisposable.Dispose");

        /// <summary>
        /// Dispose メソッドの内容を移動したメソッド。
        /// </summary>
        public MethodDefinition OriginalDisposeMethod { get; private set; }

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        public IteratorStateMachineInjector(MethodDefinition targetMethod, CustomAttribute aspectAttribute)
            : base(targetMethod, aspectAttribute)
        {
            ExitFlagField    = CreateField("*exitFlag*", FieldAttributes.Private, Module.TypeSystem.Boolean);
            IsDisposingField = CreateField("*isDisposing*", FieldAttributes.Private, Module.TypeSystem.Int32);

            ReplaceDisposeMethod();
        }

        #endregion

        #region メソッド

        public void ReplaceMoveNextMethod(Action<ILProcessor> onEntry, Action<ILProcessor> onResume, Action<ILProcessor> onYield, Action<ILProcessor> onSuccess, Action<ILProcessor> onException, Action<ILProcessor> onExit)
        {
            /// 新たなメソッドを生成し、MoveNext メソッドの内容を移動します。
            ReplaceMoveNextMethod();

            /// 元々の MoveNext メソッドを書き換えます。
            {
                var moveNextMethod = MoveNextMethod;
                moveNextMethod.Body = new MethodBody(moveNextMethod);

                /// ローカル変数を追加します。
                var variables = moveNextMethod.Body.Variables;

                int exitVariable = variables.Count;
                variables.Add(new VariableDefinition(Module.TypeSystem.Boolean));

                /// 例外ハンドラーを追加します。
                var handlers = moveNextMethod.Body.ExceptionHandlers;
                var @catch   = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = Module.ImportReference(typeof(Exception)) };
                var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
                handlers.Add(@catch);
                handlers.Add(@finally);

                var processor = moveNextMethod.Body.GetILProcessor();

                /// if (!_exitFlag && _isDisposing == 0)
                /// {
                ///     if (!_resumeFlag)
                ///     {
                ///         onEntry();
                ///         _resumeFlag = true;
                ///     }
                ///     else
                ///     {
                ///         onResume();
                ///     }
                /// }
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

                    ////////////////////////////////////////////////////////////////////////////////
                    onEntry(processor);
                    //CreateAspectArgsInstance(processor);
                    //InvokeEventHandler(processor, "OnEntry");
                    //SetArgumentFields(processor);
                    ////////////////////////////////////////////////////////////////////////////////

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stfld, ResumeFlagField);
                    branch[3] = processor.EmitBranch(OpCodes.Br_S);

                    branch[2].Operand = processor.EmitNop();
                    ////////////////////////////////////////////////////////////////////////////////
                    onResume(processor);
                    //InvokeEventHandler(processor, "OnResume");
                    //SetArgumentFields(processor);
                    ////////////////////////////////////////////////////////////////////////////////

                    branch[0].Operand = branch[1].Operand = branch[3].Operand = processor.EmitNop();
                }

                /// bool exit;
                /// try
                /// {
                ///     exit = true;
                ///     bool result;
                ///     if (_isDisposing != 2)
                ///     {
                ///         result = MoveNext<Original>();
                ///     }
                ///     exit = _isDisposing != 0 || !result;
                ///     if (!_exitFlag && _resumeFlag)
                ///     {
                ///         if (exit)
                ///         {
                ///             onSuccess();
                ///         }
                ///         else
                ///         {
                ///             onYield();
                ///         }
                ///     }
                Instruction leave;
                {
                    var branch = new Instruction[8];

                    @catch.TryStart = @finally.TryStart = processor.EmitNop();

                    processor.Emit(OpCodes.Ldc_I4_1);
                    processor.Emit(OpCodes.Stloc, exitVariable);

                    int resultVariable = variables.Count;
                    variables.Add(new VariableDefinition(Module.TypeSystem.Boolean));

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, IsDisposingField);
                    processor.Emit(OpCodes.Ldc_I4_2);
                    branch[0] = processor.EmitBranch(OpCodes.Beq_S);

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Call, OriginalMoveNextMethod);
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
                    ////////////////////////////////////////////////////////////////////////////////
                    onSuccess(processor);
                    //InvokeEventHandler(processor, "OnSuccess");
                    ////////////////////////////////////////////////////////////////////////////////
                    branch[7] = processor.EmitBranch(OpCodes.Br_S);

                    branch[6].Operand = processor.EmitNop();
                    ////////////////////////////////////////////////////////////////////////////////
                    onYield(processor);
                    //SetYieldValue(processor);
                    //InvokeEventHandler(processor, "OnYield");
                    //SetCurrentField(processor);
                    ////////////////////////////////////////////////////////////////////////////////

                    branch[4].Operand = branch[5].Operand = branch[7].Operand = leave = processor.EmitLeave(OpCodes.Leave);
                }

                /// }
                /// catch (Exception exception)
                /// {
                ///     onException();
                ///     throw;
                {
                    @catch.TryEnd = @catch.HandlerStart = processor.EmitNop();
                    ////////////////////////////////////////////////////////////////////////////////
                    onException(processor);
                    //SetException(processor);
                    //InvokeEventHandler(processor, "OnException");
                    ////////////////////////////////////////////////////////////////////////////////
                    processor.Emit(OpCodes.Rethrow);
                }

                /// }
                /// finally
                /// {
                ///     if (!exitFlag && resumeFlag && exit)
                ///     {
                ///         exitFlag = true;
                ///         onExit();
                ///     }
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
                    ////////////////////////////////////////////////////////////////////////////////
                    onExit(processor);
                    //InvokeEventHandler(processor, "OnExit");
                    ////////////////////////////////////////////////////////////////////////////////

                    branch[0].Operand = branch[1].Operand = branch[2].Operand = processor.EmitNop();
                    processor.Emit(OpCodes.Endfinally);
                }

                /// }
                /// return !exit;
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

                /// IL を最適化します。
                moveNextMethod.Optimize();
            }
        }

        /// <summary>
        /// <see cref="MethodExecutionArgs.YieldValue"/> に値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetYieldValue(ILProcessor processor)
        {
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, AspectArgsField);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, CurrentField);
            if (CurrentField.FieldType.IsValueType)
            {
                processor.Emit(OpCodes.Box, CurrentField.FieldType);
            }
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetSetMethod()));
        }

        /// <summary>
        /// Current フィールドに値を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetCurrentField(ILProcessor processor)
        {
            processor.Emit(OpCodes.Ldarg_0);

            processor.Emit(OpCodes.Dup);
            processor.Emit(OpCodes.Ldfld, AspectArgsField);
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetGetMethod()));
            if (CurrentField.FieldType.IsValueType)
            {
                processor.Emit(OpCodes.Unbox_Any, CurrentField.FieldType);
            }
            processor.Emit(OpCodes.Stfld, CurrentField);
        }

        /// <summary>
        /// <see cref="IDisposable.Dispose"/> を書き換えます。
        /// </summary>
        private void ReplaceDisposeMethod()
        {
            /// 新たな Dispose メソッドを生成し、Dispose メソッドのコードを移動します。
            Assert.NotNull(DisposeMethod);
            Assert.Null(OriginalDisposeMethod);

            OriginalDisposeMethod = new MethodDefinition(DisposeMethod.Name + "<Original>", DisposeMethod.Attributes, DisposeMethod.ReturnType);
            foreach (var parameter in DisposeMethod.Parameters)
            {
                OriginalDisposeMethod.Parameters.Add(parameter);
            }

            OriginalDisposeMethod.Body = DisposeMethod.Body;
            foreach (var sequencePoint in DisposeMethod.DebugInformation.SequencePoints)
            {
                OriginalDisposeMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            StateMachineType.Methods.Add(OriginalDisposeMethod);

            /// Dispose のメソッドのコードを書き換えます。
            {
                //var disposeMethod = injector.DisposeMethod;
                DisposeMethod.Body = new MethodBody(DisposeMethod);

                /// 例外ハンドラーを追加します。
                var handlers = DisposeMethod.Body.ExceptionHandlers;
                var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
                handlers.Add(@finally);

                var processor = DisposeMethod.Body.GetILProcessor();

                /// if (isDisposing == 0)
                /// {
                ///     isDisposing = 1;
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

                ///     try
                ///     {
                ///         System.IDisposable.Dispose<Original>();
                Instruction leave;
                {
                    @finally.TryStart = processor.EmitNop();
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Call, OriginalDisposeMethod);
                    leave = processor.EmitLeave(OpCodes.Leave_S);
                }

                ///     }
                ///     finally
                ///     {
                ///         isDisposing = 2;
                ///         MoveNext();
                ///         isDisposing = 0;
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

                ///     }
                /// }
                {
                    leave.Operand = @finally.HandlerEnd = processor.EmitNop();
                    processor.Emit(OpCodes.Ret);
                }
            }
        }

        #endregion
    }
}
