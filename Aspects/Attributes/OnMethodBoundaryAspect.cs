using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッド境界アスペクト。
    /// </summary>
    [Serializable]
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

        #region アスペクトの注入

        /// <summary>
        /// アスペクトをメソッドに注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        protected override void OnInject(MethodDefinition method, CustomAttribute aspect)
        {
            /// 書き換え前の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();

            var iteratorStateMachineAttribute = method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.IteratorStateMachineAttribute");
            var asyncStateMachineAttribute    = method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");

            if (iteratorStateMachineAttribute != null)
            {
                var enumeratorType = (TypeDefinition)iteratorStateMachineAttribute.ConstructorArguments[0].Value;
                var module = enumeratorType.Module;

                /// フィールドを追加します。
                var aspectField      = new FieldDefinition("*aspect*",      Mono.Cecil.FieldAttributes.Private, module.ImportReference(GetType()));
                var aspectArgsField  = new FieldDefinition("*aspectArgs*",  Mono.Cecil.FieldAttributes.Private, module.ImportReference(typeof(MethodExecutionArgs)));
                var argsField        = new FieldDefinition("*args*",        Mono.Cecil.FieldAttributes.Private, module.ImportReference(typeof(Arguments)));
                var resumeFlagField  = new FieldDefinition("*resumeFlag*",  Mono.Cecil.FieldAttributes.Private, module.TypeSystem.Boolean);
                var exitFlagField    = new FieldDefinition("*exitFlag*",    Mono.Cecil.FieldAttributes.Private, module.TypeSystem.Boolean);
                var isDisposingField = new FieldDefinition("*isDisposing*", Mono.Cecil.FieldAttributes.Private, module.TypeSystem.Int32);

                enumeratorType.Fields.Add(aspectField);
                enumeratorType.Fields.Add(aspectArgsField);
                enumeratorType.Fields.Add(argsField);
                enumeratorType.Fields.Add(resumeFlagField);
                enumeratorType.Fields.Add(exitFlagField);
                enumeratorType.Fields.Add(isDisposingField);

                /// 各メソッドを書き換えます。
                CreateAspectField(enumeratorType, aspect, aspectField);
                ReplaceMoveNextMethod(method, enumeratorType, aspectField, aspectArgsField, argsField, resumeFlagField, exitFlagField, isDisposingField);
                ReplaceDisposeMethod(enumeratorType, isDisposingField);
            }
            else if (asyncStateMachineAttribute != null)
            {
                var injector = new AsyncStateMachine(aspect, method);

                injector.CreateAspectInstance();
                ReplaceMoveNextMethod(injector);
            }
            else
            {
                ReplaceMethod(method, aspect);
            }

            /// 書き換え後の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();
        }

        #region 通常のメソッド

        /// <summary>
        /// メソッドを書き換えます。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        private void ReplaceMethod(MethodDefinition method, CustomAttribute aspect)
        {
            var module        = method.Module;
            var attributes    = method.Attributes;
            var declaringType = method.DeclaringType;
            var returnType    = method.ReturnType;

            /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
            var originalMethod = new MethodDefinition(method.Name + "<Original>", attributes, returnType);
            foreach (var parameter in method.Parameters)
            {
                originalMethod.Parameters.Add(parameter);
            }

            originalMethod.Body = method.Body;

            foreach (var sequencePoint in method.DebugInformation.SequencePoints)
            {
                originalMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            declaringType.Methods.Add(originalMethod);

            /// 元々のメソッドを書き換えます。
            method.Body = new Mono.Cecil.Cil.MethodBody(method);

            var processor = method.Body.GetILProcessor();
            var variables = method.Body.Variables;

            var aspectArgsVariable = variables.Count();
            variables.Add(new VariableDefinition(module.ImportReference(typeof(MethodExecutionArgs))));

            int resultVariable = -1;
            if (method.HasReturnValue())
            {
                resultVariable = variables.Count();
                variables.Add(new VariableDefinition(method.ReturnType));
            }

            int exceptionVariable = variables.Count;
            variables.Add(new VariableDefinition(module.ImportReference(typeof(Exception))));

            var aspectVariable  = processor.Emit(aspect);
            {
                processor.Emit(OpCodes.Ldarg_0);
                {
                    var parameters     = method.Parameters;
                    var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType()).ToArray();

                    var argumentsType = parameters.Count switch
                    {
                        0 => typeof(Arguments),
                        1 => typeof(Arguments<>).MakeGenericType(parameterTypes),
                        2 => typeof(Arguments<,>).MakeGenericType(parameterTypes),
                        3 => typeof(Arguments<,,>).MakeGenericType(parameterTypes),
                        4 => typeof(Arguments<,,,>).MakeGenericType(parameterTypes),
                        5 => typeof(Arguments<,,,,>).MakeGenericType(parameterTypes),
                        6 => typeof(Arguments<,,,,,>).MakeGenericType(parameterTypes),
                        7 => typeof(Arguments<,,,,,,>).MakeGenericType(parameterTypes),
                        8 => typeof(Arguments<,,,,,,,>).MakeGenericType(parameterTypes),
                        _ => typeof(ArgumentsArray)
                    };

                    if (parameters.Count <= 8)
                    {
                        for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                        {
                            var parameter = parameters[parameterIndex];
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        processor.Emit(OpCodes.Newobj, module.ImportReference(argumentsType.GetConstructor(parameterTypes)));
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldc_I4, parameters.Count);
                        processor.Emit(OpCodes.Newarr, module.ImportReference(typeof(object)));
                        for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                        {
                            var parameter = parameters[parameterIndex];
                            processor.Emit(OpCodes.Dup);
                            processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                            if (parameter.ParameterType.IsValueType)
                            {
                                processor.Emit(OpCodes.Box, parameter.ParameterType);
                            }
                            processor.Emit(OpCodes.Stelem_Ref);
                        }
                        processor.Emit(OpCodes.Newobj, module.ImportReference(argumentsType.GetConstructor(new Type[] { typeof(object[]) })));
                    }
                }
                processor.Emit(OpCodes.Newobj, module.ImportReference(typeof(MethodExecutionArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) })));
                processor.Emit(OpCodes.Stloc, aspectArgsVariable);

                /// メソッド情報をイベントデータに設定します。
                processor.Emit(OpCodes.Ldloc, aspectArgsVariable);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { })));
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Method)).GetSetMethod()));

                /// OnEntry を呼び出します。
                processor.Emit(OpCodes.Ldloc, aspectVariable);
                processor.Emit(OpCodes.Ldloc, aspectArgsVariable);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnEntry))));
            }

            /// try {
            Instruction tryStart;
            Instruction leave;
            {
                tryStart = processor.EmitAndReturn(OpCodes.Ldarg_0);

                for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
                {
                    processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                }

                /// 元々のメソッドを呼び出します。
                processor.Emit(OpCodes.Callvirt, originalMethod);
                if (method.HasReturnValue())
                {
                    processor.Emit(OpCodes.Stloc, resultVariable);

                    processor.Emit(OpCodes.Ldloc, aspectArgsVariable);
                    processor.Emit(OpCodes.Ldloc, resultVariable);
                    if (method.ReturnType.IsValueType)
                    {
                        processor.Emit(OpCodes.Box, method.ReturnType);
                    }
                    processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetSetMethod()));
                }

                /// OnSuccess を呼び出します。
                processor.Emit(OpCodes.Ldloc, aspectVariable);
                processor.Emit(OpCodes.Ldloc, aspectArgsVariable);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess))));

                leave = processor.EmitAndReturn(OpCodes.Leave);
            }

            /// } catch (Exception exception) {
            Instruction catchStart;
            {
                catchStart = processor.EmitAndReturn(OpCodes.Stloc, exceptionVariable);

                processor.Emit(OpCodes.Ldloc, aspectArgsVariable);
                processor.Emit(OpCodes.Ldloc, exceptionVariable);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Exception)).GetSetMethod()));

                processor.Emit(OpCodes.Ldloc, aspectVariable);
                processor.Emit(OpCodes.Ldloc, aspectArgsVariable);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnException))));
                processor.Emit(OpCodes.Rethrow);
            }

            /// } finally {
            Instruction catchEnd;
            Instruction finallyStart;
            {
                catchEnd = finallyStart = processor.EmitAndReturn(OpCodes.Ldloc, aspectVariable);
                processor.Emit(OpCodes.Ldloc, aspectArgsVariable);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit))));

                processor.Emit(OpCodes.Endfinally);
            }

            ///
            Instruction finallyEnd;
            {
                if (method.HasReturnValue())
                {
                    leave.Operand = finallyEnd = processor.EmitAndReturn(OpCodes.Ldloc, resultVariable);
                    processor.Emit(OpCodes.Ret);
                }
                else
                {
                    leave.Operand = finallyEnd = processor.EmitAndReturn(OpCodes.Ret);
                }
            }

            /// Catch ハンドラーを追加します。
            var exceptionHandlers = method.Body.ExceptionHandlers;
            {
                var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    CatchType    = module.ImportReference(typeof(Exception)),
                    TryStart     = tryStart,
                    TryEnd       = catchStart,
                    HandlerStart = catchStart,
                    HandlerEnd   = catchEnd,
                };
                exceptionHandlers.Add(handler);
            }

            /// Finally ハンドラーを追加します。
            {
                var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
                {
                    TryStart     = tryStart,
                    TryEnd       = finallyStart,
                    HandlerStart = finallyStart,
                    HandlerEnd   = finallyEnd,
                };
                exceptionHandlers.Add(handler);
            }

            method.OptimizeIL();
        }

        #endregion

        #region 特殊なメソッド

        /// <summary>
        /// アスペクトフィールドのインスタンスを生成します。
        /// </summary>
        /// <param name="stateMachineType">ステートマシンの型。</param>
        /// <param name="aspect">アスペクト。</param>
        /// <param name="aspectField">アスペクトのフィールド。</param>
        private void CreateAspectField(TypeDefinition stateMachineType, CustomAttribute aspect, FieldDefinition aspectField)
        {
            var constructor  = stateMachineType.Methods.Single(m => m.Name == ".ctor");
            var processor    = constructor.Body.GetILProcessor();
            var instructions = constructor.Body.Instructions;
            var first        = instructions.First();

            /// アスペクトのインスタンスを生成し、ローカル変数にストアします。
            var aspectVariable = processor.InsertBefore(first, aspect);

            /// アスペクトのインスタンスをフィールドにストアします。
            processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, aspectVariable));
            processor.InsertBefore(first, processor.Create(OpCodes.Stfld, aspectField));
        }

        #region イテレーターメソッド

        /// <summary>
        /// <see cref="IEnumerator.MoveNext"/> を書き換えます。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="enumeratorType">反復子の型。</param>
        /// <param name="aspectField"><c>aspect</c > のフィールド。</param>
        /// <param name="aspectArgsField"><c>aspectArgs</c> のフィールド。</param>
        /// <param name="argsField"><c>args</c> のフィールド。</param>
        /// <param name="resumeFlagField"><c> resumeFlag</c> のフィールド。</param>
        /// <param name="exitFlagField"><c> exitFlag</c> のフィールド。</param>
        /// <param name="isDisposingField"><c> isDisposing</c> のフィールド。</param>
        private void ReplaceMoveNextMethod(MethodDefinition method, TypeDefinition enumeratorType, FieldDefinition aspectField, FieldDefinition aspectArgsField, FieldDefinition argsField, FieldDefinition resumeFlagField, FieldDefinition exitFlagField, FieldDefinition isDisposingField)
        {
            var module         = enumeratorType.Module;
            var moveNextMethod = enumeratorType.Methods.Single(m => m.Name == "MoveNext");
            var attributes     = moveNextMethod.Attributes;
            var declaringType  = moveNextMethod.DeclaringType;
            var returnType     = moveNextMethod.ReturnType;

            /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
            var originalMoveNextMethod = new MethodDefinition(moveNextMethod.Name + "<Original>", attributes, returnType);
            foreach (var parameter in moveNextMethod.Parameters)
            {
                originalMoveNextMethod.Parameters.Add(parameter);
            }

            originalMoveNextMethod.Body = moveNextMethod.Body;

            foreach (var sequencePoint in moveNextMethod.DebugInformation.SequencePoints)
            {
                originalMoveNextMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            declaringType.Methods.Add(originalMoveNextMethod);

            /// 元々のメソッドを書き換えます。
            moveNextMethod.Body = new Mono.Cecil.Cil.MethodBody(moveNextMethod);

            /// ローカル変数を追加します。
            var variables = moveNextMethod.Body.Variables;

            int resultVariable = variables.Count;
            variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

            int exceptionVariable = variables.Count;
            variables.Add(new VariableDefinition(module.ImportReference(typeof(Exception))));

            int exitFlagVariable = variables.Count;
            variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

            ///
            var processor = moveNextMethod.Body.GetILProcessor();
            {
                var branch = new Instruction[5];

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, exitFlagField);
                branch[0] = processor.EmitAndReturn(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, isDisposingField);
                branch[1] = processor.EmitAndReturn(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, resumeFlagField);
                branch[2] = processor.EmitAndReturn(OpCodes.Brtrue_S);

                // 
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, enumeratorType.Fields.Single(f => f.Name == "<>4__this"));
                if (method.DeclaringType.IsValueType)
                {
                    processor.Emit(OpCodes.Box, method.DeclaringType);
                }
                processor.Emit(OpCodes.Ldarg_0);
                {
                    var parameters     = method.Parameters;
                    var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType()).ToArray();

                    var argumentsType = parameters.Count switch
                    {
                        0 => typeof(Arguments),
                        1 => typeof(Arguments<>).MakeGenericType(parameterTypes),
                        2 => typeof(Arguments<,>).MakeGenericType(parameterTypes),
                        3 => typeof(Arguments<,,>).MakeGenericType(parameterTypes),
                        4 => typeof(Arguments<,,,>).MakeGenericType(parameterTypes),
                        5 => typeof(Arguments<,,,,>).MakeGenericType(parameterTypes),
                        6 => typeof(Arguments<,,,,,>).MakeGenericType(parameterTypes),
                        7 => typeof(Arguments<,,,,,,>).MakeGenericType(parameterTypes),
                        8 => typeof(Arguments<,,,,,,,>).MakeGenericType(parameterTypes),
                        _ => typeof(ArgumentsArray)
                    };

                    if (parameters.Count <= 8)
                    {
                        for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                        {
                            var parameter = parameters[parameterIndex];
                            processor.Emit(OpCodes.Ldarg_0);
                            processor.Emit(OpCodes.Ldfld, enumeratorType.Fields.Single(f => f.Name == parameter.Name));
                        }
                        processor.Emit(OpCodes.Newobj, module.ImportReference(argumentsType.GetConstructor(parameterTypes)));
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldc_I4, parameters.Count);
                        processor.Emit(OpCodes.Newarr, module.ImportReference(typeof(object)));
                        for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                        {
                            var parameter = parameters[parameterIndex];
                            processor.Emit(OpCodes.Dup);
                            processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                            processor.Emit(OpCodes.Ldarg_0);
                            processor.Emit(OpCodes.Ldfld, enumeratorType.Fields.Single(f => f.Name == parameter.Name));
                            if (parameter.ParameterType.IsValueType)
                            {
                                processor.Emit(OpCodes.Box, parameter.ParameterType);
                            }
                            processor.Emit(OpCodes.Stelem_Ref);
                        }
                        processor.Emit(OpCodes.Newobj, module.ImportReference(argumentsType.GetConstructor(new Type[] { typeof(object[]) })));
                    }
                }
                processor.Emit(OpCodes.Stfld, argsField);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, argsField);
                processor.Emit(OpCodes.Newobj, module.ImportReference(typeof(MethodExecutionArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) })));
                processor.Emit(OpCodes.Stfld, aspectArgsField);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnEntry))));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, resumeFlagField);
                branch[3] = processor.EmitAndReturn(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stfld, resumeFlagField);
                branch[4] = processor.EmitAndReturn(OpCodes.Br_S);

                branch[2].Operand = branch[3].Operand = processor.EmitAndReturn(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnResume))));

                branch[0].Operand = branch[1].Operand = branch[4].Operand = processor.EmitAndReturn(OpCodes.Nop);
            }

            /// try {
            Instruction tryStart;
            Instruction leave;
            {
                var branch = new Instruction[8];

                //
                tryStart = processor.EmitAndReturn(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stloc, exitFlagVariable);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, isDisposingField);
                processor.Emit(OpCodes.Ldc_I4_2);
                branch[0] = processor.EmitAndReturn(OpCodes.Beq_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, originalMoveNextMethod);
                processor.Emit(OpCodes.Stloc, resultVariable);

                branch[0].Operand = processor.EmitAndReturn(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, isDisposingField);

                branch[1] = processor.EmitAndReturn(OpCodes.Brtrue_S);
                processor.Emit(OpCodes.Ldloc, resultVariable);
                branch[2] = processor.EmitAndReturn(OpCodes.Brfalse_S);

                processor.Emit(OpCodes.Ldc_I4_0);
                processor.Emit(OpCodes.Stloc, exitFlagVariable);
                branch[3] = processor.EmitAndReturn(OpCodes.Br_S);

                branch[1].Operand = branch[2].Operand = processor.EmitAndReturn(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stloc, exitFlagVariable);

                branch[3].Operand = processor.EmitAndReturn(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, exitFlagField);
                branch[4] = processor.EmitAndReturn(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, resumeFlagField);
                branch[5] = processor.EmitAndReturn(OpCodes.Brfalse_S);

                processor.Emit(OpCodes.Ldloc, exitFlagVariable);
                branch[6] = processor.EmitAndReturn(OpCodes.Brfalse_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess))));
                branch[7] = processor.EmitAndReturn(OpCodes.Br_S);

                branch[6].Operand = processor.EmitAndReturn(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Ldarg_0);
                var currentField = enumeratorType.Fields.Single(f => f.Name == "<>2__current");
                processor.Emit(OpCodes.Ldfld, currentField);
                if (currentField.FieldType.IsValueType)
                {
                    processor.Emit(OpCodes.Box, currentField.FieldType);
                }
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetSetMethod()));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnYield))));

                branch[4].Operand = branch[5].Operand = branch[7].Operand = leave = processor.EmitAndReturn(OpCodes.Leave);
            }

            /// } catch (Exception exception) {
            Instruction catchStart;
            {
                catchStart = processor.EmitAndReturn(OpCodes.Stloc, exceptionVariable);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Ldloc, exceptionVariable);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Exception)).GetSetMethod()));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnException))));
                processor.Emit(OpCodes.Rethrow);
            }

            /// } finally {
            Instruction catchEnd;
            Instruction finallyStart;
            {
                var branch = new Instruction[3];

                catchEnd = finallyStart = processor.EmitAndReturn(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, exitFlagField);
                branch[0] = processor.EmitAndReturn(OpCodes.Brtrue_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, resumeFlagField);
                branch[1] = processor.EmitAndReturn(OpCodes.Brfalse_S);

                processor.Emit(OpCodes.Ldloc, exitFlagVariable);
                branch[2] = processor.EmitAndReturn(OpCodes.Brfalse_S);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stfld, exitFlagField);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit))));

                branch[0].Operand = branch[1].Operand = branch[2].Operand = processor.EmitAndReturn(OpCodes.Endfinally);
            }

            ///
            Instruction finallyEnd;
            {
                int resultIndex = variables.Count;
                variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

                leave.Operand = finallyEnd = processor.EmitAndReturn(OpCodes.Ldloc, exitFlagVariable);
                processor.Emit(OpCodes.Ldc_I4_0);
                processor.Emit(OpCodes.Ceq);
                processor.Emit(OpCodes.Stloc, resultIndex);
                processor.Emit(OpCodes.Ldloc, resultIndex);
                processor.Emit(OpCodes.Ret);
            }

            /// Catch ハンドラーを追加します。
            var handlers = moveNextMethod.Body.ExceptionHandlers;
            {
                var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    CatchType    = module.ImportReference(typeof(Exception)),
                    TryStart     = tryStart,
                    TryEnd       = catchStart,
                    HandlerStart = catchStart,
                    HandlerEnd   = catchEnd,
                };
                handlers.Add(handler);
            }

            /// Finally ハンドラーを追加します。
            {
                var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
                {
                    TryStart     = tryStart,
                    TryEnd       = finallyStart,
                    HandlerStart = finallyStart,
                    HandlerEnd   = finallyEnd,
                };
                handlers.Add(handler);
            }

            /// IL コードを最適化します。
            moveNextMethod.OptimizeIL();
        }

        /// <summary>
        /// <see cref="IDisposable.Dispose"/> を書き換えます。
        /// </summary>
        /// <param name="enumeratorType">反復子の型。</param>
        /// <param name="isDisposingField"><c>isDisposing</c> のフィールド。</param>
        private void ReplaceDisposeMethod(TypeDefinition enumeratorType, FieldDefinition isDisposingField)
        {
            ///
            var module        = enumeratorType.Module;
            var disposeMethod = enumeratorType.Methods.Single(m => m.Name == "System.IDisposable.Dispose");
            var attributes    = disposeMethod.Attributes;
            var declaringType = disposeMethod.DeclaringType;
            var returnType    = disposeMethod.ReturnType;

            /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
            var originalDisposeMethod = new MethodDefinition(disposeMethod.Name + "<Original>", attributes, returnType);
            foreach (var parameter in disposeMethod.Parameters)
            {
                originalDisposeMethod.Parameters.Add(parameter);
            }

            originalDisposeMethod.Body = disposeMethod.Body;

            foreach (var sequencePoint in disposeMethod.DebugInformation.SequencePoints)
            {
                originalDisposeMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            declaringType.Methods.Add(originalDisposeMethod);

            /// 元々のメソッドを書き換えます。
            disposeMethod.Body = new Mono.Cecil.Cil.MethodBody(disposeMethod);

            var processor = disposeMethod.Body.GetILProcessor();

            {
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, isDisposingField);
                var branch = processor.EmitAndReturn(OpCodes.Brfalse_S);
                processor.Emit(OpCodes.Ret);

                branch.Operand = processor.EmitAndReturn(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stfld, isDisposingField);
            }

            /// try {
            Instruction tryStart;
            Instruction leave;
            {
                tryStart = processor.EmitAndReturn(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, originalDisposeMethod);
                leave = processor.EmitAndReturn(OpCodes.Leave_S);
            }

            /// } finally {
            Instruction finallyStart;
            {
                finallyStart = processor.EmitAndReturn(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldc_I4_2);
                processor.Emit(OpCodes.Stfld, isDisposingField);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Callvirt, enumeratorType.Methods.Single(m => m.Name == "MoveNext"));

                processor.Emit(OpCodes.Pop);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldc_I4_0);
                processor.Emit(OpCodes.Stfld, isDisposingField);
                processor.Emit(OpCodes.Endfinally);
            }

            Instruction finallyEnd;
            {
                leave.Operand = finallyEnd = processor.EmitAndReturn(OpCodes.Ret);
            }

            /// Finally ハンドラーを追加します。
            var exceptionHandlers = disposeMethod.Body.ExceptionHandlers;
            var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart     = tryStart,
                TryEnd       = finallyStart,
                HandlerStart = finallyStart,
                HandlerEnd   = finallyEnd,
            };
            exceptionHandlers.Add(handler);
        }

        #endregion

        #region 非同期メソッド

        /// <summary>
        /// <see cref="IAsyncStateMachine.MoveNext"> を書き換えます。
        /// </summary>
        /// <param name="stateMachine">非同期ステートマシン。</param>
        private void ReplaceMoveNextMethod(AsyncStateMachine stateMachine)
        {
            var module         = stateMachine.Module;
            var moveNextMethod = stateMachine.MoveNextMethod;

            /// ローカル変数を追加します。
            var variables = moveNextMethod.Body.Variables;
            int resultVariable    = 1;
            int exceptionVariable = variables.Count;
            variables.Add(new VariableDefinition(module.ImportReference(typeof(Exception))));

            /// IL を書き換えます。
            var processor    = moveNextMethod.Body.GetILProcessor();
            var instructions = moveNextMethod.Body.Instructions;
            var handlers     = moveNextMethod.Body.ExceptionHandlers;

            var outerCatchHandler = handlers[0];
            var outerCatchStart   = outerCatchHandler.HandlerStart;
            var outerCatchEnd     = outerCatchHandler.HandlerEnd;
            var innerTryStart     = outerCatchHandler.TryStart;
            {
                var branch = new Instruction[2];
                var insert = innerTryStart;

                outerCatchHandler.TryStart = processor.EmitBefore(insert, OpCodes.Ldarg_0);
                processor.EmitBefore(insert, OpCodes.Ldfld, stateMachine.ResumeFlagField);
                branch[0] = processor.EmitBranchBefore(insert, OpCodes.Brtrue_S);

                /// AspectArgs フィールドのインスタンスを生成します。
                stateMachine.CreateAspectArgsInstance(processor, insert);

                /// <see cref="OnEntry"/> を呼びだします。
                stateMachine.InvokeEventHandler(processor, insert, nameof(OnEntry));

                processor.EmitBefore(insert, OpCodes.Ldarg_0);
                processor.EmitBefore(insert, OpCodes.Ldc_I4_1);
                processor.EmitBefore(insert, OpCodes.Stfld, stateMachine.ResumeFlagField);
                branch[1] = processor.EmitBranchBefore(insert, OpCodes.Br_S);

                /// <see cref="OnResume"/> を呼びだします。
                branch[0].Operand = processor.EmitBefore(insert, OpCodes.Nop);
                stateMachine.InvokeEventHandler(processor, insert, nameof(OnResume));

                branch[1].Operand = processor.EmitBefore(insert, OpCodes.Nop);
            }

            ///
            var leave = handlers[0].HandlerStart.Previous;
            {
                var branch = new Instruction[2];
                var insert = leave;

                var leaveTarget = processor.EmitBefore(insert, OpCodes.Ldarg_0);                    // try 内の Leave 命令の転送先 (OnYield と OnSuccess の呼び出し処理に転送します)。
                processor.EmitBefore(insert, OpCodes.Ldfld, stateMachine.StateField);
                processor.EmitBefore(insert, OpCodes.Ldc_I4, -1);
                branch[0] = processor.EmitBranchBefore(insert, OpCodes.Beq);

                /// <see cref="OnYield"/> を呼びだします。
                stateMachine.InvokeEventHandler(processor, insert, nameof(OnYield));
                branch[1] = processor.EmitBefore(insert, OpCodes.Br_S, insert);

                /// <see cref="OnSuccess"/> を呼びだします。
                branch[0].Operand = processor.EmitBefore(insert, OpCodes.Nop);
                stateMachine.SetReturnValue(processor, insert, resultVariable);
                stateMachine.InvokeEventHandler(processor, insert, nameof(OnSuccess));

                /// try 内の Leave 命令の転送先を書き換えます。
                /// この書き換えにより OnYield と OnSuccess の呼び出し処理に転送します。
                for (var instruction = innerTryStart; instruction != leaveTarget; instruction = instruction.Next)
                {
                    if (instruction.OpCode == OpCodes.Leave || instruction.OpCode == OpCodes.Leave_S)
                    {
                        instruction.OpCode  = OpCodes.Br;
                        instruction.Operand = leaveTarget;
                    }
                }
            }

            /// } catch (Exception exception) {
            Instruction innerCatchStart;
            {
                var insert = outerCatchStart;

                /// <see cref="OnException"/> を呼び出します。
                innerCatchStart = processor.EmitBefore(insert, OpCodes.Stloc, exceptionVariable);
                stateMachine.SetException(processor, insert, exceptionVariable);
                stateMachine.InvokeEventHandler(processor, insert, nameof(OnException));
                processor.EmitBefore(insert, OpCodes.Rethrow);
            }

            /// } finally {
            Instruction innerCatchEnd;
            Instruction innerFinallyStart;
            Instruction innerFinallyEnd;
            {
                var branch = new Instruction[2];
                var insert = outerCatchStart;

                innerCatchEnd = innerFinallyStart = processor.EmitBefore(insert, OpCodes.Ldarg_0);
                processor.EmitBefore(insert, OpCodes.Ldfld, stateMachine.StateField);
                processor.EmitBefore(insert, OpCodes.Ldc_I4, -1);
                branch[0] = processor.EmitBranchBefore(insert, OpCodes.Beq);
                branch[1] = processor.EmitBranchBefore(insert, OpCodes.Br);

                /// <see cref="OnExit"/> を呼び出します。
                branch[0].Operand = processor.EmitBefore(insert, OpCodes.Nop);
                stateMachine.InvokeEventHandler(processor, insert, nameof(OnExit));

                branch[1].Operand = processor.EmitBefore(insert, OpCodes.Endfinally);
                innerFinallyEnd = insert;
            }

            {
                var insert = outerCatchEnd;

                leave.Operand = handlers[0].HandlerEnd = processor.EmitBefore(insert, OpCodes.Ldarg_0);
                processor.EmitBefore(insert, OpCodes.Ldfld, stateMachine.StateField);
                processor.EmitBefore(insert, OpCodes.Ldc_I4, -1);
                processor.EmitBefore(insert, OpCodes.Beq, insert);

                processor.EmitBefore(insert, OpCodes.Br, instructions.Last());
            }

            /// 例外ハンドラーを追加します。
            {
                /// Catch ハンドラーを追加します。
                var innerCatchHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    CatchType    = module.ImportReference(typeof(Exception)),
                    TryStart     = innerTryStart,
                    TryEnd       = innerCatchStart,
                    HandlerStart = innerCatchStart,
                    HandlerEnd   = innerCatchEnd,
                };

                /// Finally ハンドラーを追加します。
                var innerFinallryHandler = new ExceptionHandler(ExceptionHandlerType.Finally)
                {
                    TryStart     = innerTryStart,
                    TryEnd       = innerFinallyStart,
                    HandlerStart = innerFinallyStart,
                    HandlerEnd   = innerFinallyEnd,
                };

                handlers.Clear();
                handlers.Add(innerCatchHandler);
                handlers.Add(innerFinallryHandler);
                handlers.Add(outerCatchHandler);
            }

            /// IL を最適化します。
            moveNextMethod.OptimizeIL();
        }

        #endregion

        #endregion

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
        /// <c>yield return</c> または <c>await</c> ステートメントの結果として、ステートマシンが結果を出力するときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        /// <remarks>
        /// イテレーターメソッドでは、アドバイスは <c>yield return</c> ステートメントで呼びだされます。
        /// 非同期メソッドでは、<c>await</c> ステートメントの結果としてステートマシンが待機を開始した直後にアドバイスが呼びだされます。
        /// <c>await</c> ステートメントのオペランドが同期的に完了した操作である場合、ステートマシンは結果を出力せず、<see cref="OnYield(MethodExecutionArgs)"/> アドバイスは呼び出されません。
        /// </remarks>
        public virtual void OnYield(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// <c>yield return</c> または <c>await</c> ステートメントの後にステートマシンが実行を再開するときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        /// <remarks>
        /// イテレーターメソッドの場合、このアドバイスは MoveNext() メソッドの前に呼びだされます。
        /// ただし、MoveNext() の最初の呼び出しは <see cref="OnEntry(MethodExecutionArgs)"/> にマップされます。
        /// 非同期メソッドでは、<c>await</c> ステートメントの結果として待機した後、ステートマシンが実行を再開した直後にアドバイスが呼びだされます。
        /// </remarks>
        public virtual void OnResume(MethodExecutionArgs args)
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
