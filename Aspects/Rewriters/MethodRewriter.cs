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
        /// メソッドのコードをコピーしたメソッド。
        /// </summary>
        public MethodDefinition OriginalMethod { get; private set; }

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
        /// <param name="onEntry">OnEntory のアドバイス注入処理。</param>
        /// <param name="onSuccess">OnSuccess のアドバイス注入処理。</param>
        /// <param name="onException">OnException のアドバイス注入処理。</param>
        /// <param name="onExit">OnExit のアドバイス注入処理。</param>
        public void RewriteMethod(Action<ILProcessor> onEntry, Action<ILProcessor> onSuccess, Action<ILProcessor> onException, Action<ILProcessor> onExit)
        {
            /// 新たなメソッドを生成し、対象メソッドのコードをコピーします。
            CopyMethod();

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

                /// onEntry();
                {
                    onEntry(processor);
                }

                /// try
                /// {
                ///     aspectArgs.ReturnValue = OriginalMethod(...);
                ///     onSuccess();
                Instruction leave;
                {
                    @catch.TryStart = @finally.TryStart = processor.EmitNop();
                    InvokeOriginalMethod();
                    onSuccess(processor);
                    leave = processor.EmitLeave(OpCodes.Leave);
                }

                /// }
                /// catch (Exception ex)
                /// {
                ///     onException();
                ///     throw;
                {
                    @catch.TryEnd = @catch.HandlerStart = processor.EmitNop();
                    onException(processor);
                    processor.Emit(OpCodes.Rethrow);
                }

                /// }
                /// finally
                /// {
                ///     onExit();
                {
                    @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = processor.EmitNop();
                    onExit(processor);
                    processor.Emit(OpCodes.Endfinally);
                }

                /// }
                /// return (TResult)aspectArgs.ReturnValue;
                {
                    leave.Operand = @finally.HandlerEnd = processor.EmitNop();
                    Return();
                }

                /// IL コードを最適化します。
                method.Optimize();
            }
        }

        /// <summary>
        /// 新たなメソッドを生成し、メソッドのコードをコピーします。
        /// </summary>
        public void CopyMethod()
        {
            Assert.Null(OriginalMethod);

            OriginalMethod = new MethodDefinition(Method.Name + "<Original>", Method.Attributes, Method.ReturnType);
            foreach (var parameter in Method.Parameters)
            {
                OriginalMethod.Parameters.Add(parameter);
            }
            OriginalMethod.Body = Method.Body;

            foreach (var sequencePoint in Method.DebugInformation.SequencePoints)
            {
                OriginalMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }
            DeclaringType.Methods.Add(OriginalMethod);

            Method.Body = new Mono.Cecil.Cil.MethodBody(Method);
        }

        /// <summary>
        /// AspectAttribute を生成し、ローカル変数にストアします。
        /// </summary>
        public void NewAspectAttribute()
        {
            Assert.Equal(AspectAttributeVariable, -1);
            AspectAttributeVariable = Processor.EmitCreateAspectCode(AspectAttribute);
        }

        /// <summary>
        /// Arguments を生成し、ローカル変数にストアします。
        /// </summary>
        public void NewArguments()
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
        public void NewAspectArgs(TypeReference aspectArgsType)
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
        /// MethodArgs.Method にメソッド情報を設定します。
        /// </summary>
        public void SetMethod()
        {
            Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[] { })));
            Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.Method)).GetSetMethod()));
        }

        /// <summary>
        /// Arguments の内容で引数を更新します。
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
        /// 引数の内容で Arguments を更新します。
        /// </summary>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを更新対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを更新します。
        /// <c>false</c> の場合、すべての引数を更新します。
        /// </param>
        public void UpdateAspectArguments(bool pointerOnly)
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
        /// イベントハンドラーを呼びだします。
        /// </summary>
        /// <param name="eventHandlerName">イベントハンドラー名。</param>
        public void InvokeEventHandler(string eventHandlerName)
        {
            Assert.NotEqual(AspectAttributeVariable, -1);
            Assert.NotEqual(AspectArgsVariable, -1);

            Processor.Emit(OpCodes.Ldloc, AspectAttributeVariable);
            Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);

            var aspectType = AspectAttribueType;
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
        /// メソッドのコードをコピーしたメソッドを呼びだします。
        /// </summary>
        public void InvokeOriginalMethod()
        {
            Assert.NotNull(OriginalMethod);

            /// 戻り値がある場合、AspectArgs をスタックにロードします (戻り値を AspectArgs.ReturnValue に設定するための前処理)。
            if (OriginalMethod.HasReturnValue())
            {
                Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
            }

            /// Arguments の内容で引数を更新します。
            UpdateArguments(pointerOnly: false);

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

            /// 引数の内容で Arguments を更新します (ポインタ引数のみ)。
            UpdateAspectArguments(pointerOnly: true);
        }

        /// <summary>
        /// AspectArgs.Exception に例外を設定します。
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
            if (Method.HasReturnValue())
            {
                Processor.Emit(OpCodes.Ldloc, AspectArgsVariable);
                Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.ReturnValue)).GetGetMethod()));
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

        #endregion
    }
}
