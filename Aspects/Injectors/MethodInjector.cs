using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using SoftCube.Asserts;
using System;
using System.Linq;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッドの注入。
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
        public TypeDefinition AspectType => Aspect.AttributeType.Resolve();

        /// <summary>
        /// アスペクトの <see cref="Type"/>。
        /// </summary>
        public Type AspectSystemType => Aspect.AttributeType.ToSystemType();

        /// <summary>
        /// ターゲットメソッド。
        /// </summary>
        public MethodDefinition TargetMethod { get; }

        /// <summary>
        /// ターゲットメソッドの宣言型。
        /// </summary>
        public TypeDefinition DeclaringType => TargetMethod.DeclaringType;

        /// <summary>
        /// モジュール。
        /// </summary>
        public ModuleDefinition Module => TargetMethod.Module;

        /// <summary>
        /// IL プロセッサー。
        /// </summary>
        public ILProcessor Processor => TargetMethod.Body.GetILProcessor();

        /// <summary>
        /// ターゲットメソッドの内容を移動したメソッド。
        /// </summary>
        public MethodDefinition OriginalMethod { get; private set; }

        #region パラメーター

        /// <summary>
        /// パラメーターコレクション。
        /// </summary>
        protected Collection<ParameterDefinition> Parameters => TargetMethod.Parameters;

        /// <summary>
        /// Arguments の <see cref="Type"/>。
        /// </summary>
        protected Type ArgumentsSystemType
        {
            get
            {
                var parameters = TargetMethod.Parameters;
                var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType(removePointer : true)).ToArray();
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
                    _ => typeof(ArgumentsArray),
                };
            }
        }

        #endregion

        #region ローカル変数

        /// <summary>
        /// ローカル変数コレクション。
        /// </summary>
        protected Collection<VariableDefinition> Variables => TargetMethod.Body.Variables;

        /// <summary>
        /// aspect のローカル変数。
        /// </summary>
        protected int AspectVariable { get; set; } = -1;

        /// <summary>
        /// arguments のローカル変数。
        /// </summary>
        protected int ArgumentsVariable { get; set; } = -1;

        /// <summary>
        /// aspectArgs のローカル変数。
        /// </summary>
        protected int AspectArgsVariable { get; set; } = -1;

        /// <summary>
        /// objects のローカル変数。
        /// </summary>
        protected int ObjectsVariable { get; set; } = -1;

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
            TargetMethod = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
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
            DeclaringType.Methods.Add(OriginalMethod);

            TargetMethod.Body = new Mono.Cecil.Cil.MethodBody(TargetMethod);
        }

        /// <summary>
        /// aspect ローカル変数のインスタンスを生成します。
        /// </summary>
        public void CreateAspectVariable()
        {
            Assert.Equal(AspectVariable, -1);

            /// ローカル変数を追加します。
            AspectVariable = Variables.Count();
            Variables.Add(new VariableDefinition(Module.ImportReference(AspectSystemType)));

            /// アスペクトを生成して、ローカル変数にストアします。
            var argumentTypes  = Aspect.ConstructorArguments.Select(a => a.Type.ToSystemType());
            var argumentValues = Aspect.ConstructorArguments.Select(a => a.Value);
            foreach (var argumentValue in argumentValues)
            {
                switch (argumentValue)
                {
                    case bool value:
                        if (value)
                        {
                            Processor.Emit(OpCodes.Ldc_I4_1);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldc_I4_0);
                        }
                        break;

                    case sbyte value:
                        Processor.Emit(OpCodes.Ldc_I4_S, value);
                        break;

                    case short value:
                        Processor.Emit(OpCodes.Ldc_I4, value);
                        break;

                    case int value:
                        Processor.Emit(OpCodes.Ldc_I4, value);
                        break;

                    case long value:
                        Processor.Emit(OpCodes.Ldc_I8, value);
                        break;

                    case byte value:
                        Processor.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                        break;

                    case ushort value:
                        Processor.Emit(OpCodes.Ldc_I4, (short)value);
                        break;

                    case uint value:
                        Processor.Emit(OpCodes.Ldc_I4, (int)value);
                        break;

                    case ulong value:
                        Processor.Emit(OpCodes.Ldc_I8, (long)value);
                        break;

                    case float value:
                        Processor.Emit(OpCodes.Ldc_R4, value);
                        break;

                    case double value:
                        Processor.Emit(OpCodes.Ldc_R8, value);
                        break;

                    case char value:
                        Processor.Emit(OpCodes.Ldc_I4, value);
                        break;

                    case string value:
                        Processor.Emit(OpCodes.Ldstr, value);
                        break;

                    case TypeReference value:
                        Processor.Emit(OpCodes.Ldtoken, Module.ImportReference(value));
                        Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))));
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            Processor.Emit(OpCodes.Newobj, Module.ImportReference(AspectSystemType.GetConstructor(argumentTypes.ToArray())));
            Processor.Emit(OpCodes.Stloc, AspectVariable);

            /// アスペクトのプロパティを設定します。
            foreach (var property in Aspect.Properties)
            {
                var propertyName  = property.Name;
                var propertyValue = property.Argument.Value;

                Processor.Emit(OpCodes.Ldloc, AspectVariable);

                switch (propertyValue)
                {
                    case bool value:
                        if (value)
                        {
                            Processor.Emit(OpCodes.Ldc_I4_1);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldc_I4_0);
                        }
                        break;

                    case sbyte value:
                        Processor.Emit(OpCodes.Ldc_I4_S, value);
                        break;

                    case short value:
                        Processor.Emit(OpCodes.Ldc_I4, value);
                        break;

                    case int value:
                        Processor.Emit(OpCodes.Ldc_I4, value);
                        break;

                    case long value:
                        Processor.Emit(OpCodes.Ldc_I8, value);
                        break;

                    case byte value:
                        Processor.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                        break;

                    case ushort value:
                        Processor.Emit(OpCodes.Ldc_I4, (short)value);
                        break;

                    case uint value:
                        Processor.Emit(OpCodes.Ldc_I4, (int)value);
                        break;

                    case ulong value:
                        Processor.Emit(OpCodes.Ldc_I8, (long)value);
                        break;

                    case float value:
                        Processor.Emit(OpCodes.Ldc_R4, value);
                        break;

                    case double value:
                        Processor.Emit(OpCodes.Ldc_R8, value);
                        break;

                    case char value:
                        Processor.Emit(OpCodes.Ldc_I4, value);
                        break;

                    case string value:
                        Processor.Emit(OpCodes.Ldstr, value);
                        break;

                    case TypeReference value:
                        Processor.Emit(OpCodes.Ldtoken, Module.ImportReference(value));
                        Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))));
                        break;

                    default:
                        throw new NotSupportedException();
                }

                Processor.Emit(OpCodes.Callvirt, Module.ImportReference(AspectSystemType.GetProperty(propertyName).GetSetMethod()));
            }
        }

        /// <summary>
        /// arguments ローカル変数のインスタンスを生成します。
        /// </summary>
        public void CreateArgumentsVariable()
        {
            Assert.Equal(ArgumentsVariable, -1);

            /// ローカル変数を追加します。
            ArgumentsVariable = Variables.Count();
            Variables.Add(new VariableDefinition(Module.ImportReference(ArgumentsSystemType)));

            // Arguments を生成して、ローカル変数にストアします。
            if (Parameters.Count <= 8)
            {
                for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
                {
                    var parameter     = Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (TargetMethod.IsStatic)
                    {
                        Processor.Emit(OpCodes.Ldarg, parameterIndex);
                    }
                    else
                    {
                        Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                    }

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();
                        Processor.EmitLdind(elementType);
                    }
                }
                Processor.Emit(OpCodes.Newobj, Module.ImportReference(ArgumentsSystemType.GetConstructor(Parameters.Select(p => p.ParameterType.ToSystemType(removePointer: true)).ToArray())));
                Processor.Emit(OpCodes.Stloc, ArgumentsVariable);
            }
            else
            {
                Assert.Equal(ObjectsVariable, -1);
                ObjectsVariable = Variables.Count();
                Variables.Add(new VariableDefinition(Module.ImportReference(typeof(object[]))));

                Processor.Emit(OpCodes.Ldc_I4, Parameters.Count);
                Processor.Emit(OpCodes.Newarr, Module.ImportReference(typeof(object)));
                for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
                {
                    var parameter     = Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    Processor.Emit(OpCodes.Dup);
                    Processor.Emit(OpCodes.Ldc_I4, parameterIndex);

                    if (TargetMethod.IsStatic)
                    {
                        Processor.Emit(OpCodes.Ldarg, parameterIndex);
                    }
                    else 
                    {
                        Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                    }

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        Processor.EmitLdind(elementType);
                        if (elementType.IsValueType)
                        {
                            Processor.Emit(OpCodes.Box, elementType);
                        }
                    }
                    else
                    {
                        if (parameterType.IsValueType)
                        {
                            Processor.Emit(OpCodes.Box, parameterType);
                        }
                    }
                    Processor.Emit(OpCodes.Stelem_Ref);
                }
                Processor.Emit(OpCodes.Stloc, ObjectsVariable);

                Processor.Emit(OpCodes.Ldloc, ObjectsVariable);
                Processor.Emit(OpCodes.Newobj, Module.ImportReference(ArgumentsSystemType.GetConstructor(new Type[] { typeof(object[]) })));
                Processor.Emit(OpCodes.Stloc, ArgumentsVariable);
            }
        }

        /// <summary>
        /// aspectArgs ローカル変数のインスタンスを生成します。
        /// </summary>
        /// <param name="aspectArgsType">AspectArgs の型参照。</param>
        public void CreateAspectArgsVariable(TypeReference aspectArgsType)
        {
            Assert.Equal(AspectArgsVariable, -1);
            Assert.NotEqual(ArgumentsVariable, -1);

            // ローカル変数を追加します。
            AspectArgsVariable = Variables.Count();
            Variables.Add(new VariableDefinition(aspectArgsType));

            // AspectArgs を生成し、ローカル変数にストアします。
            if (TargetMethod.IsStatic)
            {
                Processor.Emit(OpCodes.Ldnull);
            }
            else
            {
                Processor.Emit(OpCodes.Ldarg_0);
            }
            Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);

            Processor.Emit(OpCodes.Newobj, Module.ImportReference(aspectArgsType.Resolve().Methods.Single(m => m.Name == ".ctor")));
            Processor.Emit(OpCodes.Stloc, AspectArgsVariable);
        }

        /// <summary>
        /// aspectArgs.Method にメソッド情報を設定します。
        /// </summary>
        public void SetMethod()
        {
            Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { })));
            Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Method)).GetSetMethod()));
        }

        /// <summary>
        /// 引数に aspectArgs.Argument の内容を設定します。
        /// </summary>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを設定対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを設定します。
        /// <c>false</c> の場合、すべての引数を設定します。
        /// </param>
        public void SetAspectArguments(bool pointerOnly)
        {
            if (Parameters.Count <= 8)
            {
                var fieldNames = new string[] { "Arg0", "Arg1", "Arg2", "Arg3", "Arg4", "Arg5", "Arg6", "Arg7" };

                for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
                {
                    var parameter     = Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        if (TargetMethod.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        Processor.Emit(OpCodes.Ldfld, Module.ImportReference(ArgumentsSystemType.GetField(fieldNames[parameterIndex])));
                        Processor.EmitStind(elementType);
                    }
                    else if (!pointerOnly)
                    {
                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        Processor.Emit(OpCodes.Ldfld, Module.ImportReference(ArgumentsSystemType.GetField(fieldNames[parameterIndex])));
                        if (TargetMethod.IsStatic)
                        {
                            Processor.Emit(OpCodes.Starg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Starg, parameterIndex + 1);
                        }
                    }
                }
            }
            else
            {
                for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
                {
                    var parameter     = Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        if (TargetMethod.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }

                        Processor.Emit(OpCodes.Ldloc, ObjectsVariable);
                        Processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        Processor.Emit(OpCodes.Ldelem_Ref);
                        if (elementType.IsValueType)
                        {
                            Processor.Emit(OpCodes.Unbox, elementType);
                            Processor.Emit(OpCodes.Ldobj, elementType);
                        }
                        Processor.EmitStind(elementType);
                    }
                    else if (!pointerOnly)
                    {
                        Processor.Emit(OpCodes.Ldloc, ObjectsVariable);
                        Processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        Processor.Emit(OpCodes.Ldelem_Ref);
                        if (parameterType.IsValueType)
                        {
                            Processor.Emit(OpCodes.Unbox_Any, parameterType);
                        }
                        if (TargetMethod.IsStatic)
                        {
                            Processor.Emit(OpCodes.Starg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Starg, parameterIndex + 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// aspectArgs.Argument に引数の内容を設定します。
        /// </summary>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを設定対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを設定します。
        /// <c>false</c> の場合、すべての引数を設定します。
        /// </param>
        public void SetArguments(bool pointerOnly)
        {
            if (Parameters.Count <= 8)
            {
                var fieldNames = new string[] { "Arg0", "Arg1", "Arg2", "Arg3", "Arg4", "Arg5", "Arg6", "Arg7" };

                for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
                {
                    var parameter     = Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        if (TargetMethod.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        Processor.EmitLdind(elementType);
                        Processor.Emit(OpCodes.Stfld, Module.ImportReference(ArgumentsSystemType.GetField(fieldNames[parameterIndex])));
                    }
                    else if (!pointerOnly)
                    {
                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        if (TargetMethod.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        Processor.Emit(OpCodes.Stfld, Module.ImportReference(ArgumentsSystemType.GetField(fieldNames[parameterIndex])));
                    }
                }
            }
            else
            {
                for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
                {
                    var parameter     = Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsByReference)
                    {
                        var elementType = parameterType.GetElementType();

                        Processor.Emit(OpCodes.Ldloc, ObjectsVariable);
                        Processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        if (TargetMethod.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        Processor.EmitLdind(elementType);
                        if (elementType.IsValueType)
                        {
                            Processor.Emit(OpCodes.Box, elementType);
                        }
                        Processor.Emit(OpCodes.Stelem_Ref);
                    }
                    else if (!pointerOnly)
                    {
                        Processor.Emit(OpCodes.Ldloc, ObjectsVariable);
                        Processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        if (TargetMethod.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        if (parameterType.IsValueType)
                        {
                            Processor.Emit(OpCodes.Box, parameterType);
                        }
                        Processor.Emit(OpCodes.Stelem_Ref);
                    }
                }
            }
        }

        /// <summary>
        /// イベントハンドラーを呼びだします。
        /// </summary>
        /// <param name="eventHandlerName">イベントハンドラー名。</param>
        public void InvokeEventHandler(string eventHandlerName)
        {
            Assert.NotEqual(AspectVariable, -1);
            Assert.NotEqual(AspectArgsVariable, -1);

            Processor.Emit(OpCodes.Ldloc, AspectVariable);
            Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);

            var aspectType = AspectType;
            while (true)
            {
                var eventHandler = aspectType.Methods.SingleOrDefault(m => m.Name == eventHandlerName);
                if (eventHandler != null)
                {
                    Processor.Emit(OpCodes.Callvirt, Module.ImportReference(eventHandler));
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
        /// ターゲットメソッドの内容を移動したメソッドを呼びだします。
        /// </summary>
        public void InvokeOriginalMethod()
        {
            Assert.NotNull(OriginalMethod);

            /// 戻り値がある場合、AspectArgs をスタックにロードします (戻り値を AspectArgs.ReturnValue に設定するための前処理)。
            if (OriginalMethod.HasReturnValue())
            {
                Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            }

            /// 引数に aspectArgs.Argument の内容を設定します。
            SetAspectArguments(pointerOnly: false);

            /// 引数をスタックにロードします。
            /// ターゲットメソッドの内容を移動したメソッドを呼びだします。
            if (!TargetMethod.IsStatic)
            {
                Processor.Emit(OpCodes.Ldarg_0);
            }

            for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
            {
                if (TargetMethod.IsStatic)
                {
                    Processor.Emit(OpCodes.Ldarg, parameterIndex);
                }
                else
                {
                    Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                }
            }

            Processor.Emit(OpCodes.Call, OriginalMethod);

            /// 戻り値を AspectArgs.ReturnValue に設定します。
            if (OriginalMethod.HasReturnValue())
            {
                if (OriginalMethod.ReturnType.IsValueType)
                {
                    Processor.Emit(OpCodes.Box, OriginalMethod.ReturnType);
                }
                Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetSetMethod()));
            }

            /// aspectArgs.Argument に引数の内容を設定します (ポインタ引数のみ)。
            SetArguments(pointerOnly: true);
        }

        /// <summary>
        /// aspectArgs.Exception に例外を設定します。
        /// </summary>
        public void SetException()
        {
            int exceptionVariable = Variables.Count;
            Variables.Add(new VariableDefinition(Module.ImportReference(typeof(Exception))));

            Processor.Emit(OpCodes.Stloc, exceptionVariable);

            Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            Processor.Emit(OpCodes.Ldloc, exceptionVariable);
            Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.Exception)).GetSetMethod()));
        }

        /// <summary>
        /// 戻り値を戻します。
        /// </summary>
        public void Return()
        {
            if (TargetMethod.HasReturnValue())
            {
                Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
                Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetGetMethod()));
                if (TargetMethod.ReturnType.IsValueType)
                {
                    Processor.Emit(OpCodes.Unbox_Any, TargetMethod.ReturnType);
                }

                Processor.Emit(OpCodes.Ret);
            }
            else
            {
                Processor.Emit(OpCodes.Ret);
            }
        }

        #endregion
    }
}
