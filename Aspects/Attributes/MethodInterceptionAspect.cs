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
            var (derivedMethodInterceptionArgs, overridenInvokeMethod) = InjectDerivedMethodInterceptionArgs(method);
            var movedMethod = InjectInvokeHandler(method, derivedMethodInterceptionArgs);
            InjectOverriddenInvokeMethod(overridenInvokeMethod, movedMethod);

            /// IL コードを最適化します。
            method.OptimizeIL();

            /// 書き換え後の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        private (TypeDefinition, MethodDefinition) InjectDerivedMethodInterceptionArgs(MethodDefinition method)
        {
            var declaringType = method.DeclaringType;                                               ///
            var module        = method.DeclaringType.Module.Assembly.MainModule;                    /// モジュール。
            var @namespace    = declaringType.Namespace;

            /// 基底クラス (MethodInterceptionArgs) の情報を取得します。
            var baseTypeReference = module.ImportReference(typeof(MethodInterceptionArgs));
            var baseType          = baseTypeReference.Resolve();
            var baseInvokeMethod  = baseType.Methods.Single(m => m.Name == nameof(MethodInterceptionArgs.Invoke));

            /// 派生クラス (メソッド名+MethodInterceptionArgs) を追加します。
            var derivedType = new TypeDefinition(@namespace, method.Name + "+" + nameof(MethodInterceptionArgs), Mono.Cecil.TypeAttributes.Class, baseTypeReference);
            derivedType.IsNestedPrivate = true;
            declaringType.NestedTypes.Add(derivedType);

            /// 派生クラスのコンストラクターを追加します。
            {
                var methodAttributes = Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName;
                var constructor = new MethodDefinition(".ctor", methodAttributes, module.TypeSystem.Void);

                var parameter = new ParameterDefinition("instance", Mono.Cecil.ParameterAttributes.None, module.TypeSystem.Object);
                constructor.Parameters.Add(parameter);

                var processor = constructor.Body.GetILProcessor();
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldarg_1);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetConstructor(new[] { typeof(object) })));
                processor.Emit(OpCodes.Ret);
                derivedType.Methods.Add(constructor);
            }

            /// Invoke メソッドをオーバーライドします。
            var overridenInvokeMethod = new MethodDefinition(baseInvokeMethod.Name, (baseInvokeMethod.Attributes | Mono.Cecil.MethodAttributes.CheckAccessOnOverride) & ~Mono.Cecil.MethodAttributes.NewSlot, baseInvokeMethod.ReturnType);

            var argumentsTypeReferernce = module.ImportReference(typeof(Arguments));
            var parameterType = new ParameterDefinition(argumentsTypeReferernce);

            parameterType.Name = "argument";
            overridenInvokeMethod.Parameters.Add(parameterType);

            derivedType.Methods.Add(overridenInvokeMethod);

            return (derivedType, overridenInvokeMethod);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        private MethodDefinition InjectInvokeHandler(MethodDefinition method, TypeDefinition derivedMethodInterceptionArgsType)
        {
            ///
            var declaringType = method.DeclaringType;                                               //
            var returnType    = method.ReturnType;
            var attributes    = method.Attributes;

            /// 新たなメソッド (Method?) を作成し、元々のメソッド (Method) の内容を移動します。
            var movedMethod = new MethodDefinition(method.Name + "?", attributes, returnType);
            movedMethod.Body = method.Body;

            foreach (var sequencePoint in method.DebugInformation.SequencePoints)
            {
                movedMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            declaringType.Methods.Add(movedMethod);

            /// 元々のメソッド (Method) の内容を、新たなメソッド (Method?) を呼び出すコードに書き換えます。
            method.Body = new Mono.Cecil.Cil.MethodBody(method);

            /// 
            var processor    = method.Body.GetILProcessor();                                        /// IL プロセッサー。
            var instructions = method.Body.Instructions;                                            /// 命令コレクション。
            var module       = method.DeclaringType.Module.Assembly.MainModule;                     /// モジュール。
            var variables    = method.Body.Variables;                                               /// ローカル変数コレクション。

            instructions.Clear();
            variables.Clear();

            /// ローカル変数にアスペクトとイベントデータを追加します。
            var aspectIndex = variables.Count();                                                    /// アスペクトの変数インデックス。
            variables.Add(new VariableDefinition(module.ImportReference(GetType())));

            var eventArgsIndex = variables.Count();                                                 /// イベントデータの変数インデックス。
            variables.Add(new VariableDefinition(module.ImportReference(typeof(MethodInterceptionArgs))));

            /// アスペクトをローカル変数にストアします。
            processor.Append(processor.Create(OpCodes.Newobj, module.ImportReference(GetType().GetConstructor(new Type[] { }))));
            processor.Append(processor.Create(OpCodes.Stloc, aspectIndex));

            /// イベントデータを生成し、ローカル変数にストアします。
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Newobj, derivedMethodInterceptionArgsType.Methods.Single(m => m.Name == ".ctor")));
            processor.Append(processor.Create(OpCodes.Stloc, eventArgsIndex));

            /// メソッド情報をイベントデータに設定します。
            processor.Append(processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.Append(processor.Create(OpCodes.Call, module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { }))));
            processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Method)).GetSetMethod())));

            /// パラメーター情報をイベントデータに設定します。
            processor.Append(processor.Create(OpCodes.Ldloc, eventArgsIndex));
            {
                /// パラメーターコレクションを生成し、ロードします。
                var parameters = method.Parameters;                                                 /// パラメーターコレクション。
                processor.Append(processor.Create(OpCodes.Ldc_I4, parameters.Count));
                processor.Append(processor.Create(OpCodes.Newarr, module.ImportReference(typeof(object))));
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter = parameters[parameterIndex];
                    processor.Append(processor.Create(OpCodes.Dup));
                    processor.Append(processor.Create(OpCodes.Ldc_I4, parameterIndex));
                    processor.Append(processor.Create(OpCodes.Ldarg, parameterIndex + 1));
                    if (parameter.ParameterType.IsValueType)
                    {
                        processor.Append(processor.Create(OpCodes.Box, parameter.ParameterType));
                    }
                    processor.Append(processor.Create(OpCodes.Stelem_Ref));
                }
                processor.Append(processor.Create(OpCodes.Newobj, module.ImportReference(typeof(Arguments).GetConstructor(new Type[] { typeof(object[]) }))));
            }
            processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Arguments)).GetSetMethod())));

            /// OnInvoke を呼び出します。
            processor.Append(processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnInvoke)))));

            /// 戻り値を戻します。
            if (method.HasReturnValue())
            {
                processor.Append(processor.Create(OpCodes.Ldloc, eventArgsIndex));
                processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.ReturnValue)).GetGetMethod())));
                if (returnType.IsValueType)
                {
                    processor.Append(processor.Create(OpCodes.Unbox_Any, returnType));
                }
                processor.Append(processor.Create(OpCodes.Ret));
            }
            else
            {
                processor.Append(processor.Create(OpCodes.Ret));
            }

            return movedMethod;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="overridenInvokeMethod"></param>
        /// <param name="movedMethod"></param>
        private void InjectOverriddenInvokeMethod(MethodDefinition overridenInvokeMethod, MethodDefinition movedMethod)
        {
            var module    = overridenInvokeMethod.Module;
            var processor = overridenInvokeMethod.Body.GetILProcessor();

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Call, module.ImportReference(typeof(AdviceArgs).GetProperty(nameof(AdviceArgs.Instance)).GetGetMethod()));

            for (int parameterIndex = 0; parameterIndex < movedMethod.Parameters.Count; parameterIndex++)
            {
                var parameter = movedMethod.Parameters[parameterIndex];

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Arguments)).GetSetMethod()));

                processor.Emit(OpCodes.Ldind_I4, parameterIndex);
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

            processor.Emit(OpCodes.Callvirt, movedMethod);
            if (movedMethod.ReturnType.IsValueType)
            {
                processor.Emit(OpCodes.Box, movedMethod.ReturnType);
            }
            else
            {
                processor.Emit(OpCodes.Castclass, movedMethod.ReturnType);
            }
            processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.ReturnValue)).GetSetMethod()));

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Call, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.ReturnValue)).GetGetMethod()));
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
