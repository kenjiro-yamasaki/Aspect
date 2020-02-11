using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドインターセプトアスペクト。
    /// </summary>
    [Serializable]
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

            /// 
            CreateDerivedAspectArgs(injector);
            ReplaceMethod(injector);
            OverrideInvokeMethod(injector);
            OverrideProceedMethod(injector);

            /// IL コードを最適化します。
            method.OptimizeIL();
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
            var module        = declaringType.Module.Assembly.MainModule;
            var @namespace    = declaringType.Namespace;

            /// MethodInterceptionArgs の派生クラスを追加します。
            var aspectArgsTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var aspectArgsType          = aspectArgsTypeReference.Resolve();
            var derivedAspectArgsType = new TypeDefinition(@namespace, method.Name + "+" + nameof(MethodInterceptionArgs), Mono.Cecil.TypeAttributes.Class, aspectArgsTypeReference);
            derivedAspectArgsType.IsNestedPrivate = true;
            declaringType.NestedTypes.Add(derivedAspectArgsType);

            /// MethodInterceptionArgs の派生クラスにコンストラクターを追加します。
            var methodAttributes = Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName;
            var constructor = new MethodDefinition(".ctor", methodAttributes, module.TypeSystem.Void);

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
        /// 注入対象のメソッドを書き換えます。
        /// </summary>
        /// <param name="injector">メソッドへの注入。</param>
        /// <remarks>
        /// 新たなメソッド (メソッド?) を生成し、元々のメソッドの内容を移動します。
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
                /// aspect.OnEntry(aspectArgs);
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
        /// <see cref="MethodInterceptionArgs.Proceed"/> をオーバーライドします。
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

        #region イベントハンドラー

        /// <summary>
        /// メソッドが実行されたときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッドインターセプション引数。</param>
        public virtual void OnInvoke(MethodInterceptionArgs args)
        {
        }

        #endregion

        #endregion
    }
}
