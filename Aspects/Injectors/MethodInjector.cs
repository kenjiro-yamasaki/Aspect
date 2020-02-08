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

        /// <summary>
        /// ターゲットメソッドの内容を移動した新たなメソッド。
        /// </summary>
        public MethodDefinition OriginalMethod { get; private set; }

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
        /// 新たなメソッドを生成し、ターゲットメソッドの内容を移動します。
        /// </summary>
        public void ReplaceMethod()
        {
            Assert.Null(OriginalMethod);

            OriginalMethod = new MethodDefinition(TargetMethod.Name + "<Original>", TargetMethod.Attributes, TargetMethod.ReturnType);
            foreach (var parameter in TargetMethod.Parameters)
            {
                OriginalMethod.Parameters.Add(parameter);
            }

            OriginalMethod.Body = TargetMethod.Body;

            foreach (var sequencePoint in TargetMethod.DebugInformation.SequencePoints)
            {
                OriginalMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            TargetMethod.DeclaringType.Methods.Add(OriginalMethod);
        
        }

        /// <summary>
        /// Aspect ローカル変数のインスタンスを生成します。
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="insert"></param>
        public void CreateAspectVariable(ILProcessor processor)
        {
            Assert.Equal(AspectVariable, -1);
            AspectVariable = processor.Emit(Aspect);
        }

        /// <summary>
        /// AspectArgs ローカル変数のインスタンスを生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void CreateAspectArgsVariable<TAspectArgs>(ILProcessor processor)
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
                        processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                    }
                    processor.Emit(OpCodes.Newobj, Module.ImportReference(argumentsType.GetConstructor(parameterTypes)));
                }
                else
                {
                    processor.Emit(OpCodes.Ldc_I4, parameters.Count);
                    processor.Emit(OpCodes.Newarr, Module.ImportReference(typeof(object)));
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
                    processor.Emit(OpCodes.Newobj, Module.ImportReference(argumentsType.GetConstructor(new Type[] { typeof(object[]) })));
                }
            }
            processor.Emit(OpCodes.Newobj, Module.ImportReference(typeof(TAspectArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) })));
            processor.Emit(OpCodes.Stloc, AspectArgsVariable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        public void InvokeOriginalMethod(ILProcessor processor)
        {
            Assert.NotNull(OriginalMethod);

            if (OriginalMethod.HasReturnValue())
            {
                processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    for (int parameterIndex = 0; parameterIndex < OriginalMethod.Parameters.Count; parameterIndex++)
                    {
                        processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                    }
                    processor.Emit(OpCodes.Callvirt, OriginalMethod);
                }
                if (OriginalMethod.ReturnType.IsValueType)
                {
                    processor.Emit(OpCodes.Box, OriginalMethod.ReturnType);
                }
                processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetSetMethod()));
            }
            else
            {
                processor.Emit(OpCodes.Ldarg_0);
                for (int parameterIndex = 0; parameterIndex < OriginalMethod.Parameters.Count; parameterIndex++)
                {
                    processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                }
                processor.Emit(OpCodes.Callvirt, OriginalMethod);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        public void Return(ILProcessor processor)
        {
            if (TargetMethod.HasReturnValue())
            {
                processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
                processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetGetMethod()));
                if (TargetMethod.ReturnType.IsValueType)
                {
                    processor.Emit(OpCodes.Unbox_Any, TargetMethod.ReturnType);
                }

                processor.Emit(OpCodes.Ret);
            }
            else
            {
                processor.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// イベントハンドラーを呼びだします。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="eventHandlerName">イベントハンドラー名。</param>
        public void InvokeEventHandler(ILProcessor processor, string eventHandlerName)
        {
            Assert.NotEqual(AspectVariable, -1);
            Assert.NotEqual(AspectArgsVariable, -1);

            processor.Emit(OpCodes.Ldloc, AspectVariable);
            processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            processor.Emit(OpCodes.Callvirt, AspectType.Methods.Single(m => m.Name == eventHandlerName));
        }

        /// <summary>
        /// <see cref="MethodArgs.Method"/> にメソッド情報を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetMethod(ILProcessor processor)
        {
            processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { })));
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Method)).GetSetMethod()));
        }

        /// <summary>
        /// <see cref="MethodArgs.Exception"/> に例外を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetException(ILProcessor processor)
        {
            var variables = TargetMethod.Body.Variables;
            int exceptionVariable = variables.Count;
            variables.Add(new VariableDefinition(Module.ImportReference(typeof(Exception))));

            processor.Emit(OpCodes.Stloc, exceptionVariable);

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            processor.Emit(OpCodes.Ldloc, exceptionVariable);
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.Exception)).GetSetMethod()));
        }

        #endregion
    }
}
