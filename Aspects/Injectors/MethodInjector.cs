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
        /// ターゲットメソッドの内容を移動したメソッド。
        /// </summary>
        public MethodDefinition OriginalMethod { get; private set; }

        /// <summary>
        /// Arguments の型。
        /// </summary>
        public Type ArgumentsType
        {
            get
            {
                var parameters = TargetMethod.Parameters;
                var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType()).ToArray();
                return parameters.Count switch
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
            }
        }

        #region ローカル変数

        /// <summary>
        /// aspect のローカル変数。
        /// </summary>
        public int AspectVariable { get; private set; } = -1;

        /// <summary>
        /// arguments のローカル変数。
        /// </summary>
        public int ArgumentsVariable { get; private set; } = -1;

        /// <summary>
        /// aspectArgs のローカル変数。
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
        /// aspect ローカル変数のインスタンスを生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void CreateAspectVariable(ILProcessor processor)
        {
            Assert.Equal(AspectVariable, -1);
            AspectVariable = processor.Emit(Aspect);
        }

        /// <summary>
        /// arguments ローカル変数のインスタンスを生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void CreateArgumentsVariable(ILProcessor processor)
        {
            Assert.Equal(ArgumentsVariable, -1);
            var variables = TargetMethod.Body.Variables;
            ArgumentsVariable = variables.Count();

            var parameters = TargetMethod.Parameters;
            var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType()).ToArray();
            variables.Add(new VariableDefinition(Module.ImportReference(ArgumentsType)));

            if (parameters.Count <= 8)
            {
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter = parameters[parameterIndex];
                    processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                }
                processor.Emit(OpCodes.Newobj, Module.ImportReference(ArgumentsType.GetConstructor(parameterTypes)));
                processor.Emit(OpCodes.Stloc, ArgumentsVariable);
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
                processor.Emit(OpCodes.Newobj, Module.ImportReference(ArgumentsType.GetConstructor(new Type[] { typeof(object[]) })));
                processor.Emit(OpCodes.Stloc, ArgumentsVariable);
            }
        }

        /// <summary>
        /// aspectArgs ローカル変数のインスタンスを生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void CreateAspectArgsVariable<TAspectArgs>(ILProcessor processor)
        {
            Assert.Equal(AspectArgsVariable, -1);
            Assert.NotEqual(ArgumentsVariable, -1);
            var variables = TargetMethod.Body.Variables;
            AspectArgsVariable = variables.Count();
            variables.Add(new VariableDefinition(Module.ImportReference(typeof(TAspectArgs))));

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldloc, ArgumentsVariable);

            processor.Emit(OpCodes.Newobj, Module.ImportReference(typeof(TAspectArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) })));
            processor.Emit(OpCodes.Stloc, AspectArgsVariable);
        }

        /// <summary>
        /// ターゲットメソッドの内容を移動したメソッドを呼びだします。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void InvokeOriginalMethod(ILProcessor processor)
        {
            Assert.NotNull(OriginalMethod);

            /// 戻り値がある場合、AspectArgs をスタックにロードします (戻り値を AspectArgs.ReturnValue に設定するための前処理)。
            if (OriginalMethod.HasReturnValue())
            {
                processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            }

            /// 
            processor.Emit(OpCodes.Ldarg_0);

            /// 引数をスタックにロードします。
            var parameters = TargetMethod.Parameters;
            if (parameters.Count <= 8)
            {
                var propertyNames = new string[] { "Arg0", "Arg1", "Arg2", "Arg3", "Arg4", "Arg5", "Arg6", "Arg7" };

                for (int parameterIndex = 0; parameterIndex < OriginalMethod.Parameters.Count; parameterIndex++)
                {
                    processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                    processor.Emit(OpCodes.Call, Module.ImportReference(ArgumentsType.GetProperty(propertyNames[parameterIndex]).GetGetMethod()));
                }
            }
            else
            {
                for (int parameterIndex = 0; parameterIndex < OriginalMethod.Parameters.Count; parameterIndex++)
                {
                    var parameter = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                    processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                    processor.Emit(OpCodes.Callvirt, Module.ImportReference(ArgumentsType.GetMethod(nameof(ArgumentsArray.GetArgument))));
                    if (parameterType.IsValueType)
                    {
                        processor.Emit(OpCodes.Unbox_Any, parameterType);
                    }
                }
            }

            /// ターゲットメソッドの内容を移動したメソッドを呼びだします。
            processor.Emit(OpCodes.Callvirt, OriginalMethod);

            /// 戻り値を AspectArgs.ReturnValue に設定します。
            if (OriginalMethod.HasReturnValue())
            {
                if (OriginalMethod.ReturnType.IsValueType)
                {
                    processor.Emit(OpCodes.Box, OriginalMethod.ReturnType);
                }
                processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetSetMethod()));
            }
        }

        /// <summary>
        /// 戻り値を戻します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
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

            var aspectType = AspectType;
            while (true)
            {
                var eventHandler = aspectType.Methods.SingleOrDefault(m => m.Name == eventHandlerName);
                if (eventHandler != null)
                {
                    processor.Emit(OpCodes.Callvirt, Module.ImportReference(eventHandler));
                    break;
                }
                else
                {
                    Assert.NotNull(aspectType.BaseType);
                    aspectType = aspectType.BaseType.Resolve();
                }
            }
        }

        /// <summary>
        /// aspectArgs.Method にメソッド情報を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetMethod(ILProcessor processor)
        {
            processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { })));
            processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Method)).GetSetMethod()));
        }

        /// <summary>
        /// aspectArgs.Exception に例外を設定します。
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
