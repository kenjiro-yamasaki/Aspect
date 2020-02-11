using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドインターセプトアスペクト。
    /// </summary>
    public abstract class MethodInterceptionAspect : MethodLevelAspect
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        protected MethodInterceptionAspect()
        {
        }

        #endregion

        #region メソッド

        #region アスペクト (カスタムコード) の注入

        /// <summary>
        /// アスペクト (カスタムコード) を注入します。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        protected sealed override void OnInject(MethodDefinition method, CustomAttribute aspect)
        {
            var injector = new MethodInjector(method, aspect);

            var asyncStateMachineAttribute = method.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");
            var isInvokeAsyncOverridden    = injector.AspectType.Methods.Any(m => m.Name == nameof(OnInvokeAsync));
            if (asyncStateMachineAttribute != null && isInvokeAsyncOverridden)
            {
                CreateDerivedAspectArgs(injector);
                ReplaceAsyncMethod(injector);
                OverrideInvokeAsyncMethod(injector);
                OverrideProceedAsyncMethod(injector);
            }
            else
            {
                CreateDerivedAspectArgs(injector);
                ReplaceMethod(injector);
                OverrideInvokeMethod(injector);
                OverrideProceedMethod(injector);
            }
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs"/> の派生クラスを生成します。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs"/> の派生クラスを生成し、注入対象のメソッドの定義クラスのネスト型とします。
        /// </remarks>
        private void CreateDerivedAspectArgs(MethodInjector injector)
        {
            var method        = injector.TargetMethod;
            var declaringType = method.DeclaringType;
            var module        = declaringType.Module;
            var @namespace    = declaringType.Namespace;

            /// MethodInterceptionArgs の派生クラスを追加します。
            var aspectArgsTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType          = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType   = new TypeDefinition(@namespace, method.Name + "+" + nameof(MethodInterceptionArgs), Mono.Cecil.TypeAttributes.Class, aspectArgsTypeReference) { IsNestedPrivate = true };

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

        #region 通常のメソッド

        /// <summary>
        /// 注入対象のメソッドを書き換えます。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
        /// 元々のメソッドの内容を、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="CreateDerivedAspectArgs(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void ReplaceMethod(MethodInjector injector)
        {
            /// 新たなメソッドを生成し、ターゲットメソッドの内容を移動します。
            injector.ReplaceMethod();

            /// 元々のメソッドを書き換えます。
            {
                var method = injector.TargetMethod;
                method.Body = new Mono.Cecil.Cil.MethodBody(method);
                var declaringType = method.DeclaringType;
                var derivedAspectArgsType = declaringType.NestedTypes.Single(nt => nt.Name == method.Name + "+" + nameof(MethodInterceptionArgs));
                var processor = method.Body.GetILProcessor();

                /// var aspect = new Aspect();
                /// var arguments = new Arguments(...);
                /// var aspectArgs = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnInvoke(aspectArgs);
                /// return (TResult)aspectArgs.ReturnValue;
                injector.CreateAspectVariable(processor);
                injector.CreateArgumentsVariable(processor);
                injector.CreateAspectArgsVariable(processor, derivedAspectArgsType);
                injector.SetMethod(processor);
                injector.InvokeEventHandler(processor, nameof(OnInvoke));
                injector.Return(processor);
            }
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs.Invoke(Arguments)"/> をオーバーライドします。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs.Invoke(Arguments)"/> をオーバーライドして、元々のメソッドを呼びだすコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="ReplaceMethod(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void OverrideInvokeMethod(MethodInjector injector)
        {
            var method         = injector.TargetMethod;
            var module         = method.Module;
            var declaringType  = method.DeclaringType;
            var originalMethod = injector.OriginalMethod;

            /// Invoke メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType          = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType   = declaringType.NestedTypes.Single(nt => nt.Name == method.Name + "+" + nameof(MethodInterceptionArgs));
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
        /// <see cref="MethodInterceptionArgs.Proceed()"/> をオーバーライドします。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs.Proceed()"/> をオーバーライドして、元々のメソッドを呼びだすコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="ReplaceMethod(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void OverrideProceedMethod(MethodInjector injector)
        {
            var method         = injector.TargetMethod;
            var module         = method.Module;
            var declaringType  = method.DeclaringType;
            var originalMethod = injector.OriginalMethod;

            /// Proceed メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType          = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType   = declaringType.NestedTypes.Single(nt => nt.Name == method.Name + "+" + nameof(MethodInterceptionArgs));
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
                if (originalMethod.HasReturnValue())
                {
                    processor.Emit(OpCodes.Ldarg_0);
                }

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(AdviceArgs).GetProperty(nameof(AdviceArgs.Instance)).GetGetMethod()));
                for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = originalMethod.Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

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
                }
                processor.Emit(OpCodes.Callvirt, originalMethod);

                if (originalMethod.HasReturnValue())
                {
                    if (originalMethod.ReturnType.IsValueType)
                    {
                        processor.Emit(OpCodes.Box, originalMethod.ReturnType);
                    }
                    processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.ReturnValue)).GetSetMethod()));
                }

                processor.Emit(OpCodes.Ret);
            }
        }

        #endregion

        #region 非同期メソッド

        /// <summary>
        /// 注入対象のメソッドを書き換えます。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// 新たなメソッドを生成し、元々のメソッドの内容を移動します。
        /// 元々のメソッドの内容を、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="CreateDerivedAspectArgs(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void ReplaceAsyncMethod(MethodInjector injector)
        {
            /// 新たなメソッドを生成し、ターゲットメソッドの内容を移動します。
            injector.ReplaceMethod();

            /// 元々のメソッドを書き換えます。
            {
                var method = injector.TargetMethod;
                method.Body = new Mono.Cecil.Cil.MethodBody(method);
                var declaringType = method.DeclaringType;
                var derivedAspectArgsType = declaringType.NestedTypes.Single(nt => nt.Name == method.Name + "+" + nameof(MethodInterceptionArgs));
                var processor = method.Body.GetILProcessor();

                /// var aspect = new Aspect();
                /// var arguments = new Arguments(...);
                /// var aspectArgs = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspect.OnInvokeAsync(aspectArgs);
                /// return (TResult)aspectArgs.ReturnValue;
                injector.CreateAspectVariable(processor);
                injector.CreateArgumentsVariable(processor);
                injector.CreateAspectArgsVariable(processor, derivedAspectArgsType);
                injector.SetMethod(processor);
                injector.InvokeEventHandler(processor, nameof(OnInvokeAsync));
                processor.Emit(OpCodes.Pop);
                injector.Return(processor);
            }
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs.InvokeAsync(Arguments)"/> をオーバーライドします。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs.InvokeAsync(Arguments)"/> をオーバーライドして、元々のメソッドを呼びだすコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="ReplaceMethod(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void OverrideInvokeAsyncMethod(MethodInjector injector)
        {
            var method         = injector.TargetMethod;
            var module         = method.Module;
            var declaringType  = method.DeclaringType;
            var originalMethod = injector.OriginalMethod;

            /// Invoke メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference    = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType             = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType      = declaringType.NestedTypes.Single(nt => nt.Name == method.Name + "+" + nameof(MethodInterceptionArgs));
            var argumentsTypeReferernce    = module.ImportReference(typeof(Arguments));
            var invokeAsyncMethod          = aspectArgsType.Methods.Single(m => m.Name == nameof(MethodInterceptionArgs.InvokeAsync));
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
        /// <see cref="MethodInterceptionArgs.ProceedAsync()"/> をオーバーライドします。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs.ProceedAsync()"/> をオーバーライドして、元々のメソッドを呼びだすコードに書き換えます。
        /// このメソッドを呼びだす前に <see cref="ReplaceMethod(MethodInjector)"/> を呼びだしてください。
        /// </remarks>
        private void OverrideProceedAsyncMethod(MethodInjector injector)
        {
            var method         = injector.TargetMethod;
            var module         = method.Module;
            var declaringType  = method.DeclaringType;
            var originalMethod = injector.OriginalMethod;

            /// Proceed メソッドのオーバーライドを追加します。
            var aspectArgsTypeReference     = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType              = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType       = declaringType.NestedTypes.Single(nt => nt.Name == method.Name + "+" + nameof(MethodInterceptionArgs));
            var proceedAsyncMethod          = aspectArgsType.Methods.Single(m => m.Name == nameof(MethodInterceptionArgs.ProceedAsync));
            var overridenProceedAsyncMethod = new MethodDefinition(proceedAsyncMethod.Name, (proceedAsyncMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), module.ImportReference(typeof(Task)));

            derivedAspectArgsType.Methods.Add(overridenProceedAsyncMethod);

            /// Proceed メソッドのオーバーライドを実装します。
            {
                var processor = overridenProceedAsyncMethod.Body.GetILProcessor();

                /// public override Task ProceedAsync()
                /// {
                ///     base.ReturnType = ((TInstance)base.Instance).OriginalMethod((TArg0)base.Arguments[0], (TArg1)base.Arguments[1], ...);
                ///     return base.ReturnType;
                /// }
                if (originalMethod.HasReturnValue())
                {
                    processor.Emit(OpCodes.Ldarg_0);
                }

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(AdviceArgs).GetProperty(nameof(AdviceArgs.Instance)).GetGetMethod()));
                for (int parameterIndex = 0; parameterIndex < originalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter     = originalMethod.Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

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
                }
                processor.Emit(OpCodes.Callvirt, originalMethod);

                if (originalMethod.HasReturnValue())
                {
                    if (originalMethod.ReturnType.IsValueType)
                    {
                        processor.Emit(OpCodes.Box, originalMethod.ReturnType);
                    }
                    processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.ReturnValue)).GetSetMethod()));
                }

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.ReturnValue)).GetGetMethod()));
                processor.Emit(OpCodes.Ret);
            }
        }

        #endregion

        #endregion

        #region イベントハンドラー

        /// <summary>
        /// メソッドが実行されたときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッドインターセプション引数。</param>
        public virtual void OnInvoke(MethodInterceptionArgs args)
        {
        }

        /// <summary>
        /// メソッドが実行されたときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッドインターセプション引数。</param>
        public virtual Task OnInvokeAsync(MethodInterceptionArgs args)
        {
            return null;
        }

        #endregion

        #endregion
    }
}
