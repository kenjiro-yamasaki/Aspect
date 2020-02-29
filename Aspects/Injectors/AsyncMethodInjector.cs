using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Asserts;
using System;
using System.Runtime.CompilerServices;

namespace SoftCube.Aspects
{
    /// <summary>
    /// 非同期メソッドの注入。
    /// </summary>
    public class AsyncMethodInjector : MethodInjector
    {
        #region プロパティ

        /// <summary>
        /// asyncStateMachine のローカル変数。
        /// </summary>
        private int AsyncStateMachineVariable { get; set; } = -1;

        /// <summary>
        /// asyncTaskMethodBuilder のローカル変数。
        /// </summary>
        private int AsyncTaskMethodBuilderVariable { get; set; } = -1;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspect">アスペクト属性。</param>
        public AsyncMethodInjector(MethodDefinition targetMethod, CustomAttribute aspect)
            : base(targetMethod, aspect)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 非同期ステートマシンを開始します。
        /// </summary>
        /// <param name="stateMachineType">非同期ステートマシンの <see cref="Type"/>。</param>
        /// <param name="aspectType">アスペクトの <see cref="Type"/>。</param>
        /// <param name="aspectArgsType">アスペクト引数の <see cref="Type"/>。</param>
        public void StartAsyncStateMachine(Type stateMachineType, Type aspectType, Type aspectArgsType)
        {
            Assert.Equal(AsyncStateMachineVariable, -1);
            Assert.Equal(AsyncTaskMethodBuilderVariable, -1);
            Assert.NotEqual(AspectVariable, -1);
            Assert.NotEqual(AspectArgsVariable, -1);

            AsyncStateMachineVariable      = Variables.Count + 0;
            AsyncTaskMethodBuilderVariable = Variables.Count + 1;

            var taskType = TargetMethod.ReturnType;
            if (taskType is GenericInstanceType genericInstanceType)
            {
                var returnType  = genericInstanceType.GenericArguments[0].ToSystemType();
                var builderType = typeof(AsyncTaskMethodBuilder<>).MakeGenericType(returnType);

                Variables.Add(new VariableDefinition(Module.ImportReference(stateMachineType)));
                Variables.Add(new VariableDefinition(Module.ImportReference(builderType)));

                Processor.Emit(OpCodes.Ldloc, AspectVariable);
                Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
                Processor.Emit(OpCodes.Newobj, Module.ImportReference(stateMachineType.GetConstructor(new Type[] { aspectType, aspectArgsType })));
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

                Processor.Emit(OpCodes.Ldloc, AspectVariable);
                Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
                Processor.Emit(OpCodes.Newobj, Module.ImportReference(stateMachineType.GetConstructor(new Type[] { aspectType, aspectArgsType })));
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
