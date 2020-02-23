using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Asserts;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドへの注入。
    /// </summary>
    public class MethodInjector
    {
        #region プロパティ

        /// <summary>
        /// アスペクト。
        /// </summary>
        public CustomAttribute Aspect { get; }

        /// <summary>
        /// アスペクトの型。
        /// </summary>
        public TypeDefinition AspectType { get; }

        /// <summary>
        /// ターゲットメソッド。
        /// </summary>
        public MethodDefinition TargetMethod { get; }

        /// <summary>
        /// モジュール。
        /// </summary>
        public ModuleDefinition Module { get; }

        /// <summary>
        /// ターゲットメソッドの内容を移動したメソッド。
        /// </summary>
        public MethodDefinition OriginalMethod { get; private set; }

        /// <summary>
        /// Arguments の型。
        /// </summary>
        public Type ArgumentsType
        {
            get
            {
                var parameters = TargetMethod.Parameters;
                var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType(removePointer : true)).ToArray();
                return parameters.Count switch
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
            }
        }

        #region ローカル変数

        /// <summary>
        /// aspect のローカル変数。
        /// </summary>
        public int AspectVariable { get; private set; } = -1;

        /// <summary>
        /// arguments のローカル変数。
        /// </summary>
        public int ArgumentsVariable { get; private set; } = -1;

        /// <summary>
        /// aspectArgs のローカル変数。
        /// </summary>
        public int AspectArgsVariable { get; private set; } = -1;

        /// <summary>
        /// objects のローカル変数。
        /// </summary>
        public int ObjectsVariable { get; private set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public int AsyncStateMachineVariable { get; private set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public int AsyncTaskMethodBuilderVariable { get; private set; } = -1;

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        public MethodInjector(MethodDefinition targetMethod, CustomAttribute aspect)
        {
            Aspect       = aspect ?? throw new ArgumentNullException(nameof(aspect));
            AspectType   = Aspect.AttributeType.Resolve();
            TargetMethod = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            Module       = TargetMethod.Module;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 新たなメソッドを生成し、ターゲットメソッドの内容を移動します。
        /// </summary>
        public void ReplaceMethod()
        {
            Assert.Null(OriginalMethod);

            OriginalMethod = new MethodDefinition(TargetMethod.Name + "<Original>", TargetMethod.Attributes, TargetMethod.ReturnType);
            foreach (var parameter in TargetMethod.Parameters)
            {
                OriginalMethod.Parameters.Add(parameter);
            }

            OriginalMethod.Body = TargetMethod.Body;

            foreach (var sequencePoint in TargetMethod.DebugInformation.SequencePoints)
            {
                OriginalMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            TargetMethod.DeclaringType.Methods.Add(OriginalMethod);
        }

        /// <summary>
        /// aspect ローカル変数のインスタンスを生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void CreateAspectVariable(ILProcessor processor)
        {
            Assert.Equal(AspectVariable, -1);
            AspectVariable = processor.Emit(Aspect);
        }

        /// <summary>
        /// arguments ローカル変数のインスタンスを生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void CreateArgumentsVariable(ILProcessor processor)
        {
            var variables = TargetMethod.Body.Variables;

            Assert.Equal(ArgumentsVariable, -1);
            ArgumentsVariable = variables.Count();

            var parameters     = TargetMethod.Parameters;
            var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType(removePointer: true)).ToArray();
            variables.Add(new VariableDefinition(Module.ImportReference(ArgumentsType)));

            if (parameters.Count <= 8)
            {
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter     = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (TargetMethod.IsStatic)
                    {
                        processor.Emit(OpCodes.Ldarg, parameterIndex);
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                    }
                    if (parameterType.IsByReference)
                    {
                        processor.Emit(OpCodes.Ldind_I4);
                    }
                }
                processor.Emit(OpCodes.Newobj, Module.ImportReference(ArgumentsType.GetConstructor(parameterTypes)));
                processor.Emit(OpCodes.Stloc, ArgumentsVariable);
            }
            else
            {
                Assert.Equal(ObjectsVariable, -1);
                ObjectsVariable = variables.Count();
                variables.Add(new VariableDefinition(Module.ImportReference(typeof(object[]))));

                processor.Emit(OpCodes.Ldc_I4, parameters.Count);
                processor.Emit(OpCodes.Newarr, Module.ImportReference(typeof(object)));
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter     = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    processor.Emit(OpCodes.Dup);
                    processor.Emit(OpCodes.Ldc_I4, parameterIndex);

                    if (TargetMethod.IsStatic)
                    {
                        processor.Emit(OpCodes.Ldarg, parameterIndex);
                    }
                    else 
                    {
                        processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                    }

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        processor.Emit(OpCodes.Ldind_I4);
                        if (elementType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, elementType);
                        }
                    }
                    else
                    {
                        if (parameterType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, parameterType);
                        }
                    }
                    processor.Emit(OpCodes.Stelem_Ref);
                }
                processor.Emit(OpCodes.Stloc, ObjectsVariable);

                processor.Emit(OpCodes.Ldloc, ObjectsVariable);
                processor.Emit(OpCodes.Newobj, Module.ImportReference(ArgumentsType.GetConstructor(new Type[] { typeof(object[]) })));
                processor.Emit(OpCodes.Stloc, ArgumentsVariable);
            }
        }

        /// <summary>
        /// aspectArgs ローカル変数のインスタンスを生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="aspectArgsType">AspectArgs の型参照。</param>
        public void CreateAspectArgsVariable(ILProcessor processor, TypeReference aspectArgsType)
        {
            Assert.Equal(AspectArgsVariable, -1);
            Assert.NotEqual(ArgumentsVariable, -1);
            var variables = TargetMethod.Body.Variables;
            AspectArgsVariable = variables.Count();
            variables.Add(new VariableDefinition(aspectArgsType));

            if (TargetMethod.IsStatic)
            {
                processor.Emit(OpCodes.Ldnull);
            }
            else
            {
                processor.Emit(OpCodes.Ldarg_0);
            }
            processor.Emit(OpCodes.Ldloc, ArgumentsVariable);

            processor.Emit(OpCodes.Newobj, Module.ImportReference(aspectArgsType.Resolve().Methods.Single(m => m.Name == ".ctor")));
            processor.Emit(OpCodes.Stloc, AspectArgsVariable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        public void CreateAsyncSteteMachine(ILProcessor processor)
        {
            Assert.Equal(AsyncStateMachineVariable, -1);
            Assert.Equal(AsyncTaskMethodBuilderVariable, -1);
            Assert.NotEqual(AspectVariable, -1);
            Assert.NotEqual(AspectArgsVariable, -1);

            var variables = TargetMethod.Body.Variables;
            AsyncStateMachineVariable      = variables.Count() + 0;
            AsyncTaskMethodBuilderVariable = variables.Count() + 1;

            processor.Emit(OpCodes.Ldloc, AspectVariable);
            processor.Emit(OpCodes.Ldloc, AspectArgsVariable);

            var taskType = TargetMethod.ReturnType;
            if (taskType is GenericInstanceType genericInstanceType)
            {
                var returnType       = genericInstanceType.GenericArguments[0].ToSystemType();
                var stateMachineType = typeof(MethodInterceptionAsyncStateMachine<>).MakeGenericType(returnType);
                var builderType      = typeof(AsyncTaskMethodBuilder<>).MakeGenericType(returnType);

                variables.Add(new VariableDefinition(Module.ImportReference(stateMachineType)));
                variables.Add(new VariableDefinition(Module.ImportReference(builderType)));

                processor.Emit(OpCodes.Newobj, Module.ImportReference(stateMachineType.GetConstructor(new Type[] { typeof(MethodInterceptionAspect), typeof(MethodInterceptionArgs) })));
                processor.Emit(OpCodes.Stloc, AsyncStateMachineVariable);
                processor.Emit(OpCodes.Ldloc, AsyncStateMachineVariable);
                processor.Emit(OpCodes.Ldfld, Module.ImportReference(stateMachineType.GetField("Builder")));
                processor.Emit(OpCodes.Stloc, AsyncTaskMethodBuilderVariable);
            }
            else
            {
                var stateMachineType = typeof(MethodInterceptionAsyncStateMachine);
                var builderType      = typeof(AsyncTaskMethodBuilder);

                variables.Add(new VariableDefinition(Module.ImportReference(stateMachineType)));
                variables.Add(new VariableDefinition(Module.ImportReference(builderType)));

                processor.Emit(OpCodes.Newobj, Module.ImportReference(stateMachineType.GetConstructor(new Type[] { typeof(MethodInterceptionAspect), typeof(MethodInterceptionArgs) })));
                processor.Emit(OpCodes.Stloc, AsyncStateMachineVariable);
                processor.Emit(OpCodes.Ldloc, AsyncStateMachineVariable);
                processor.Emit(OpCodes.Ldfld, Module.ImportReference(stateMachineType.GetField("Builder")));
                processor.Emit(OpCodes.Stloc, AsyncTaskMethodBuilderVariable);
            }
        }

        public void StartAsyncTaskMethodBuilder(ILProcessor processor)
        {
            //IL_0026:  ldloca.s   V_1
            //IL_0028:  ldloca.s   V_0
            //IL_002a:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::Start<class SoftCube.Aspects.Program/'<例外_>d__1'>(!!0&)
            processor.Emit(OpCodes.Ldloca, AsyncTaskMethodBuilderVariable);
            processor.Emit(OpCodes.Ldloca, AsyncStateMachineVariable);

            var taskType = TargetMethod.ReturnType;
            if (taskType is GenericInstanceType genericInstanceType)
            {
                var returnType       = genericInstanceType.GenericArguments[0].ToSystemType();
                var stateMachineType = typeof(MethodInterceptionAsyncStateMachine<>).MakeGenericType(returnType);
                var builderType      = typeof(AsyncTaskMethodBuilder<>).MakeGenericType(returnType);

                processor.Emit(OpCodes.Call, Module.ImportReference(builderType.GetMethod("Start").MakeGenericMethod(stateMachineType)));

                processor.Emit(OpCodes.Ldloc,  AsyncStateMachineVariable);
                processor.Emit(OpCodes.Ldflda, Module.ImportReference(stateMachineType.GetField("Builder")));
                processor.Emit(OpCodes.Call, Module.ImportReference(builderType.GetProperty("Task").GetGetMethod()));
                processor.Emit(OpCodes.Ret);
                //processor.Emit(OpCodes.Ldloca, AsyncTaskMethodBuilderVariable);
                //processor.Emit(OpCodes.Call, Module.ImportReference(builderType.GetProperty("Task").GetGetMethod()));
                //processor.Emit(OpCodes.Ret);
            }
            else 
            {
                var builderType = typeof(AsyncTaskMethodBuilder);
                var stateMachineType = typeof(MethodInterceptionAsyncStateMachine);

                processor.Emit(OpCodes.Call, Module.ImportReference(builderType.GetMethod("Start").MakeGenericMethod(stateMachineType)));


                //IL_007f:  ldloc.1
                //IL_0080:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> SoftCube.Aspects.Program/'<例外>d__1'::'<>t__builder'
                //IL_0085:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::get_Task()
                //IL_008a:  stloc.0
                //IL_008b:  ldloc.0
                //IL_008c:  ret
                processor.Emit(OpCodes.Ldloc,  AsyncStateMachineVariable);
                processor.Emit(OpCodes.Ldflda, Module.ImportReference(stateMachineType.GetField("Builder")));
                processor.Emit(OpCodes.Call, Module.ImportReference(builderType.GetProperty("Task").GetGetMethod()));
                processor.Emit(OpCodes.Ret);




                //processor.Emit(OpCodes.Ldloca, AsyncTaskMethodBuilderVariable);
                //processor.Emit(OpCodes.Call, Module.ImportReference(builderType.GetProperty("Task").GetGetMethod()));
                //processor.Emit(OpCodes.Ret);
            }

            processor.Body.Method.Log();
        }





        /// <summary>
        /// ターゲットメソッドの内容を移動したメソッドを呼びだします。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void InvokeOriginalMethod(ILProcessor processor)
        {
            Assert.NotNull(OriginalMethod);

            /// 戻り値がある場合、AspectArgs をスタックにロードします (戻り値を AspectArgs.ReturnValue に設定するための前処理)。
            if (OriginalMethod.HasReturnValue())
            {
                processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            }

            ///
            UpdateArguments(processor);

            /// 引数をスタックにロードします。
            /// ターゲットメソッドの内容を移動したメソッドを呼びだします。
            if (!TargetMethod.IsStatic)
            {
                processor.Emit(OpCodes.Ldarg_0);
            }

            var parameters = TargetMethod.Parameters;
            for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
            {
                if (TargetMethod.IsStatic)
                {
                    processor.Emit(OpCodes.Ldarg, parameterIndex);
                }
                else
                {
                    processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                }
            }

            processor.Emit(OpCodes.Call, OriginalMethod);

            /// 戻り値を AspectArgs.ReturnValue に設定します。
            if (OriginalMethod.HasReturnValue())
            {
                if (OriginalMethod.ReturnType.IsValueType)
                {
                    processor.Emit(OpCodes.Box, OriginalMethod.ReturnType);
                }
                processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetSetMethod()));
            }

            ///
            UpdateAspectArguments(processor);
        }

        /// <summary>
        /// 戻り値を戻します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void Return(ILProcessor processor)
        {
            if (TargetMethod.HasReturnValue())
            {
                processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
                processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetGetMethod()));
                if (TargetMethod.ReturnType.IsValueType)
                {
                    processor.Emit(OpCodes.Unbox_Any, TargetMethod.ReturnType);
                }

                processor.Emit(OpCodes.Ret);
            }
            else
            {
                processor.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// イベントハンドラーを呼びだします。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="eventHandlerName">イベントハンドラー名。</param>
        public void InvokeEventHandler(ILProcessor processor, string eventHandlerName)
        {
            Assert.NotEqual(AspectVariable, -1);
            Assert.NotEqual(AspectArgsVariable, -1);

            processor.Emit(OpCodes.Ldloc, AspectVariable);
            processor.Emit(OpCodes.Ldloc, AspectArgsVariable);

            var aspectType = AspectType;
            while (true)
            {
                var eventHandler = aspectType.Methods.SingleOrDefault(m => m.Name == eventHandlerName);
                if (eventHandler != null)
                {
                    processor.Emit(OpCodes.Callvirt, Module.ImportReference(eventHandler));
                    break;
                }
                else
                {
                    Assert.NotNull(aspectType.BaseType);
                    aspectType = aspectType.BaseType.Resolve();
                }
            }
        }

        /// <summary>
        /// aspectArgs.Method にメソッド情報を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetMethod(ILProcessor processor)
        {
            processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { })));
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Method)).GetSetMethod()));
        }

        /// <summary>
        /// aspectArgs.Argument の内容で引数を更新します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void UpdateArguments(ILProcessor processor)
        {
            var parameters = TargetMethod.Parameters;
            if (8 < parameters.Count)
            {
                for (int parameterIndex = 0; parameterIndex < OriginalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        if (TargetMethod.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }

                        processor.Emit(OpCodes.Ldloc, ObjectsVariable);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(OpCodes.Ldelem_Ref);
                        if (elementType.IsValueType)
                        {
                            processor.Emit(OpCodes.Unbox, elementType);
                            processor.Emit(OpCodes.Ldobj, elementType);
                        }
                        processor.Emit(OpCodes.Stind_I4);
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldloc, ObjectsVariable);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(OpCodes.Ldelem_Ref);
                        if (parameterType.IsValueType)
                        {
                            processor.Emit(OpCodes.Unbox_Any, parameterType);
                        }
                        if (TargetMethod.IsStatic)
                        {
                            processor.Emit(OpCodes.Starg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Starg, parameterIndex + 1);
                        }
                    }
                }
            }
            else
            {
                var propertyNames = new string[] { "Arg0", "Arg1", "Arg2", "Arg3", "Arg4", "Arg5", "Arg6", "Arg7" };

                for (int parameterIndex = 0; parameterIndex < OriginalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = OriginalMethod.Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        if (TargetMethod.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        processor.Emit(OpCodes.Ldfld, Module.ImportReference(ArgumentsType.GetField(propertyNames[parameterIndex])));
                        processor.Emit(OpCodes.Stind_I4);
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        processor.Emit(OpCodes.Ldfld, Module.ImportReference(ArgumentsType.GetField(propertyNames[parameterIndex])));
                        if (TargetMethod.IsStatic)
                        {
                            processor.Emit(OpCodes.Starg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Starg, parameterIndex + 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 引数の内容で aspectArgs.Argument を更新します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void UpdateAspectArguments(ILProcessor processor)
        {
            var parameters = TargetMethod.Parameters;
            if (8 < parameters.Count)
            {
                for (int parameterIndex = 0; parameterIndex < OriginalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        processor.Emit(OpCodes.Ldloc, ObjectsVariable);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        if (TargetMethod.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        processor.Emit(OpCodes.Ldind_I4);
                        if (elementType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, elementType);
                        }
                        processor.Emit(OpCodes.Stelem_Ref);
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldloc, ObjectsVariable);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        if (TargetMethod.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        if (parameterType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, parameterType);
                        }
                        processor.Emit(OpCodes.Stelem_Ref);
                    }
                }
            }
            else
            {
                var propertyNames = new string[] { "Arg0", "Arg1", "Arg2", "Arg3", "Arg4", "Arg5", "Arg6", "Arg7" };

                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter     = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        if (TargetMethod.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        processor.Emit(OpCodes.Ldind_I4);
                        processor.Emit(OpCodes.Stfld, Module.ImportReference(ArgumentsType.GetField(propertyNames[parameterIndex])));
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        if (TargetMethod.IsStatic)
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        processor.Emit(OpCodes.Stfld, Module.ImportReference(ArgumentsType.GetField(propertyNames[parameterIndex])));
                    }
                }
            }
        }

        /// <summary>
        /// aspectArgs.Exception に例外を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetException(ILProcessor processor)
        {
            var variables = TargetMethod.Body.Variables;
            int exceptionVariable = variables.Count;
            variables.Add(new VariableDefinition(Module.ImportReference(typeof(Exception))));

            processor.Emit(OpCodes.Stloc, exceptionVariable);

            processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            processor.Emit(OpCodes.Ldloc, exceptionVariable);
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.Exception)).GetSetMethod()));
        }

        #endregion
    }
}
