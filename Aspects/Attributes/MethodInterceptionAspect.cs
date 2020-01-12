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
        /// <param name="method">注入対象のメソッド定義。</param>
        protected override void OnInject(MethodDefinition method)
        {
            /// 書き換え前の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();

            /// 
            CreateDerivedMethodInterceptionArgs(method);
            ReplaceMethod(method);
            OverrideInvokeMethod(method);
            OverrideProceedMethod(method);

            /// IL コードを最適化します。
            method.OptimizeIL();

            /// 書き換え後の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs"/> の派生クラスを生成します。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs"/> の派生クラスを生成し、注入対象のメソッドの定義クラスのネスト型とします。
        /// </remarks>
        private void CreateDerivedMethodInterceptionArgs(MethodDefinition method)
        {
            var declaringType = method.DeclaringType;
            var module        = declaringType.Module.Assembly.MainModule;
            var @namespace    = declaringType.Namespace;

            /// 基底クラス (MethodInterceptionArgs) の情報を取得します。
            var methodInterceptionArgsTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var methodInterceptionArgsType          = methodInterceptionArgsTypeReference.Resolve();
            var invokeMethod                        = methodInterceptionArgsType.Methods.Single(m => m.Name == nameof(MethodInterceptionArgs.Invoke));

            /// 派生クラス (メソッド名+MethodInterceptionArgs) を追加します。
            var derivedMethodInterceptionArgsType = new TypeDefinition(@namespace, method.Name + "+" + nameof(MethodInterceptionArgs), Mono.Cecil.TypeAttributes.Class, methodInterceptionArgsTypeReference);
            derivedMethodInterceptionArgsType.IsNestedPrivate = true;
            declaringType.NestedTypes.Add(derivedMethodInterceptionArgsType);

            /// 派生クラスのコンストラクターを追加します。
            var methodAttributes = Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName;
            var constructor = new MethodDefinition(".ctor", methodAttributes, module.TypeSystem.Void);

            var parameter = new ParameterDefinition("instance", Mono.Cecil.ParameterAttributes.None, module.TypeSystem.Object);
            constructor.Parameters.Add(parameter);

            var processor = constructor.Body.GetILProcessor();
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetConstructor(new[] { typeof(object) })));
            processor.Emit(OpCodes.Ret);
            derivedMethodInterceptionArgsType.Methods.Add(constructor);
        }

        /// <summary>
        /// 注入対象のメソッドを書き換えます。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <remarks>
        /// 新たなメソッド (メソッド?) を生成し、元々のメソッドの内容を移動します。
        /// 元々のメソッドの内容を、<see cref="OnInvoke(MethodInterceptionArgs)"/> を呼び出すコードに書き換えます。
        /// このメソッドを呼び出す前に <see cref="MethodInterceptionArgs"/> の派生クラスを生成してください。
        /// </remarks>
        private void ReplaceMethod(MethodDefinition method)
        {
            ///
            var module        = method.DeclaringType.Module.Assembly.MainModule;
            var attributes    = method.Attributes;
            var declaringType = method.DeclaringType;
            var returnType    = method.ReturnType;

            /// 新たなメソッド (メソッド?) を生成し、元々のメソッドの内容を移動します。
            var movedMethod = new MethodDefinition(method.Name + "?", attributes, returnType);
            foreach (var parameter in method.Parameters)
            {
                movedMethod.Parameters.Add(parameter);
            }

            movedMethod.Body = method.Body;

            foreach (var sequencePoint in method.DebugInformation.SequencePoints)
            {
                movedMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            declaringType.Methods.Add(movedMethod);

            /// 元々のメソッドの内容を、新たなメソッド (メソッド?) を呼び出すコードに書き換えます。
            method.Body = new Mono.Cecil.Cil.MethodBody(method);

            /// 
            var processor    = method.Body.GetILProcessor();
            var instructions = method.Body.Instructions;
            var variables    = method.Body.Variables;

            /// ローカル変数にアスペクトとイベントデータを追加します。
            var aspectIndex = variables.Count();                                                    /// アスペクトの変数インデックス。
            variables.Add(new VariableDefinition(module.ImportReference(GetType())));

            var eventArgsIndex = variables.Count();                                                 /// イベントデータの変数インデックス。
            variables.Add(new VariableDefinition(module.ImportReference(typeof(MethodInterceptionArgs))));

            /// アスペクトをローカル変数にストアします。
            processor.Emit(OpCodes.Newobj, module.ImportReference(GetType().GetConstructor(new Type[] { })));
            processor.Emit(OpCodes.Stloc, aspectIndex);

            /// イベントデータを生成し、ローカル変数にストアします。
            var derivedMethodInterceptionArgsType = declaringType.NestedTypes.Single(nt => nt.Name == method.Name + "+" + nameof(MethodInterceptionArgs));
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Newobj, derivedMethodInterceptionArgsType.Methods.Single(m => m.Name == ".ctor"));
            processor.Emit(OpCodes.Stloc, eventArgsIndex);

            /// メソッド情報をイベントデータに設定します。
            processor.Emit(OpCodes.Ldloc, eventArgsIndex);
            processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { })));
            processor.Emit(OpCodes.Callvirt, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Method)).GetSetMethod()));

            /// パラメーター情報をイベントデータに設定します。
            processor.Emit(OpCodes.Ldloc, eventArgsIndex);
            {
                /// パラメーターコレクションを生成し、ロードします。
                var parameters = method.Parameters;                                                 /// パラメーターコレクション。
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
                processor.Emit(OpCodes.Newobj, module.ImportReference(typeof(Arguments).GetConstructor(new Type[] { typeof(object[]) })));
            }
            processor.Emit(OpCodes.Callvirt, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Arguments)).GetSetMethod()));

            /// OnInvoke を呼び出します。
            processor.Emit(OpCodes.Ldloc, aspectIndex);
            processor.Emit(OpCodes.Ldloc, eventArgsIndex);
            processor.Emit(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnInvoke))));

            /// 戻り値を戻します。
            if (method.HasReturnValue())
            {
                processor.Emit(OpCodes.Ldloc, eventArgsIndex);
                processor.Emit(OpCodes.Callvirt, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.ReturnValue)).GetGetMethod()));
                if (returnType.IsValueType)
                {
                    processor.Emit(OpCodes.Unbox_Any, returnType);
                }
                processor.Emit(OpCodes.Ret);
            }
            else
            {
                processor.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs.Invoke(Arguments)"/> をオーバーライドします。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        /// <remarks>
        /// <see cref="MethodInterceptionArgs.Invoke(Arguments)"/> をオーバーライドして、メソッド名?を呼び出すコードに書き換えます。
        /// このメソッドを呼び出す前に注入対象のメソッドを書き換えてください。
        /// </remarks>
        private void OverrideInvokeMethod(MethodDefinition method)
        {
            var module        = method.Module;
            var declaringType = method.DeclaringType;
            var movedMethod   = declaringType.Methods.Single(m => m.Name == method.Name + "?");

            /// Invoke メソッドをオーバーライドします。
            var methodInterceptionArgsTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var methodInterceptionArgsType          = methodInterceptionArgsTypeReference.Resolve();
            var derivedMethodInterceptionArgsType   = declaringType.NestedTypes.Single(nt => nt.Name == method.Name + "+" + nameof(MethodInterceptionArgs));

            var argumentsTypeReferernce = module.ImportReference(typeof(Arguments));
            var parameterType = new ParameterDefinition(argumentsTypeReferernce);
            parameterType.Name = "arguments";

            var invokeMethod = methodInterceptionArgsType.Methods.Single(m => m.Name == nameof(MethodInterceptionArgs.Invoke));
            var overridenInvokeMethod = new MethodDefinition(invokeMethod.Name, (invokeMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), invokeMethod.ReturnType);
            overridenInvokeMethod.Parameters.Add(parameterType);

            derivedMethodInterceptionArgsType.Methods.Add(overridenInvokeMethod);

            /// <see cref="MethodInterceptionArgs.Invoke(Arguments)"> のパラメーターからメソッド?へのパラメーターをロードします。
            var processor = overridenInvokeMethod.Body.GetILProcessor();

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Call, module.ImportReference(typeof(AdviceArgs).GetProperty(nameof(AdviceArgs.Instance)).GetGetMethod()));

            for (int parameterIndex = 0; parameterIndex < movedMethod.Parameters.Count; parameterIndex++)
            {
                var parameter = movedMethod.Parameters[parameterIndex];

                processor.Emit(OpCodes.Ldarg_1);
                processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
                if (parameter.ParameterType.IsValueType)
                {
                    processor.Emit(OpCodes.Box, parameter.ParameterType);
                }
                else
                {
                    processor.Emit(OpCodes.Castclass, parameter.ParameterType);
                }
            }

            /// メソッド?を呼び出します。
            processor.Emit(OpCodes.Callvirt, movedMethod);

            ///
            if (movedMethod.HasReturnValue())
            {
                if (movedMethod.ReturnType.IsValueType)
                {
                    processor.Emit(OpCodes.Box, movedMethod.ReturnType);
                }
                else
                {
                    processor.Emit(OpCodes.Castclass, movedMethod.ReturnType);
                }
            }
            else
            {
                processor.Emit(OpCodes.Ldnull);
            }
            processor.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// <see cref="MethodInterceptionArgs.Proceed"/> をオーバーライドします。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義。</param>
        private void OverrideProceedMethod(MethodDefinition method)
        {
            var module        = method.Module;
            var declaringType = method.DeclaringType;
            var movedMethod   = declaringType.Methods.Single(m => m.Name == method.Name + "?");

            /// Invoke メソッドをオーバーライドします。
            var methodInterceptionArgsTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var methodInterceptionArgsType          = methodInterceptionArgsTypeReference.Resolve();
            var derivedMethodInterceptionArgsType   = declaringType.NestedTypes.Single(nt => nt.Name == method.Name + "+" + nameof(MethodInterceptionArgs));

            var proceedMethod = methodInterceptionArgsType.Methods.Single(m => m.Name == nameof(MethodInterceptionArgs.Proceed));
            var overridenProceedMethod = new MethodDefinition(proceedMethod.Name, (proceedMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~(Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.Abstract), proceedMethod.ReturnType);

            derivedMethodInterceptionArgsType.Methods.Add(overridenProceedMethod);

            /// <see cref="MethodInterceptionArgs.Invoke(Arguments)"> のパラメーターからメソッド?へのパラメーターをロードします。
            var processor = overridenProceedMethod.Body.GetILProcessor();

            if (movedMethod.HasReturnValue())
            {
                processor.Emit(OpCodes.Ldarg_0);
            }
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Call, module.ImportReference(typeof(AdviceArgs).GetProperty(nameof(AdviceArgs.Instance)).GetGetMethod()));

            for (int parameterIndex = 0; parameterIndex < movedMethod.Parameters.Count; parameterIndex++)
            {
                var parameter = movedMethod.Parameters[parameterIndex];

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Arguments)).GetGetMethod()));
                processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
                if (parameter.ParameterType.IsValueType)
                {
                    processor.Emit(OpCodes.Box, parameter.ParameterType);
                }
                else
                {
                    processor.Emit(OpCodes.Castclass, parameter.ParameterType);
                }
            }

            /// メソッド?を呼び出します。
            processor.Emit(OpCodes.Callvirt, movedMethod);

            /// <see cref="MethodExecutionArgs.ReturnValue"/> へメソッド?の戻り値をストアします。
            if (movedMethod.HasReturnValue())
            {
                if (movedMethod.ReturnType.IsValueType)
                {
                    processor.Emit(OpCodes.Box, movedMethod.ReturnType);
                }
                else
                {
                    processor.Emit(OpCodes.Castclass, movedMethod.ReturnType);
                }
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.ReturnValue)).GetSetMethod()));
            }

            /// リターンします。
            processor.Emit(OpCodes.Ret);
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
