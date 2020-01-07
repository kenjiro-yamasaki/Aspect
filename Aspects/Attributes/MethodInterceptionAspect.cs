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
            // 書き換え前の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();

            //
            var declaringType = method.DeclaringType;                                               //
            var returnType    = method.ReturnType;
            var attributes    = method.Attributes;

            // 新たなメソッド (Method?) を作成し、元々のメソッド (Method) の内容を移動します。
            var movedMethod = new MethodDefinition(method.Name + "?", attributes, returnType);
            movedMethod.Body = method.Body;
            declaringType.Methods.Add(movedMethod);

            // 元々のメソッド (Method) の内容を、新たなメソッド (Method?) を呼び出すコードに書き換えます。
            method.Body = new Mono.Cecil.Cil.MethodBody(method);

            // 
            var processor    = method.Body.GetILProcessor();                                        // IL プロセッサー。
            var instructions = method.Body.Instructions;                                            // 命令コレクション。
            var module       = method.DeclaringType.Module.Assembly.MainModule;                     // モジュール。
            var variables    = method.Body.Variables;                                               // ローカル変数コレクション。

            // ローカル変数にアスペクトとイベントデータを追加します。
            var aspectIndex = variables.Count();                                                    // アスペクトの変数インデックス。
            variables.Add(new VariableDefinition(module.ImportReference(GetType())));

            var eventArgsIndex = variables.Count();                                                 // イベントデータの変数インデックス。
            variables.Add(new VariableDefinition(module.ImportReference(typeof(MethodInterceptionArgs))));

            // アスペクトをローカル変数にストアします。
            processor.Append(processor.Create(OpCodes.Newobj, module.ImportReference(GetType().GetConstructor(new Type[] { }))));
            processor.Append(processor.Create(OpCodes.Stloc, aspectIndex));

            // イベントデータを生成し、ローカル変数にストアします。
            processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Newobj, module.ImportReference(typeof(MethodInterceptionArgs).GetConstructor(new Type[] { typeof(object) }))));
            processor.Append(processor.Create(OpCodes.Stloc, eventArgsIndex));

            // メソッド情報をイベントデータに設定します。
            processor.Append(processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.Append(processor.Create(OpCodes.Call, module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { }))));
            processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodInterceptionArgs).GetProperty(nameof(MethodInterceptionArgs.Method)).GetSetMethod())));

            // パラメーター情報をイベントデータに設定します。
            processor.Append(processor.Create(OpCodes.Ldloc, eventArgsIndex));
            {
                // パラメーターコレクションを生成し、ロードします。
                var parameters = method.Parameters;                                                 // パラメーターコレクション。
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

            // OnInvoke を呼び出します。
            processor.Append(processor.Create(OpCodes.Ldloc, aspectIndex));
            processor.Append(processor.Create(OpCodes.Ldloc, eventArgsIndex));
            processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnInvoke)))));

            // 戻り値を戻します。
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

            // IL コードを最適化します。
            method.OptimizeIL();

            // 書き換え後の IL コードをログ出力します (デバッグ用、削除可)。
            method.Log();
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
