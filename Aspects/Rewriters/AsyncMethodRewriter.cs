using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using SoftCube.Asserts;
using System;
using System.Runtime.CompilerServices;

namespace SoftCube.Aspects
{
    /// <summary>
    /// 非同期メソッドの書き換え。
    /// </summary>
    public class AsyncMethodRewriter : MethodRewriter
    {
        #region プロパティ

        /// <summary>
        /// ローカル変数コレクション。
        /// </summary>
        private Collection<VariableDefinition> Variables => TargetMethod.Body.Variables;

        /// <summary>
        /// AsyncStateMachine のローカル変数。
        /// </summary>
        private int AsyncStateMachineVariable { get; set; } = -1;

        /// <summary>
        /// AsyncTaskMethodBuilder のローカル変数。
        /// </summary>
        private int AsyncTaskMethodBuilderVariable { get; set; } = -1;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        public AsyncMethodRewriter(MethodDefinition targetMethod, CustomAttribute aspectAttribute)
            : base(targetMethod, aspectAttribute)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 非同期ステートマシンを開始します。
        /// </summary>
        /// <param name="stateMachineType">非同期ステートマシンの型。</param>
        /// <param name="aspectAttributeType">アスペクト属性の型。</param>
        /// <param name="aspectArgsType">アスペクト引数の型。</param>
        public void StartAsyncStateMachine(Type stateMachineType, Type aspectAttributeType, Type aspectArgsType, int aspectAttributeVariable, int aspectArgsVariable)
        {
            Assert.Equal(AsyncStateMachineVariable, -1);
            Assert.Equal(AsyncTaskMethodBuilderVariable, -1);
            Assert.NotEqual(aspectAttributeVariable, -1);
            Assert.NotEqual(aspectArgsVariable, -1);

            AsyncStateMachineVariable      = Variables.Count + 0;
            AsyncTaskMethodBuilderVariable = Variables.Count + 1;

            var taskType = TargetMethod.ReturnType;
            if (taskType is GenericInstanceType genericInstanceType)
            {
                var returnType  = genericInstanceType.GenericArguments[0].ToSystemType();
                var builderType = typeof(AsyncTaskMethodBuilder<>).MakeGenericType(returnType);

                Variables.Add(new VariableDefinition(Module.ImportReference(stateMachineType)));
                Variables.Add(new VariableDefinition(Module.ImportReference(builderType)));

                Processor.Emit(OpCodes.Ldloc, aspectAttributeVariable);
                Processor.Emit(OpCodes.Ldloc, aspectArgsVariable);
                Processor.Emit(OpCodes.Newobj, Module.ImportReference(stateMachineType.GetConstructor(new Type[] { aspectAttributeType, aspectArgsType })));
                Processor.Emit(OpCodes.Stloc, AsyncStateMachineVariable);

                Processor.Emit(OpCodes.Ldloc, AsyncStateMachineVariable);
                Processor.Emit(OpCodes.Ldfld, Module.ImportReference(stateMachineType.GetField("Builder")));
                Processor.Emit(OpCodes.Stloc, AsyncTaskMethodBuilderVariable);

                Processor.Emit(OpCodes.Ldloca, AsyncTaskMethodBuilderVariable);
                Processor.Emit(OpCodes.Ldloca, AsyncStateMachineVariable);
                Processor.Emit(OpCodes.Call, Module.ImportReference(builderType.GetMethod("Start").MakeGenericMethod(stateMachineType)));

                Processor.Emit(OpCodes.Ldloc,  AsyncStateMachineVariable);
                Processor.Emit(OpCodes.Ldflda, Module.ImportReference(stateMachineType.GetField("Builder")));
                Processor.Emit(OpCodes.Call, Module.ImportReference(builderType.GetProperty("Task").GetGetMethod()));
                Processor.Emit(OpCodes.Ret);
            }
            else
            {
                var builderType = typeof(AsyncTaskMethodBuilder);

                Variables.Add(new VariableDefinition(Module.ImportReference(stateMachineType)));
                Variables.Add(new VariableDefinition(Module.ImportReference(builderType)));

                Processor.Emit(OpCodes.Ldloc, aspectAttributeVariable);
                Processor.Emit(OpCodes.Ldloc, aspectArgsVariable);
                Processor.Emit(OpCodes.Newobj, Module.ImportReference(stateMachineType.GetConstructor(new Type[] { aspectAttributeType, aspectArgsType })));
                Processor.Emit(OpCodes.Stloc, AsyncStateMachineVariable);

                Processor.Emit(OpCodes.Ldloc, AsyncStateMachineVariable);
                Processor.Emit(OpCodes.Ldfld, Module.ImportReference(stateMachineType.GetField("Builder")));
                Processor.Emit(OpCodes.Stloc, AsyncTaskMethodBuilderVariable);

                Processor.Emit(OpCodes.Ldloca, AsyncTaskMethodBuilderVariable);
                Processor.Emit(OpCodes.Ldloca, AsyncStateMachineVariable);
                Processor.Emit(OpCodes.Call, Module.ImportReference(builderType.GetMethod("Start").MakeGenericMethod(stateMachineType)));

                Processor.Emit(OpCodes.Ldloc,  AsyncStateMachineVariable);
                Processor.Emit(OpCodes.Ldflda, Module.ImportReference(stateMachineType.GetField("Builder")));
                Processor.Emit(OpCodes.Call, Module.ImportReference(builderType.GetProperty("Task").GetGetMethod()));
                Processor.Emit(OpCodes.Ret);
            }
        }

        #endregion
    }
}
