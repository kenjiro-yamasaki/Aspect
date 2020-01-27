using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Asserts;
using System;
using System.Linq;
using System.Reflection;

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

        #region ローカル変数

        /// <summary>
        /// Aspect のローカル変数。
        /// </summary>
        public int AspectVariable { get; private set; } = -1;

        /// <summary>
        /// AspectArgs のローカル変数。
        /// </summary>
        public int AspectArgsVariable { get; private set; } = -1;

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
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="insert"></param>
        public void CreateAspectVariable(ILProcessor processor)
        {
            Assert.Equal(AspectVariable, -1);
            AspectVariable = processor.Emit(Aspect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="insert"></param>
        public void CreateAspectVariable(ILProcessor processor, Instruction insert)
        {
            Assert.Equal(AspectVariable, -1);
            AspectVariable = processor.Emit(null, Aspect);
        }

        /// <summary>
        /// AspectArgs フィールドのインスタンスを生成します。
        /// </summary>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        public void CreateAspectArgsVariable<TAspectArgs>(ILProcessor processor)
        {
            CreateAspectArgsVariable<TAspectArgs>(processor, null);
        }

        /// <summary>
        /// AspectArgs フィールドのインスタンスを生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        public void CreateAspectArgsVariable<TAspectArgs>(ILProcessor processor, Instruction insert)
        {
            Assert.Equal(AspectArgsVariable, -1);
            var variables = TargetMethod.Body.Variables;
            AspectArgsVariable = variables.Count();
            variables.Add(new VariableDefinition(Module.ImportReference(typeof(TAspectArgs))));

            processor.Emit(OpCodes.Ldarg_0);
            {
                var parameters     = TargetMethod.Parameters;
                var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType()).ToArray();

                var argumentsType = parameters.Count switch
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

                if (parameters.Count <= 8)
                {
                    for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                    {
                        var parameter = parameters[parameterIndex];
                        processor.Emit(insert, OpCodes.Ldarg, parameterIndex + 1);
                    }
                    processor.Emit(insert, OpCodes.Newobj, Module.ImportReference(argumentsType.GetConstructor(parameterTypes)));
                }
                else
                {
                    processor.Emit(insert, OpCodes.Ldc_I4, parameters.Count);
                    processor.Emit(insert, OpCodes.Newarr, Module.ImportReference(typeof(object)));
                    for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                    {
                        var parameter = parameters[parameterIndex];
                        processor.Emit(insert, OpCodes.Dup);
                        processor.Emit(insert, OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(insert, OpCodes.Ldarg, parameterIndex + 1);
                        if (parameter.ParameterType.IsValueType)
                        {
                            processor.Emit(insert, OpCodes.Box, parameter.ParameterType);
                        }
                        processor.Emit(insert, OpCodes.Stelem_Ref);
                    }
                    processor.Emit(insert, OpCodes.Newobj, Module.ImportReference(argumentsType.GetConstructor(new Type[] { typeof(object[]) })));
                }
            }
            processor.Emit(insert, OpCodes.Newobj, Module.ImportReference(typeof(MethodExecutionArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) })));
            processor.Emit(insert, OpCodes.Stloc, AspectArgsVariable);
        }

        /// <summary>
        /// イベントハンドラーを呼びだします。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="eventHandlerName">イベントハンドラー名。</param>
        public void InvokeEventHandler(ILProcessor processor, string eventHandlerName)
        {
            InvokeEventHandler(processor, null, eventHandlerName);
        }

        /// <summary>
        /// イベントハンドラーを呼びだします。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        /// <param name="eventHandlerName">イベントハンドラー名。</param>
        public void InvokeEventHandler(ILProcessor processor, Instruction insert, string eventHandlerName)
        {
            Assert.NotEqual(AspectVariable, -1);
            Assert.NotEqual(AspectArgsVariable, -1);

            processor.Emit(insert, OpCodes.Ldloc, AspectVariable);
            processor.Emit(insert, OpCodes.Ldloc, AspectArgsVariable);
            processor.Emit(insert, OpCodes.Callvirt, AspectType.Methods.Single(m => m.Name == eventHandlerName));
        }

        /// <summary>
        /// <see cref="MethodArgs.Method"/> にメソッド情報を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetMethod(ILProcessor processor)
        {
            SetMethod(processor, null);
        }

        /// <summary>
        /// <see cref="MethodArgs.Method"/> にメソッド情報を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        public void SetMethod(ILProcessor processor, Instruction insert)
        {
            processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { })));
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Method)).GetSetMethod()));

            //processor.Emit(insert, OpCodes.Ldarg_0);
            //processor.Emit(insert, OpCodes.Ldloc, AspectArgsVariable);
            //processor.Emit(insert, OpCodes.Ldloc, exceptionVariable);
            //processor.Emit(insert, OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.Exception)).GetSetMethod()));
        }

        /// <summary>
        /// <see cref="MethodArgs.Exception"/> に例外を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="exceptionVariable">例外のローカル変数。</param>
        public void SetException(ILProcessor processor, int exceptionVariable)
        {
            SetException(processor, null, exceptionVariable);
        }

        /// <summary>
        /// <see cref="MethodArgs.Exception"/> に例外を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        /// <param name="exceptionVariable">例外のローカル変数。</param>
        public void SetException(ILProcessor processor, Instruction insert, int exceptionVariable)
        {
            processor.Emit(insert, OpCodes.Ldarg_0);
            processor.Emit(insert, OpCodes.Ldloc, AspectArgsVariable);
            processor.Emit(insert, OpCodes.Ldloc, exceptionVariable);
            processor.Emit(insert, OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.Exception)).GetSetMethod()));
        }

        #endregion
    }
}
