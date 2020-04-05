using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドインターセプト引数の書き換え。
    /// </summary>
    public class MethodInterceptionArgsRewriter : AspectArgsRewriter
    {
        #region プロパティ

        /// <summary>
        /// アスペクトの型。
        /// </summary>
        public TypeDefinition AspectArgsImplType => DeclaringType.NestedTypes.Single(nt => nt.Name == MethodInterceptionArgsImplTypeName);

        /// <summary>
        /// MethodInterceptionArgs の派生クラスの型名。
        /// </summary>
        private string MethodInterceptionArgsImplTypeName { get; set; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        public MethodInterceptionArgsRewriter(MethodDefinition targetMethod, CustomAttribute aspectAttribute)
            : base(targetMethod, aspectAttribute)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="MethodInterceptionArgs"/> の派生クラスを生成します。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs"/> の派生クラスを生成し、注入対象のメソッドの定義クラスのネスト型とします。
        /// </remarks>
        public void CreateAspectArgsImpl()
        {
            MethodInterceptionArgsImplTypeName = $"*{nameof(MethodInterceptionArgs)}<{TargetMethod.Name}>";
            for (int number = 2; true; number++)
            {
                if (!DeclaringType.NestedTypes.Any(nt => nt.Name == MethodInterceptionArgsImplTypeName))
                {
                    break;
                }
                MethodInterceptionArgsImplTypeName = $"*{nameof(MethodInterceptionArgs)}<{TargetMethod.Name}><{number}>";
            }

            var aspectArgsTypeReference = Module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsImplType      = new TypeDefinition(DeclaringType.Namespace, MethodInterceptionArgsImplTypeName, Mono.Cecil.TypeAttributes.Class, aspectArgsTypeReference) { IsNestedPrivate = true };

            DeclaringType.NestedTypes.Add(aspectArgsImplType);
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs"/> のコンストラクターを生成します。
        /// </summary>
        public void CreateConstructor()
        {
            var methodAttributes = Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName;
            var constructor      = new MethodDefinition(".ctor", methodAttributes, Module.TypeSystem.Void);

            // public MethodInterceptionArgsImpl(object instance, Arguments arguments)
            //     : base(instance, arguments)
            // {
            // }
            var instanceParameter  = new ParameterDefinition("instance",  Mono.Cecil.ParameterAttributes.None, Module.TypeSystem.Object);
            var argumentsParameter = new ParameterDefinition("arguments", Mono.Cecil.ParameterAttributes.None, Module.ImportReference(typeof(Arguments)));
            constructor.Parameters.Add(instanceParameter);
            constructor.Parameters.Add(argumentsParameter);

            var aspectArgsImplType = DeclaringType.NestedTypes.Single(nt => nt.Name == MethodInterceptionArgsImplTypeName);
            aspectArgsImplType.Methods.Add(constructor);

            //
            var processor = constructor.Body.GetILProcessor();
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldarg_2);

            processor.CallConstructor(typeof(MethodInterceptionArgs), new[] { typeof(object), typeof(Arguments) });
            //processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodInterceptionArgs).GetConstructor(new[] { typeof(object), typeof(Arguments) })));
            processor.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs.InvokeImpl(Arguments)"/> をオーバーライドします。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        public void OverrideInvokeImplMethod(MethodDefinition originalMethod)
        {
            // InvokeImpl メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference = Module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType          = aspectArgsTypeReference.Resolve();
            var aspectArgsImplType      = DeclaringType.NestedTypes.Single(nt => nt.Name == MethodInterceptionArgsImplTypeName);
            var argumentsTypeReferernce = Module.ImportReference(typeof(Arguments));
            var invokeMethod            = aspectArgsType.Methods.Single(m => m.Name == "InvokeImpl");
            var overridenInvokeMethod   = new MethodDefinition(invokeMethod.Name, (invokeMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), invokeMethod.ReturnType);

            aspectArgsImplType.Methods.Add(overridenInvokeMethod);
            overridenInvokeMethod.Parameters.Add(new ParameterDefinition(argumentsTypeReferernce) { Name = "arguments" });

            // InvokeImpl メソッドのオーバーライドを実装します。
            // protected override object InvokeImpl(Arguments arguments)
            // {
            //     var arg0 = (TArg0)arguments[0];
            //     var arg1 = (TArg1)arguments[1];
            //     ...
            //     var result = ((Program)base.Instance).OriginalFunc(arg0, arg1, ...);
            //     arguments[0] = arg0;
            //     arguments[1] = arg1;
            //     ...
            //     return result;
            // }
            {
                var processor = overridenInvokeMethod.Body.GetILProcessor();
                var variables = overridenInvokeMethod.Body.Variables;

                // var arg0 = (TArg0)arguments[0];
                // var arg1 = (TArg1)arguments[1];
                // ...
                for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = originalMethod.Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        var variable = variables.Count;
                        variables.Add(new VariableDefinition(elementType));

                        processor.Emit(OpCodes.Ldarg_1);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);

                        processor.GetProperty(typeof(Arguments), "Item");
                        if (elementType.IsValueType)
                        {
                            processor.Emit(OpCodes.Unbox_Any, elementType);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Castclass, elementType);
                        }
                        processor.Emit(OpCodes.Stloc, variable);
                    }
                    else
                    {
                        var variable = variables.Count;
                        variables.Add(new VariableDefinition(parameterType));

                        processor.Emit(OpCodes.Ldarg_1);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.GetProperty(typeof(Arguments), "Item");
                        if (parameterType.IsValueType)
                        {
                            processor.Emit(OpCodes.Unbox_Any, parameterType);
                        }
                        else
                        {
                            processor.Emit(OpCodes.Castclass, parameterType);
                        }
                        processor.Emit(OpCodes.Stloc, variable);
                    }
                }

                // var result = ((Program)base.Instance).OriginalFunc(arg0, arg1, ...);
                if (!originalMethod.IsStatic)
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.GetProperty(typeof(AspectArgs), nameof(AspectArgs.Instance));
                }
                for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = originalMethod.Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        processor.Emit(OpCodes.Ldloca, parameterIndex);
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldloc, parameterIndex);
                    }
                }
                processor.Emit(OpCodes.Call, originalMethod);

                if (originalMethod.HasReturnValue())
                {
                    if (originalMethod.ReturnType.IsValueType)
                    {
                        processor.Emit(OpCodes.Box, originalMethod.ReturnType);
                    }
                }
                else
                {
                    processor.Emit(OpCodes.Ldnull);
                }

                // arguments[0] = arg0;
                // arguments[1] = arg1;
                // ...
                for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = originalMethod.Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        processor.Emit(OpCodes.Ldarg_1);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(OpCodes.Ldloc, parameterIndex);
                        if (elementType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, elementType);
                        }
                        processor.SetProperty(typeof(Arguments), "Item");
                    }
                }

                // return result;
                processor.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs.InvokeAsyncImpl(Arguments)"/> をオーバーライドします。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        public void OverrideInvokeAsyncImplMethod(MethodDefinition originalMethod)
        {
            /// InvokeAsyncImpl メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference    = Module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType             = aspectArgsTypeReference.Resolve();
            var aspectArgsImplType         = DeclaringType.NestedTypes.Single(nt => nt.Name == MethodInterceptionArgsImplTypeName);
            var argumentsTypeReferernce    = Module.ImportReference(typeof(Arguments));
            var invokeAsyncMethod          = aspectArgsType.Methods.Single(m => m.Name == "InvokeAsyncImpl");
            var overridenInvokeAsyncMethod = new MethodDefinition(invokeAsyncMethod.Name, (invokeAsyncMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), Module.ImportReference(typeof(Task)));

            overridenInvokeAsyncMethod.Parameters.Add(new ParameterDefinition(argumentsTypeReferernce) { Name = "arguments" });
            aspectArgsImplType.Methods.Add(overridenInvokeAsyncMethod);

            /// InvokeAsyncImpl メソッドのオーバーライドを実装します。
            /// public override Task InvokeAsync(Arguments arguments)
            /// {
            ///     return ((TInstance)base.Instance).OriginalMethod((TArg0)arguments[0], (TArg1)arguments[1], ...);
            /// }
            {
                var processor = overridenInvokeAsyncMethod.Body.GetILProcessor();

                /// return ((TInstance)base.Instance).OriginalMethod((TArg0)arguments[0], (TArg1)arguments[1], ...);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, Module.ImportReference(typeof(AspectArgs).GetProperty(nameof(AspectArgs.Instance)).GetGetMethod()));
                for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = originalMethod.Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    processor.Emit(OpCodes.Ldarg_1);
                    processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                    processor.Emit(OpCodes.Call, Module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
                    if (parameterType.IsValueType)
                    {
                        processor.Emit(OpCodes.Unbox_Any, parameterType);
                    }
                    else
                    {
                        processor.Emit(OpCodes.Castclass, parameterType);
                    }
                }
                processor.Emit(OpCodes.Callvirt, originalMethod);

                if (originalMethod.HasReturnValue())
                {
                    if (originalMethod.ReturnType.IsValueType)
                    {
                        processor.Emit(OpCodes.Box, originalMethod.ReturnType);
                    }
                }
                else
                {
                    processor.Emit(OpCodes.Ldnull);
                }
                processor.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs.TaskResult"/> をオーバーライドします。
        /// </summary>
        public void OverrideTaskResultProperty()
        {
            /// TaskResult メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference   = Module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType            = aspectArgsTypeReference.Resolve();
            var aspectArgsImplType        = DeclaringType.NestedTypes.Single(nt => nt.Name == MethodInterceptionArgsImplTypeName);
            var taskResultMethod          = aspectArgsType.Properties.Single(p => p.Name == nameof(MethodInterceptionArgs.TaskResult)).GetMethod;
            var overridenTaskResultMethod = new MethodDefinition(taskResultMethod.Name, (taskResultMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), Module.TypeSystem.Object);

            aspectArgsImplType.Methods.Add(overridenTaskResultMethod);

            /// TaskResult メソッドのオーバーライドを実装します。
            /// public override object get_TaskResult()
            /// {
            ///     Task<TResult> task = base.Task as Task<TResult>;
            ///     return task.Result;
            /// }
            {
                var processor = overridenTaskResultMethod.Body.GetILProcessor();
                var variables = overridenTaskResultMethod.Body.Variables;

                if (TargetMethod.ReturnType is GenericInstanceType genericInstanceType)
                {
                    var returnType = genericInstanceType.GenericArguments[0].ToSystemType();
                    var taskType   = typeof(Task<>).MakeGenericType(returnType);

                    var taskVariable = variables.Count;
                    variables.Add(new VariableDefinition(Module.ImportReference(taskType)));

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Task)).GetGetMethod()));
                    processor.Emit(OpCodes.Isinst, Module.ImportReference(taskType));
                    processor.Emit(OpCodes.Stloc, taskVariable);
                    processor.Emit(OpCodes.Ldloc, taskVariable);
                    processor.Emit(OpCodes.Callvirt, Module.ImportReference(taskType.GetProperty("Result").GetGetMethod()));
                    if (returnType.IsValueType)
                    {
                        processor.Emit(OpCodes.Box, Module.ImportReference(returnType));
                    }
                    processor.Emit(OpCodes.Ret);
                }
                else
                {
                    processor.Emit(OpCodes.Ldnull);
                    processor.Emit(OpCodes.Ret);
                }
            }
        }

        #endregion
    }
}
