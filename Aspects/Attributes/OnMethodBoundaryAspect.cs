using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
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
        #region プロパティ

        private static int InstanceCount { get; set; }

        #endregion


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
        /// <param name="aspect">注入対象の属性。</param>
        protected override void OnInject(MethodDefinition method, CustomAttribute aspect)
        {
            /// 書き換え前の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();

            var iteratorStateMachineAttribute = method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.IteratorStateMachineAttribute");
            if (iteratorStateMachineAttribute != null)
            {
                var type = (TypeDefinition)iteratorStateMachineAttribute.ConstructorArguments[0].Value;
                InjectToIteratorMethod(type, aspect);
            }
            else
            {
                InjectToNormalMethod(method, aspect);
            }

            /// 書き換え後の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="aspect"></param>
        private void InjectToIteratorMethod(TypeDefinition type, CustomAttribute aspect)
        {
            //
            var module = type.Module;

            var isDisposingField = new FieldDefinition("isDisposing", Mono.Cecil.FieldAttributes.Private, module.TypeSystem.Int32);
            var resumeFlagField  = new FieldDefinition("resumeFlag",  Mono.Cecil.FieldAttributes.Private, module.TypeSystem.Boolean);
            var exitFlagField    = new FieldDefinition("exitFlag",    Mono.Cecil.FieldAttributes.Private, module.TypeSystem.Boolean);
            var aspectArgsField  = new FieldDefinition("aspectArgs",  Mono.Cecil.FieldAttributes.Private, module.ImportReference(typeof(MethodExecutionArgs)));
            var argsField        = new FieldDefinition("args",        Mono.Cecil.FieldAttributes.Private, module.ImportReference(typeof(Arguments)));

            type.Fields.Add(isDisposingField);
            type.Fields.Add(resumeFlagField);
            type.Fields.Add(exitFlagField);
            type.Fields.Add(aspectArgsField);
            type.Fields.Add(argsField);

            //
            var aspectField = CreateAspectField(aspect, type);
            ReplaceMoveNext(type, isDisposingField, resumeFlagField, exitFlagField, aspectField, aspectArgsField, argsField);
            ReplaceDispose(type, isDisposingField);
        }



        private FieldDefinition CreateAspectField(CustomAttribute aspect, TypeDefinition type)
        {
            var module = type.Module;

            var @namespace = "SoftCube.Aspects.ImplementationDetails";
            var typeName = $"OnMethodBoundaryAspect<{InstanceCount++}>";
            var typeAttributes = Mono.Cecil.TypeAttributes.NotPublic | Mono.Cecil.TypeAttributes.Sealed;
            var declaringType = new TypeDefinition(@namespace, typeName, typeAttributes);

            var fieldName = "Aspect";
            var fieldAttributes = Mono.Cecil.FieldAttributes.Static | Mono.Cecil.FieldAttributes.InitOnly | Mono.Cecil.FieldAttributes.Assembly;
            var aspectField = new FieldDefinition(fieldName, fieldAttributes, module.ImportReference(GetType()));
            declaringType.Fields.Add(aspectField);

            var constructorName = ".ctor";
            var constructorAttributes = Mono.Cecil.MethodAttributes.Static | Mono.Cecil.MethodAttributes.SpecialName;
            var constructor = new MethodDefinition(constructorName, constructorAttributes, module.TypeSystem.Void);

            declaringType.Methods.Add(constructor);
            module.Types.Add(declaringType);

            var processor = constructor.Body.GetILProcessor();

            var aspectIndex = processor.Emit(aspect);
            processor.Emit(OpCodes.Ldloc, aspectIndex);
            processor.Emit(OpCodes.Stfld, aspectField);
            processor.Emit(OpCodes.Ret);

            return aspectField;
        }

        private void ReplaceMoveNext(TypeDefinition type,
                                     FieldDefinition isDisposingField,
                                     FieldDefinition resumeFlagField,
                                     FieldDefinition exitFlagField,
                                     FieldDefinition aspectField,
                                     FieldDefinition aspectArgsField,
                                     FieldDefinition argsField)
        {
            ///
            var module        = type.Module;
            var method        = type.Methods.Single(m => m.Name == "MoveNext");
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

            {
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, exitFlagField);
                Instruction branch0;
                processor.Append(branch0 = processor.Create(OpCodes.Nop));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, isDisposingField);
                Instruction branch1;
                processor.Append(branch1 = processor.Create(OpCodes.Nop));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, resumeFlagField);
                Instruction branch2;
                processor.Append(branch2 = processor.Create(OpCodes.Nop));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldnull);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldsfld, module.ImportReference(typeof(Arguments).GetField(nameof(Arguments.Empty))));
                processor.Emit(OpCodes.Stfld, argsField);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, argsField);
                processor.Emit(OpCodes.Newobj, module.ImportReference(typeof(MethodExecutionArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) })));
                processor.Emit(OpCodes.Stfld, aspectArgsField);

                processor.Emit(OpCodes.Ldfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnEntry))));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, resumeFlagField);
                Instruction branch3;
                processor.Append(branch3 = processor.Create(OpCodes.Nop));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stfld, resumeFlagField);
                Instruction branch4;
                processor.Append(branch4 = processor.Create(OpCodes.Nop));

                Instruction branchTarget2;
                Instruction branchTarget3;
                processor.Append(branchTarget2 = branchTarget3 = processor.Create(OpCodes.Ldsfld, argsField));
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnResume))));

                Instruction branchTarget0;
                Instruction branchTarget1;
                Instruction branchTarget4;
                processor.Append(branchTarget0 = branchTarget1 = branchTarget4 = processor.Create(OpCodes.Nop));

                // Branch 命令を修正します。
                branch0.OpCode = OpCodes.Brtrue_S;
                branch1.OpCode = OpCodes.Brtrue_S;
                branch2.OpCode = OpCodes.Brtrue_S;
                branch3.OpCode = OpCodes.Brtrue_S;
                branch4.OpCode = OpCodes.Br_S;

                branch0.Operand = branchTarget0;
                branch1.Operand = branchTarget1;
                branch2.Operand = branchTarget2;
                branch3.Operand = branchTarget3;
                branch4.Operand = branchTarget4;
            }

            /// try {
            Instruction tryStart;

            var leave = new Instruction[2];
            var leaveTarget = new Instruction[2];

            int exitFlagIndex = variables.Count;
            variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

            {
                var branch       = new Instruction[8];
                var branchTarget = new Instruction[8];

                int resultIndex = variables.Count;
                variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

                //
                processor.Append(tryStart = processor.Create(OpCodes.Ldc_I4_1));
                processor.Emit(OpCodes.Stloc, exitFlagIndex);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, isDisposingField);
                processor.Emit(OpCodes.Ldc_I4_2);
                processor.Append(branch[0] = processor.Create(OpCodes.Nop));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, originalMethod);
                processor.Emit(OpCodes.Stloc, resultIndex);

                processor.Append(branchTarget[0] = processor.Create(OpCodes.Ldarg_0));
                processor.Emit(OpCodes.Ldfld, isDisposingField);

                processor.Append(branch[1] = processor.Create(OpCodes.Nop));
                processor.Emit(OpCodes.Ldloc, resultIndex);
                processor.Append(branch[2] = processor.Create(OpCodes.Nop));

                processor.Emit(OpCodes.Ldc_I4_0);
                processor.Emit(OpCodes.Stloc, exitFlagIndex);
                processor.Append(branch[3] = processor.Create(OpCodes.Nop));

                processor.Append(branchTarget[1] = branchTarget[2] = processor.Create(OpCodes.Ldc_I4_1));
                processor.Emit(OpCodes.Stloc, exitFlagIndex);

                processor.Append(branchTarget[3] = processor.Create(OpCodes.Ldarg_0));
                processor.Emit(OpCodes.Ldfld, exitFlagField);
                processor.Append(branch[4] = processor.Create(OpCodes.Nop));

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, resumeFlagField);
                processor.Append(branch[5] = processor.Create(OpCodes.Nop));

                processor.Emit(OpCodes.Ldloc, exitFlagIndex);
                processor.Append(branch[6] = processor.Create(OpCodes.Nop));

                processor.Emit(OpCodes.Ldsfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess))));
                processor.Append(branch[7] = processor.Create(OpCodes.Nop));

                processor.Append(branchTarget[6] = processor.Create(OpCodes.Ldarg_0));
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, type.Fields.Single(f => f.Name == "<>2__current"));
                processor.Emit(OpCodes.Box, module.TypeSystem.Int32);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.YieldValue)).GetSetMethod()));

                processor.Emit(OpCodes.Ldsfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnYield))));

                processor.Append(branchTarget[4] = branchTarget[5] = branchTarget[7] = leave[0] = processor.Create(OpCodes.Nop));

                // Branch 命令を書き換える。
                branch[0].OpCode = OpCodes.Beq_S;
                branch[1].OpCode = OpCodes.Brtrue_S;
                branch[2].OpCode = OpCodes.Brfalse_S;
                branch[3].OpCode = OpCodes.Br_S;
                branch[4].OpCode = OpCodes.Brtrue_S;
                branch[5].OpCode = OpCodes.Brfalse_S;
                branch[6].OpCode = OpCodes.Brfalse_S;
                branch[7].OpCode = OpCodes.Br_S;

                branch[0].Operand = branchTarget[0];
                branch[1].Operand = branchTarget[1];
                branch[2].Operand = branchTarget[2];
                branch[3].Operand = branchTarget[3];
                branch[4].Operand = branchTarget[4];
                branch[5].Operand = branchTarget[5];
                branch[6].Operand = branchTarget[6];
                branch[7].Operand = branchTarget[7];
            }

            /// } catch (Exception exception) {
            Instruction catchStart;
            {
                int exceptionIndex = variables.Count;
                variables.Add(new VariableDefinition(module.ImportReference(typeof(Exception))));

                processor.Append(catchStart = processor.Create(OpCodes.Stloc, exceptionIndex));
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Ldloc, exceptionIndex);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Exception)).GetSetMethod()));

                processor.Emit(OpCodes.Ldsfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnException))));
                processor.Emit(OpCodes.Rethrow);

                processor.Append(leave[1] = processor.Create(OpCodes.Nop));                         // IL_01ed:  leave.s    IL_021c
            }

            /// } finally {
            Instruction catchEnd;
            Instruction finallyStart;
            {
                var branch       = new Instruction[3];
                var branchTarget = new Instruction[3];

                processor.Append(catchEnd = finallyStart = processor.Create(OpCodes.Ldarg_0));
                processor.Emit(OpCodes.Ldfld, exitFlagField);
                processor.Append(branch[0] = processor.Create(OpCodes.Nop));        //   IL_01f5:  brtrue.s   IL_021b

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, resumeFlagField);
                processor.Append(branch[1] = processor.Create(OpCodes.Nop));        //   IL_01fd:  brfalse.s  IL_021b

                processor.Emit(OpCodes.Ldloc, exitFlagIndex);
                processor.Append(branch[2] = processor.Create(OpCodes.Nop));        //   IL_0201:  brfalse.s  IL_021b

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Stfld, exitFlagField);

                processor.Emit(OpCodes.Ldsfld, aspectField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, aspectArgsField);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit))));

                processor.Append(branchTarget[0] = branchTarget[1] = branchTarget[2] = processor.Create(OpCodes.Endfinally));

                // Branch 命令を書き換える。
                branch[0].OpCode = OpCodes.Brtrue_S;
                branch[1].OpCode = OpCodes.Brfalse_S;
                branch[2].OpCode = OpCodes.Brfalse_S;

                branch[0].Operand = branchTarget[0];
                branch[1].Operand = branchTarget[1];
                branch[2].Operand = branchTarget[2];
            }

            ///
            Instruction finallyEnd;
            {
                int resultIndex = variables.Count;
                variables.Add(new VariableDefinition(module.TypeSystem.Boolean));

                processor.Append(finallyEnd = leaveTarget[0] = leaveTarget[1] = processor.Create(OpCodes.Ldloc, exitFlagIndex));
                processor.Emit(OpCodes.Ldc_I4_0);
                processor.Emit(OpCodes.Ceq);
                processor.Emit(OpCodes.Stloc, resultIndex);
                processor.Emit(OpCodes.Ldloc, resultIndex);
                processor.Emit(OpCodes.Ret);

                // Leave 命令を書き換える。
                leave[0].OpCode = OpCodes.Leave;
                leave[1].OpCode = OpCodes.Leave_S;

                leave[0].Operand = leaveTarget[0];
                leave[1].Operand = leaveTarget[1];
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
        }

        private void ReplaceDispose(TypeDefinition type, FieldDefinition isDisposingField)
        {
            ///
            var module        = type.Module;
            var method        = type.Methods.Single(m => m.Name == "System.IDisposable.Dispose");
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

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, isDisposingField);

            Instruction branch;
            processor.Append(branch = processor.Create(OpCodes.Nop));
            processor.Emit(OpCodes.Ret);

            Instruction branchTarget;
            processor.Append(branchTarget = processor.Create(OpCodes.Ldarg_0));
            processor.Emit(OpCodes.Ldc_I4_1);
            processor.Emit(OpCodes.Stfld, isDisposingField);

            Instruction tryStart;
            processor.Append(tryStart = processor.Create(OpCodes.Ldarg_0));
            processor.Emit(OpCodes.Call, originalMethod);

            Instruction leave;
            processor.Append(leave = processor.Create(OpCodes.Nop));

            Instruction tryEnd;
            processor.Append(tryEnd = processor.Create(OpCodes.Ldarg_0));
            processor.Emit(OpCodes.Ldc_I4_2);
            processor.Emit(OpCodes.Stfld, isDisposingField);

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Callvirt, type.Methods.Single(m => m.Name == "MoveNext"));

            processor.Emit(OpCodes.Pop);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldc_I4_0);
            processor.Emit(OpCodes.Stfld, isDisposingField);
            processor.Emit(OpCodes.Endfinally);

            Instruction handlerEnd;
            Instruction leaveTarget;
            processor.Append(leaveTarget = handlerEnd = processor.Create(OpCodes.Ret));

            ///
            branch.OpCode  = OpCodes.Brfalse_S;
            branch.Operand = branchTarget;

            leave.OpCode  = OpCodes.Leave_S;
            leave.Operand = leaveTarget;

            /// Finally ハンドラーを追加します。
            var exceptionHandlers = method.Body.ExceptionHandlers;
            var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart     = tryStart,
                TryEnd       = tryEnd,
                HandlerStart = tryEnd,
                HandlerEnd   = handlerEnd,
            };
            exceptionHandlers.Add(handler);
        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <param name="attribute">注入対象の属性。</param>
        private void InjectToNormalMethod(MethodDefinition method, CustomAttribute attribute)
        {
            /// 最後の命令が Throw 命令の場合、Return 命令を追加します。
            var instructions = method.Body.Instructions;
            var processor    = method.Body.GetILProcessor();
            if (instructions.Last().OpCode == OpCodes.Throw)
            {
                processor.Append(processor.Create(OpCodes.Ret));
            }

            /// Entry ハンドラーを注入します。
            var (tryStart, attributeIndex, eventArgsIndex) = InjectEntryHandler(method, attribute);

            /// Return ハンドラーを注入します。
            var tryLast = InjectReturnHandler(method, attributeIndex, eventArgsIndex);

            /// Excetpion ハンドラーを注入します。
            var catchLast = InjectExceptionHandler(method, attributeIndex, eventArgsIndex);

            /// Exit ハンドラーを注入します。
            var finallyLast = InjectExitHandler(method, attributeIndex, eventArgsIndex);

            /// 書き換えによって、不正な状態になった IL コードを修正します。
            {
                /// Try の最終命令を Catch の最終命令への Leave 命令に修正します。
                /// InjectReturnHandler の <returns> コメントを参照してください。
                tryLast.OpCode  = OpCodes.Leave_S;
                tryLast.Operand = catchLast;

                /// 元々の例外ハンドラーの終了位置が明示されていない場合、終了位置を Leave 命令 に変更します。
                var exceptionHandlers = method.Body.ExceptionHandlers;
                foreach (var handler in exceptionHandlers.Where(eh => eh.HandlerEnd == null))
                {
                    handler.HandlerEnd = tryLast;
                }
            }

            /// 例外ハンドラーを追加します。
            AddExceptionHandlers(method, tryStart, tryLast, catchLast, finallyLast);

            /// IL コードを最適化します。
            method.OptimizeIL();
        }

        /// <summary>
        /// Entry ハンドラーを注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <param name="attribute">注入対象の属性。</param>
        /// <returns>Try の先頭命令。</returns>
        private (Instruction, int, int) InjectEntryHandler(MethodDefinition method, CustomAttribute attribute)
        {
            /// 属性をローカル変数にストアします。
            var instructions    = method.Body.Instructions;
            var first           = instructions.First();
            var processor       = method.Body.GetILProcessor();
            var attributeIndex  = processor.InsertBefore(first, attribute);

            /// イベントデータを生成し、ローカル変数にストアします。
            var variables      = method.Body.Variables;
            var eventArgsIndex = variables.Count();
            var module         = method.DeclaringType.Module.Assembly.MainModule;
            variables.Add(new VariableDefinition(module.ImportReference(typeof(MethodExecutionArgs))));

            processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(typeof(MethodExecutionArgs).GetConstructor(new Type[] { typeof(object) }))));
            processor.InsertBefore(first, processor.Create(OpCodes.Stloc, eventArgsIndex));

            /// メソッド情報をイベントデータに設定します。
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(first, processor.Create(OpCodes.Call, module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { }))));
            processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Method)).GetSetMethod())));

            /// パラメーター情報をイベントデータに設定します。
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            {
                /// パラメーターコレクションを生成し、ロードします。
                var parameters = method.Parameters;
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
            }
            processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Arguments)).GetSetMethod())));

            /// OnEntry を呼び出します。
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, attributeIndex));
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnEntry)))));

            /// Try の先頭命令を挿入します。
            Instruction tryStart;
            processor.InsertBefore(first, tryStart = processor.Create(OpCodes.Nop));
            return (tryStart, attributeIndex, eventArgsIndex);
        }

        /// <summary>
        /// Return ハンドラーを注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <param name="attributeIndex">属性の変数インデックス。</param>
        /// <param name="eventArgsIndex">イベントデータの変数インデックス。</param>
        /// <returns>
        /// Try の最終命令 (Catch の最終命令への Leave 命令)。
        /// 転送先の命令が不明であるため、Nop 命令をプレースフォルダとして追加しています。
        /// 正しい Leave 命令に書き換える必要があります。
        /// </returns>
        private Instruction InjectReturnHandler(MethodDefinition method, int attributeIndex, int eventArgsIndex)
        {
            var module       = method.DeclaringType.Module.Assembly.MainModule;
            var processor    = method.Body.GetILProcessor();
            var instructions = method.Body.Instructions;
            var returns      = instructions.Where(i => i.OpCode == OpCodes.Ret).ToArray();

            if (method.HasReturnValue())
            {
                var variables   = method.Body.Variables;
                var resultIndex = variables.Count();
                variables.Add(new VariableDefinition(method.ReturnType));

                /// 新たな Return 命令を追加します。
                Instruction newReturn;                                                              /// 新たな Return 命令。
                processor.Append(newReturn = processor.Create(OpCodes.Ldloc, resultIndex));
                processor.Append(processor.Create(OpCodes.Ret));

                /// Catch の最終命令への Leave 命令を挿入します。
                /// 転送先の命令が不明であるため、Nop 命令をプレースフォルダとして挿入しています。
                /// 後処理にて、正しい Leave 命令に書き換えます。
                Instruction leave;                                                                  /// Leave 命令。
                processor.InsertBefore(newReturn, leave = processor.Create(OpCodes.Nop));

                foreach (var @return in returns)
                {
                    /// Return 命令を書き換えて、戻り値をローカル変数にストアします。
                    @return.OpCode  = OpCodes.Stloc;
                    @return.Operand = variables[resultIndex];

                    /// Leave 命令への Branch 命令を挿入します。
                    /// 以降は Branch 命令の前にコードを挿入します。
                    Instruction branch;                                                             /// Branch 命令。
                    processor.InsertAfter(@return, branch = processor.Create(OpCodes.Br_S, leave));

                    /// 戻り値をイベントデータに設定します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, resultIndex));
                    if (method.ReturnType.IsValueType)
                    {
                        processor.InsertBefore(branch, processor.Create(OpCodes.Box, method.ReturnType));
                    }
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetSetMethod())));

                    /// OnSuccess を呼び出します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, attributeIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, eventArgsIndex));
                    processor.InsertBefore(branch, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess)))));
                }

                return leave;
            }
            else
            {
                /// 新たな Return 命令を追加します。
                Instruction newReturn;                                                              /// 新たな Return 命令。
                processor.Append(newReturn = processor.Create(OpCodes.Ret));

                /// Catch の最終命令への Leave 命令を挿入します。
                /// 転送先の命令が不明であるため、Nop 命令をプレースフォルダとして挿入しています。
                /// 後処理にて、正しい Leave 命令に書き換えます。
                Instruction leave;                                                                  /// Leave 命令。
                processor.InsertBefore(newReturn, leave = processor.Create(OpCodes.Nop));

                foreach (var @return in returns)
                {
                    /// Return 命令を Nop に書き換えます。
                    @return.OpCode  = OpCodes.Nop;
                    @return.Operand = null;

                    /// Leave 命令への Branch 命令を挿入します。
                    /// 以降は Branch 命令の前にコードを挿入します。
                    Instruction branch;                                                             /// Branch 命令。
                    processor.InsertAfter(@return, branch = processor.Create(OpCodes.Br_S, leave));

                    /// OnSuccess を呼び出します。
                    processor.InsertBefore(branch, processor.Create(OpCodes.Ldloc, attributeIndex));
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
        /// <param name="attributeIndex">属性の変数インデックス。</param>
        /// <param name="eventArgsIndex">イベントデータの変数インデックス。</param>
        /// <returns>Catch の最終命令 (Return 命令への Leave 命令)。</returns>
        private Instruction InjectExceptionHandler(MethodDefinition method, int attributeIndex, int eventArgsIndex)
        {
            var module    = method.DeclaringType.Module.Assembly.MainModule;
            var processor = method.Body.GetILProcessor();

            /// 例外オブジェクトをローカル変数にストアします。
            var variables      = method.Body.Variables;
            var exceptionIndex = variables.Count();
            processor.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(Exception))));

            var @return = method.ReturnInstruction();                                               /// Return 命令。
            processor.InsertBefore(@return, processor.Create(OpCodes.Stloc, exceptionIndex));

            /// 例外オブジェクトをイベントデータに設定します。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, exceptionIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Exception)).GetSetMethod())));

            /// OnException を呼び出します。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, attributeIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnException)))));

            /// 例外を再スローします。
            processor.InsertBefore(@return, processor.Create(OpCodes.Rethrow));

            /// Return 命令への Leave 命令を挿入します。
            Instruction leave;                                                                      /// Leave 命令。
            processor.InsertBefore(@return, leave = processor.Create(OpCodes.Leave_S, @return));

            return leave;
        }

        /// <summary>
        /// Exit ハンドラーを注入します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <param name="attributeIndex">属性の変数インデックス。</param>
        /// <param name="eventArgsIndex">イベントデータの変数インデックス。</param>
        /// <returns>Finally の最終命令。</returns>
        private Instruction InjectExitHandler(MethodDefinition method, int attributeIndex, int eventArgsIndex)
        {
            var module    = method.DeclaringType.Module.Assembly.MainModule;
            var processor = method.Body.GetILProcessor();

            /// Finally ハンドラーの先頭命令 (Nop) を挿入します。
            var @return = method.ReturnInstruction();
            processor.InsertBefore(@return, processor.Create(OpCodes.Nop));

            /// OnExit を呼び出します。
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, attributeIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));

            /// EndFinally 命令を挿入します。
            Instruction endFinally;
            processor.InsertBefore(@return, endFinally = processor.Create(OpCodes.Endfinally));

            return endFinally;
        }

        /// <summary>
        /// 例外ハンドラー (Catch ハンドラーと Finally ハンドラー) を追加します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <param name="tryStart">Try の先頭命令。</param>
        /// <param name="tryLast">Try の最終命令。</param>
        /// <param name="catchLast">Catch の最終命令。</param>
        /// <param name="finallyLast">Finally の最終命令。</param>
        private void AddExceptionHandlers(MethodDefinition method, Instruction tryStart, Instruction tryLast, Instruction catchLast, Instruction finallyLast)
        {
            var module = method.DeclaringType.Module.Assembly.MainModule;

            /// Catch ハンドラーを追加します。
            var exceptionHandlers = method.Body.ExceptionHandlers;
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

            /// Finally ハンドラーを追加します。
            {
                var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
                {
                    TryStart     = tryStart,
                    TryEnd       = catchLast.Next,
                    HandlerStart = catchLast.Next,
                    HandlerEnd   = finallyLast.Next,
                };
                exceptionHandlers.Add(handler);
            }
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
