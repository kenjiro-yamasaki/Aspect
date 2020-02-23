using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドインターセプト引数への注入。
    /// </summary>
    public class MethodInterceptionArgsInjector : AdviceArgsInjector
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        public MethodInterceptionArgsInjector(MethodDefinition targetMethod, CustomAttribute aspect)
            : base(targetMethod, aspect)
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
        public void CreateDerivedAspectArgs()
        {
            var declaringType = TargetMethod.DeclaringType;
            var module        = declaringType.Module;
            var @namespace    = declaringType.Namespace;

            /// MethodInterceptionArgs の派生クラスを追加します。
            var aspectArgsTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType          = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType   = new TypeDefinition(@namespace, TargetMethod.FullName + "+" + nameof(MethodInterceptionArgs), Mono.Cecil.TypeAttributes.Class, aspectArgsTypeReference) { IsNestedPrivate = true };

            declaringType.NestedTypes.Add(derivedAspectArgsType);

            /// MethodInterceptionArgs の派生クラスにコンストラクターを追加します。
            var methodAttributes = Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName;
            var constructor      = new MethodDefinition(".ctor", methodAttributes, module.TypeSystem.Void);

            var instanceParameter  = new ParameterDefinition("instance", Mono.Cecil.ParameterAttributes.None, module.TypeSystem.Object);
            var argumentsParameter = new ParameterDefinition("arguments", Mono.Cecil.ParameterAttributes.None, module.ImportReference(typeof(Arguments)));
            constructor.Parameters.Add(instanceParameter);
            constructor.Parameters.Add(argumentsParameter);

            var processor = constructor.Body.GetILProcessor();
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldarg_2);
            processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetConstructor(new[] { typeof(object), typeof(Arguments) })));
            processor.Emit(OpCodes.Ret);

            derivedAspectArgsType.Methods.Add(constructor);
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs.Invoke(Arguments)"/> をオーバーライドします。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs.Invoke(Arguments)"/> をオーバーライドして、元々のメソッドを呼びだすコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="ReplaceMethod(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        public void OverrideInvokeMethod(MethodDefinition originalMethod)
        {
            var module         = TargetMethod.Module;
            var declaringType  = TargetMethod.DeclaringType;

            /// Invoke メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType          = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType   = declaringType.NestedTypes.Single(nt => nt.Name == TargetMethod.FullName + "+" + nameof(MethodInterceptionArgs));
            var argumentsTypeReferernce = module.ImportReference(typeof(Arguments));
            var invokeMethod            = aspectArgsType.Methods.Single(m => m.Name == nameof(MethodInterceptionArgs.Invoke));
            var overridenInvokeMethod   = new MethodDefinition(invokeMethod.Name, (invokeMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), invokeMethod.ReturnType);

            overridenInvokeMethod.Parameters.Add(new ParameterDefinition(argumentsTypeReferernce) { Name = "arguments" });
            derivedAspectArgsType.Methods.Add(overridenInvokeMethod);

            /// Invoke メソッドのオーバーライドを実装します。
            {
                var processor = overridenInvokeMethod.Body.GetILProcessor();

                /// public override object Invoke(Arguments arguments)
                /// {
                ///     return ((TInstance)base.Instance).OriginalMethod((TArg0)arguments[0], (TArg1)arguments[1], ...);
                /// }
                var variables = overridenInvokeMethod.Body.Variables;
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
                        processor.Emit(OpCodes.Call, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
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
                        processor.Emit(OpCodes.Call, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
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

                ///
                if (!originalMethod.IsStatic)
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Call, module.ImportReference(typeof(AdviceArgs).GetProperty(nameof(AdviceArgs.Instance)).GetGetMethod()));
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
                        processor.Emit(OpCodes.Callvirt, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetSetMethod()));
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldarg_1);
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(OpCodes.Ldloc, parameterIndex);
                        if (parameterType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, parameterType);
                        }
                        processor.Emit(OpCodes.Callvirt, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetSetMethod()));
                    }
                }

                processor.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs.Proceed()"/> をオーバーライドします。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs.Proceed()"/> をオーバーライドして、元々のメソッドを呼びだすコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="ReplaceMethod(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        public void OverrideProceedMethod(MethodDefinition originalMethod)
        {
            var module        = TargetMethod.Module;
            var declaringType = TargetMethod.DeclaringType;

            /// Proceed メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType          = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType   = declaringType.NestedTypes.Single(nt => nt.Name == TargetMethod.FullName + "+" + nameof(MethodInterceptionArgs));
            var proceedMethod           = aspectArgsType.Methods.Single(m => m.Name == nameof(MethodInterceptionArgs.Proceed));
            var overridenProceedMethod  = new MethodDefinition(proceedMethod.Name, (proceedMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), proceedMethod.ReturnType);

            derivedAspectArgsType.Methods.Add(overridenProceedMethod);

            /// Proceed メソッドのオーバーライドを実装します。
            {
                var processor = overridenProceedMethod.Body.GetILProcessor();

                /// public override void Proceed()
                /// {
                ///     base.ReturnType = ((TInstance)base.Instance).OriginalMethod((TArg0)base.Arguments[0], (TArg1)base.Arguments[1], ...);
                /// }
                var variables = overridenProceedMethod.Body.Variables;
                for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = originalMethod.Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        var variable = variables.Count;
                        variables.Add(new VariableDefinition(elementType));

                        processor.Emit(OpCodes.Ldarg_0);
                        processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Arguments)).GetGetMethod()));
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(OpCodes.Call, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
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

                        processor.Emit(OpCodes.Ldarg_0);
                        processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Arguments)).GetGetMethod()));
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(OpCodes.Call, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
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

                ///
                if (originalMethod.HasReturnValue())
                {
                    processor.Emit(OpCodes.Ldarg_0);
                }

                if (!originalMethod.IsStatic)
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Call, module.ImportReference(typeof(AdviceArgs).GetProperty(nameof(AdviceArgs.Instance)).GetGetMethod()));
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
                    processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.ReturnValue)).GetSetMethod()));
                }

                ///
                for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = originalMethod.Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        processor.Emit(OpCodes.Ldarg_0);
                        processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Arguments)).GetGetMethod()));
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(OpCodes.Ldloc, parameterIndex);
                        if (elementType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, elementType);
                        }
                        processor.Emit(OpCodes.Callvirt, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetSetMethod()));
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldarg_0);
                        processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Arguments)).GetGetMethod()));
                        processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(OpCodes.Ldloc, parameterIndex);
                        if (parameterType.IsValueType)
                        {
                            processor.Emit(OpCodes.Box, parameterType);
                        }
                        processor.Emit(OpCodes.Callvirt, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetSetMethod()));
                    }
                }

                processor.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs.InvokeAsyncImpl(Arguments)"/> をオーバーライドします。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs.InvokeAsyncImpl(Arguments)"/> をオーバーライドして、元々のメソッドを呼びだすコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="ReplaceMethod(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        public void OverrideInvokeAsyncMethod(MethodDefinition originalMethod)
        {
            var module        = TargetMethod.Module;
            var declaringType = TargetMethod.DeclaringType;

            /// Invoke メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference    = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType             = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType      = declaringType.NestedTypes.Single(nt => nt.Name == TargetMethod.FullName + "+" + nameof(MethodInterceptionArgs));
            var argumentsTypeReferernce    = module.ImportReference(typeof(Arguments));
            var invokeAsyncMethod          = aspectArgsType.Methods.Single(m => m.Name == "InvokeAsyncImpl");
            var overridenInvokeAsyncMethod = new MethodDefinition(invokeAsyncMethod.Name, (invokeAsyncMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), module.ImportReference(typeof(Task)));

            overridenInvokeAsyncMethod.Parameters.Add(new ParameterDefinition(argumentsTypeReferernce) { Name = "arguments" });
            derivedAspectArgsType.Methods.Add(overridenInvokeAsyncMethod);

            /// Invoke メソッドのオーバーライドを実装します。
            {
                var processor = overridenInvokeAsyncMethod.Body.GetILProcessor();

                /// public override Task InvokeAsync(Arguments arguments)
                /// {
                ///     return ((TInstance)base.Instance).OriginalMethod((TArg0)arguments[0], (TArg1)arguments[1], ...);
                /// }
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(AdviceArgs).GetProperty(nameof(AdviceArgs.Instance)).GetGetMethod()));
                for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = originalMethod.Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    processor.Emit(OpCodes.Ldarg_1);
                    processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                    processor.Emit(OpCodes.Call, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
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
        /// 
        /// </summary>
        public void OverrideGetTaskResult()
        {
            var module        = TargetMethod.Module;
            var declaringType = TargetMethod.DeclaringType;

            /// Invoke メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference      = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType               = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType        = declaringType.NestedTypes.Single(nt => nt.Name == TargetMethod.FullName + "+" + nameof(MethodInterceptionArgs));
            var getTaskResultMethod          = aspectArgsType.Methods.Single(m => m.Name == "GetTaskResult");
            var overridenGetTaskResultMethod = new MethodDefinition(getTaskResultMethod.Name, (getTaskResultMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), module.TypeSystem.Object);

            derivedAspectArgsType.Methods.Add(overridenGetTaskResultMethod);

            /// GetTaskResult メソッドのオーバーライドを実装します。
            {
                var processor = overridenGetTaskResultMethod.Body.GetILProcessor();
                var variables = overridenGetTaskResultMethod.Body.Variables;

                if (TargetMethod.ReturnType is GenericInstanceType genericInstanceType)
                {
                    var returnType = genericInstanceType.GenericArguments[0].ToSystemType();
                    var taskType   = typeof(Task<>).MakeGenericType(returnType);

                    var taskVariable = variables.Count;
                    variables.Add(new VariableDefinition(module.ImportReference(taskType)));

                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Task)).GetGetMethod()));
                    processor.Emit(OpCodes.Isinst, module.ImportReference(taskType));
                    processor.Emit(OpCodes.Stloc, taskVariable);
                    processor.Emit(OpCodes.Ldloc, taskVariable);
                    processor.Emit(OpCodes.Callvirt, module.ImportReference(taskType.GetProperty("Result").GetGetMethod()));
                    if (returnType.IsValueType)
                    {
                        processor.Emit(OpCodes.Box, module.ImportReference(returnType));
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

        ///// <summary>
        ///// <see cref="MethodInterceptionArgs.ProceedAsyncImpl()"/> をオーバーライドします。
        ///// </summary>
        ///// <param name="injector">メソッドへの注入。</param>
        ///// <remarks>
        ///// <see cref="MethodInterceptionArgs.ProceedAsyncImpl()"/> をオーバーライドして、元々のメソッドを呼びだすコードに書き換えます。
        ///// このメソッドを呼びだす前に <see cref="ReplaceMethod(MethodInjector)"/> を呼びだしてください。
        ///// </remarks>
        //public void OverrideProceedAsyncMethod(MethodDefinition originalMethod)
        //{
        //    var module        = TargetMethod.Module;
        //    var declaringType = TargetMethod.DeclaringType;

        //    /// Proceed メソッドのオーバーライドを追加します。
        //    var aspectArgsTypeReference     = module.ImportReference(typeof(MethodInterceptionArgs));
        //    var aspectArgsType              = aspectArgsTypeReference.Resolve();
        //    var derivedAspectArgsType       = declaringType.NestedTypes.Single(nt => nt.Name == TargetMethod.FullName + "+" + nameof(MethodInterceptionArgs));
        //    var proceedAsyncMethod          = aspectArgsType.Methods.Single(m => m.Name == "ProceedAsyncImpl");
        //    var overridenProceedAsyncMethod = new MethodDefinition(proceedAsyncMethod.Name, (proceedAsyncMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), module.ImportReference(typeof(Task)));

        //    derivedAspectArgsType.Methods.Add(overridenProceedAsyncMethod);

        //    /// Proceed メソッドのオーバーライドを実装します。
        //    {
        //        var processor = overridenProceedAsyncMethod.Body.GetILProcessor();

        //        /// public override Task ProceedAsync()
        //        /// {
        //        ///     base.ReturnType = ((TInstance)base.Instance).OriginalMethod((TArg0)base.Arguments[0], (TArg1)base.Arguments[1], ...);
        //        ///     return base.ReturnType;
        //        /// }
        //        if (originalMethod.HasReturnValue())
        //        {
        //            processor.Emit(OpCodes.Ldarg_0);
        //        }

        //        processor.Emit(OpCodes.Ldarg_0);
        //        processor.Emit(OpCodes.Call, module.ImportReference(typeof(AdviceArgs).GetProperty(nameof(AdviceArgs.Instance)).GetGetMethod()));
        //        for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
        //        {
        //            var parameter     = originalMethod.Parameters[parameterIndex];
        //            var parameterType = parameter.ParameterType;

        //            processor.Emit(OpCodes.Ldarg_0);
        //            processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Arguments)).GetGetMethod()));
        //            processor.Emit(OpCodes.Ldc_I4, parameterIndex);
        //            processor.Emit(OpCodes.Call, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
        //            if (parameterType.IsValueType)
        //            {
        //                processor.Emit(OpCodes.Unbox_Any, parameterType);
        //            }
        //            else
        //            {
        //                processor.Emit(OpCodes.Castclass, parameterType);
        //            }
        //        }
        //        processor.Emit(OpCodes.Callvirt, originalMethod);

        //        //if (originalMethod.HasReturnValue())
        //        //{
        //        //    if (originalMethod.ReturnType.IsValueType)
        //        //    {
        //        //        processor.Emit(OpCodes.Box, originalMethod.ReturnType);
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    processor.Emit(OpCodes.Ldnull);
        //        //}
        //        processor.Emit(OpCodes.Castclass, Module.ImportReference(typeof(Task)));
        //        processor.Emit(OpCodes.Ret);




        //        //if (originalMethod.HasReturnValue())
        //        //{
        //        //    if (originalMethod.ReturnType.IsValueType)
        //        //    {
        //        //        processor.Emit(OpCodes.Box, originalMethod.ReturnType);
        //        //    }
        //        //    processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Task)).GetSetMethod()));
        //        //}

        //        //processor.Emit(OpCodes.Ldarg_0);
        //        //processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Task)).GetGetMethod()));
        //        //processor.Emit(OpCodes.Ret);
        //    }
        //}

        #endregion
    }
}
