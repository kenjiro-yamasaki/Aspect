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
    /// メソッドの書き換え。
    /// </summary>
    public class MethodRewriter
    {
        #region プロパティ

        /// <summary>
        /// アスペクト属性。
        /// </summary>
        public CustomAttribute AspectAttribute { get; }

        /// <summary>
        /// アスペクト属性の型。
        /// </summary>
        public TypeDefinition AspectAttribueType => AspectAttribute.AttributeType.Resolve();

        /// <summary>
        /// メソッド。
        /// </summary>
        public MethodDefinition Method { get; }

        /// <summary>
        /// オリジナルコードメソッド。
        /// </summary>
        /// <remarks>
        /// メソッドの元々のコードをコピーしたメソッド。
        /// </remarks>
        /// <seealso cref="CreateOriginalCodeMethod"/>
        /// <seealso cref="InvokeOriginalCodeMethod"/>
        public MethodDefinition OriginalCodeMethod { get; private set; }

        /// <summary>
        /// メソッドの宣言型。
        /// </summary>
        public TypeDefinition DeclaringType => Method.DeclaringType;

        /// <summary>
        /// モジュール。
        /// </summary>
        public ModuleDefinition Module => Method.Module;

        /// <summary>
        /// IL プロセッサー。
        /// </summary>
        public ILProcessor Processor => Method.Body.GetILProcessor();

        #region パラメーター

        /// <summary>
        /// パラメーターコレクション。
        /// </summary>
        protected Collection<ParameterDefinition> Parameters => Method.Parameters;

        /// <summary>
        /// Arguments の型。
        /// </summary>
        protected Type ArgumentsType
        {
            get
            {
                var parameters = Method.Parameters;
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
        protected Collection<VariableDefinition> Variables => Method.Body.Variables;

        /// <summary>
        /// AspectAttribute のローカル変数。
        /// </summary>
        protected int AspectAttributeVariable { get; set; } = -1;

        /// <summary>
        /// Arguments のローカル変数。
        /// </summary>
        protected int ArgumentsVariable { get; set; } = -1;

        /// <summary>
        /// AspectArgs のローカル変数。
        /// </summary>
        protected int AspectArgsVariable { get; set; } = -1;

        /// <summary>
        /// 戻り値のローカル変数。
        /// </summary>
        protected int ReturnValueVariable { get; set; } = -1;

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="method">メソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        public MethodRewriter(MethodDefinition method, CustomAttribute aspectAttribute)
        {
            Method          = method ?? throw new ArgumentNullException(nameof(method));
            AspectAttribute = aspectAttribute ?? throw new ArgumentNullException(nameof(aspectAttribute));
        }

        #endregion

        #region メソッド

        /// <summary>
        /// メソッドを書き換えます。
        /// </summary>
        /// <param name="onEntry">OnEntory アドバイスの注入処理。</param>
        /// <param name="onInvoke">OnInvoke アドバイスの注入処理。</param>
        /// <param name="onException">OnException アドバイスの注入処理。</param>
        /// <param name="onFinally">OnFinally アドバイスの注入処理。</param>
        /// <remarks>
        /// メソッドを以下のように書き換えます。
        /// <code>
        /// ...OnEntry アドバイス...
        /// try
        /// {
        ///     ...OnInvoke アドバイス...
        /// }
        /// catch (Exception ex)
        /// {
        ///     ...OnException アドバイス...
        /// }
        /// finally
        /// {
        ///     ...OnFinally アドバイス...
        /// }
        /// ...OnReturn アドバイス...
        /// </code>
        /// </remarks>
        public void RewriteMethod(Action<ILProcessor> onEntry, Action<ILProcessor> onInvoke, Action<ILProcessor> onException, Action<ILProcessor> onFinally, Action<ILProcessor> onReturn)
        {
            /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
            CreateOriginalCodeMethod();

            /// 対象メソッドのコードを書き換えます。
            {
                var method    = Method;
                var module    = Module;
                var processor = Processor;

                /// 例外ハンドラーを追加します。
                var handlers = method.Body.ExceptionHandlers;
                var @catch   = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = module.ImportReference(typeof(Exception)) };
                var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
                handlers.Add(@catch);
                handlers.Add(@finally);

                /// ...OnEntry アドバイス...
                {
                    onEntry(processor);
                }

                /// try
                /// {
                ///     ...OnInvoke アドバイス...
                Instruction leave;
                {
                    @catch.TryStart = @finally.TryStart = processor.EmitNop();
                    onInvoke(processor);
                    leave = processor.EmitLeave(OpCodes.Leave);
                }

                /// }
                /// catch (Exception ex)
                /// {
                ///     ...OnException アドバイス...
                /// }
                {
                    @catch.TryEnd = @catch.HandlerStart = processor.EmitNop();
                    onException(processor);
                }

                /// finally
                /// {
                ///     ...OnFinally アドバイス...
                /// }
                {
                    @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = processor.EmitNop();
                    onFinally(processor);
                    processor.Emit(OpCodes.Endfinally);
                }

                /// ...OnReturn アドバイス...
                {
                    leave.Operand = @finally.HandlerEnd = processor.EmitNop();
                    onReturn(processor);
                }

                /// IL コードを最適化します。
                method.Optimize();
            }
        }

        /// <summary>
        /// オリジナルコードメソッドを生成します。
        /// </summary>
        /// <seealso cref="OriginalCodeMethod"/>
        /// <seealso cref="InvokeOriginalCodeMethod"/>
        public void CreateOriginalCodeMethod()
        {
            Assert.Null(OriginalCodeMethod);

            OriginalCodeMethod = new MethodDefinition(Method.Name + "<Original>", Method.Attributes, Method.ReturnType);
            foreach (var parameter in Method.Parameters)
            {
                OriginalCodeMethod.Parameters.Add(parameter);
            }
            OriginalCodeMethod.Body = Method.Body;

            foreach (var sequencePoint in Method.DebugInformation.SequencePoints)
            {
                OriginalCodeMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }
            DeclaringType.Methods.Add(OriginalCodeMethod);

            Method.Body = new Mono.Cecil.Cil.MethodBody(Method);
        }

        #region アドバイスの注入

        /// <summary>
        /// AspectAttribute を生成し、ローカル変数にストアします。
        /// </summary>
        public void NewAspectAttributeVariable()
        {
            Assert.Equal(AspectAttributeVariable, -1);
            AspectAttributeVariable = Processor.EmitCreateAspectCode(AspectAttribute);
        }

        /// <summary>
        /// Arguments を生成し、ローカル変数にストアします。
        /// </summary>
        public void NewArgumentsVariable()
        {
            Assert.Equal(ArgumentsVariable, -1);

            /// ローカル変数を追加します。
            ArgumentsVariable = Variables.Count();
            Variables.Add(new VariableDefinition(Module.ImportReference(ArgumentsType)));

            /// Arguments を生成して、ローカル変数にストアします。
            if (Parameters.Count <= 8)
            {
                for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
                {
                    var parameter     = Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    if (Method.IsStatic)
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
                Processor.Emit(OpCodes.Newobj, Module.ImportReference(ArgumentsType.GetConstructor(Parameters.Select(p => p.ParameterType.ToSystemType(removePointer: true)).ToArray())));
                Processor.Emit(OpCodes.Stloc, ArgumentsVariable);
            }
            else
            {
                Processor.Emit(OpCodes.Ldc_I4, Parameters.Count);
                Processor.Emit(OpCodes.Newarr, Module.ImportReference(typeof(object)));
                for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
                {
                    var parameter     = Parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    Processor.Emit(OpCodes.Dup);
                    Processor.Emit(OpCodes.Ldc_I4, parameterIndex);

                    if (Method.IsStatic)
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

                Processor.Emit(OpCodes.Newobj, Module.ImportReference(ArgumentsType.GetConstructor(new Type[] { typeof(object[]) })));
                Processor.Emit(OpCodes.Stloc, ArgumentsVariable);
            }
        }

        /// <summary>
        /// AspectArgs を生成し、ローカル変数にストアします。
        /// </summary>
        /// <param name="aspectArgsType">AspectArgs の型。</param>
        public void NewAspectArgsVariable(TypeReference aspectArgsType)
        {
            Assert.Equal(AspectArgsVariable, -1);
            Assert.NotEqual(ArgumentsVariable, -1);

            /// ローカル変数を追加します。
            AspectArgsVariable = Variables.Count();
            Variables.Add(new VariableDefinition(aspectArgsType));

            /// AspectArgs を生成し、ローカル変数にストアします。
            if (Method.IsStatic)
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
        /// AspectArgs.Method にメソッド情報を設定します。
        /// </summary>
        public void UpdateMethodProperty()
        {
            Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { })));
            Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.Method)).GetSetMethod()));
        }

        /// <summary>
        /// 引数を更新します。
        /// </summary>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを更新対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを更新します。
        /// <c>false</c> の場合、すべての引数を更新します。
        /// </param>
        public void UpdateArguments(bool pointerOnly)
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

                        if (Method.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        Processor.Emit(OpCodes.Ldfld, Module.ImportReference(ArgumentsType.GetField(fieldNames[parameterIndex])));
                        Processor.EmitStind(elementType);
                    }
                    else if (!pointerOnly)
                    {
                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        Processor.Emit(OpCodes.Ldfld, Module.ImportReference(ArgumentsType.GetField(fieldNames[parameterIndex])));
                        if (Method.IsStatic)
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

                        if (Method.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }

                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        Processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
                        if (elementType.IsValueType)
                        {
                            Processor.Emit(OpCodes.Unbox, elementType);
                            Processor.Emit(OpCodes.Ldobj, elementType);
                        }
                        Processor.EmitStind(elementType);
                    }
                    else if (!pointerOnly)
                    {
                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        Processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
                        if (parameterType.IsValueType)
                        {
                            Processor.Emit(OpCodes.Unbox_Any, parameterType);
                        }
                        if (Method.IsStatic)
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
        /// AspectArgs.Arguments を更新します。
        /// </summary>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを更新対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを更新します。
        /// <c>false</c> の場合、すべての引数を更新します。
        /// </param>
        public void UpdateArgumentsProperty(bool pointerOnly)
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
                        if (Method.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        Processor.EmitLdind(elementType);
                        Processor.Emit(OpCodes.Stfld, Module.ImportReference(ArgumentsType.GetField(fieldNames[parameterIndex])));
                    }
                    else if (!pointerOnly)
                    {
                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        if (Method.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        Processor.Emit(OpCodes.Stfld, Module.ImportReference(ArgumentsType.GetField(fieldNames[parameterIndex])));
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

                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        Processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        if (Method.IsStatic)
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
                        Processor.Emit(OpCodes.Callvirt, Module.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.SetArgument))));
                    }
                    else
                    {
                        Processor.Emit(OpCodes.Ldloc, ArgumentsVariable);
                        Processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        if (Method.IsStatic)
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
                        Processor.Emit(OpCodes.Callvirt, Module.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.SetArgument))));
                    }
                }
            }
        }

        /// <summary>
        /// AspectArgs.ReturnValue を更新します。
        /// </summary>
        public void UpdateExceptionProperty()
        {
            int exceptionVariable = Variables.Count;
            Variables.Add(new VariableDefinition(Module.ImportReference(typeof(Exception))));

            Processor.Emit(OpCodes.Stloc, exceptionVariable);

            Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            Processor.Emit(OpCodes.Ldloc, exceptionVariable);
            Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.Exception)).GetSetMethod()));
        }

        /// <summary>
        /// 戻り値を更新します。
        /// </summary>
        public void UpdateReturnValueVariable()
        {
            if (Method.HasReturnValue())
            {
                Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
                Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.ReturnValue)).GetGetMethod()));
                if (Method.ReturnType.IsValueType)
                {
                    Processor.Emit(OpCodes.Unbox_Any, Method.ReturnType);
                }
                Processor.Emit(OpCodes.Stloc, ReturnValueVariable);
            }
        }

        /// <summary>
        /// AspectArgs.ReturnValue を更新します。
        /// </summary>
        public void UpdateReturnValueProperty()
        {
            if (Method.HasReturnValue())
            {
                Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
                Processor.Emit(OpCodes.Ldloc, ReturnValueVariable);
                if (Method.ReturnType.IsValueType)
                {
                    Processor.Emit(OpCodes.Box, Method.ReturnType);
                }
                Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.ReturnValue)).GetSetMethod()));
            }
        }

        /// <summary>
        /// アスペクトハンドラーを呼びだします。
        /// </summary>
        /// <param name="aspectHandlerName">アスペクトハンドラー名。</param>
        public void InvokeAspectHandler(string aspectHandlerName)
        {
            Assert.NotEqual(AspectAttributeVariable, -1);
            Assert.NotEqual(AspectArgsVariable, -1);

            Processor.Emit(OpCodes.Ldloc, AspectAttributeVariable);
            Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);

            var aspectAttributeType = AspectAttribueType;
            while (true)
            {
                var handler = aspectAttributeType.Methods.SingleOrDefault(m => m.Name == aspectHandlerName);
                if (handler != null)
                {
                    Processor.Emit(OpCodes.Callvirt, Module.ImportReference(handler));
                    break;
                }
                else
                {
                    Assert.NotNull(aspectAttributeType.BaseType);
                    aspectAttributeType = aspectAttributeType.BaseType.Resolve();
                }
            }
        }

        /// <summary>
        /// オリジナルコードメソッドを呼びだします。
        /// </summary>
        /// <seealso cref="OriginalCodeMethod"/>
        /// <seealso cref="CreateOriginalCodeMethod"/>
        public void InvokeOriginalCodeMethod()
        {
            Assert.NotNull(OriginalCodeMethod);

            /// 戻り値がある場合、AspectArgs をスタックにロードします (戻り値を AspectArgs.ReturnValue に設定するための前処理)。
            if (OriginalCodeMethod.HasReturnValue())
            {
                Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            }

            /// 引数をスタックにロードします。
            /// ターゲットメソッドの内容を移動したメソッドを呼びだします。
            if (!Method.IsStatic)
            {
                Processor.Emit(OpCodes.Ldarg_0);
            }
            for (int parameterIndex = 0; parameterIndex < Parameters.Count; parameterIndex++)
            {
                if (Method.IsStatic)
                {
                    Processor.Emit(OpCodes.Ldarg, parameterIndex);
                }
                else
                {
                    Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                }
            }
            Processor.Emit(OpCodes.Call, OriginalCodeMethod);

            /// 戻り値をローカル変数にストアします。
            if (Method.HasReturnValue())
            {
                ReturnValueVariable = Variables.Count;
                Variables.Add(new VariableDefinition(Module.ImportReference(Method.ReturnType)));

                Processor.Emit(OpCodes.Stloc, ReturnValueVariable);
            }
        }

        /// <summary>
        /// AspectArgs.ReturnValue を戻します。
        /// </summary>
        public void ReturnProperty()
        {
            if (Method.HasReturnValue())
            {
                Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
                Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.ReturnValue)).GetGetMethod()));
                if (Method.ReturnType.IsValueType)
                {
                    Processor.Emit(OpCodes.Unbox_Any, Method.ReturnType);
                }

                Processor.Emit(OpCodes.Ret);
            }
            else
            {
                Processor.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// 戻り値を戻します。
        /// </summary>
        public void ReturnVariable()
        {
            if (Method.HasReturnValue())
            {
                Processor.Emit(OpCodes.Ldloc, ReturnValueVariable);
                Processor.Emit(OpCodes.Ret);
            }
            else
            {
                Processor.Emit(OpCodes.Ret);
            }
        }

        #endregion

        #endregion
    }
}
