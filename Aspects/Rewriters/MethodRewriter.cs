using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using SoftCube.Asserts;
using System;
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
        /// ターゲットメソッド。
        /// </summary>
        public MethodDefinition TargetMethod { get; }

        /// <summary>
        /// オリジナルターゲットメソッド。
        /// </summary>
        /// <remarks>
        /// ターゲットメソッドの元々のコードをコピーしたメソッド。
        /// </remarks>
        /// <seealso cref="CreateOriginalTargetMethod"/>
        /// <seealso cref="InvokeOriginalTargetMethod"/>
        public MethodDefinition OriginalTargetMethod { get; private set; }

        /// <summary>
        /// メソッドの宣言型。
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

        #region パラメーター

        /// <summary>
        /// パラメーターコレクション。
        /// </summary>
        protected Collection<ParameterDefinition> Parameters => TargetMethod.Parameters;

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
            TargetMethod          = method ?? throw new ArgumentNullException(nameof(method));
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
        /// <param name="onExit">OnFinally アドバイスの注入処理。</param>
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
        public void RewriteMethod(Action<ILProcessor> onEntry, Action<ILProcessor> onInvoke, Action<ILProcessor> onException, Action<ILProcessor> onExit, Action<ILProcessor> onReturn)
        {
            /// 例外ハンドラーを追加します。
            var handlers = TargetMethod.Body.ExceptionHandlers;
            var @catch   = new ExceptionHandler(ExceptionHandlerType.Catch) { CatchType = Module.ImportReference(typeof(Exception)) };
            var @finally = new ExceptionHandler(ExceptionHandlerType.Finally);
            handlers.Add(@catch);
            handlers.Add(@finally);

            /// ...OnEntry アドバイス...
            {
                onEntry(Processor);
            }

            /// try
            /// {
            ///     ...OnInvoke アドバイス...
            Instruction leave;
            {
                @catch.TryStart = @finally.TryStart = Processor.EmitNop();
                onInvoke(Processor);
                leave = Processor.EmitLeave(OpCodes.Leave);
            }

            /// }
            /// catch (Exception ex)
            /// {
            ///     ...OnException アドバイス...
            /// }
            {
                @catch.TryEnd = @catch.HandlerStart = Processor.EmitNop();

                //ExceptionVariable = Variables.Count;
                //Variables.Add(new VariableDefinition(Module.ImportReference(typeof(Exception))));
                //Processor.Emit(OpCodes.Stloc, ExceptionVariable);

                onException(Processor);
            }

            /// finally
            /// {
            ///     ...OnFinally アドバイス...
            /// }
            {
                @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = Processor.EmitNop();
                onExit(Processor);
                Processor.Emit(OpCodes.Endfinally);
            }

            /// ...OnReturn アドバイス...
            {
                leave.Operand = @finally.HandlerEnd = Processor.EmitNop();
                onReturn(Processor);
            }

            /// IL コードを最適化します。
            TargetMethod.Optimize();
        }

        /// <summary>
        /// オリジナルターゲットメソッド (ターゲットメソッドの元々のコード) を生成します。
        /// </summary>
        /// <seealso cref="OriginalTargetMethod"/>
        /// <seealso cref="InvokeOriginalTargetMethod"/>
        public void CreateOriginalTargetMethod()
        {
            Assert.Null(OriginalTargetMethod);

            OriginalTargetMethod = new MethodDefinition(TargetMethod.Name + "<Original>", TargetMethod.Attributes, TargetMethod.ReturnType);
            foreach (var parameter in TargetMethod.Parameters)
            {
                OriginalTargetMethod.Parameters.Add(parameter);
            }
            OriginalTargetMethod.Body = TargetMethod.Body;

            foreach (var sequencePoint in TargetMethod.DebugInformation.SequencePoints)
            {
                OriginalTargetMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }
            DeclaringType.Methods.Add(OriginalTargetMethod);

            TargetMethod.Body = new Mono.Cecil.Cil.MethodBody(TargetMethod);
        }

        #region アドバイスの注入

        /// <summary>
        /// 引数を更新します。
        /// </summary>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを更新対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを更新します。
        /// <c>false</c> の場合、すべての引数を更新します。
        /// </param>
        public void UpdateArguments(bool pointerOnly, int argumentsVariable)
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
                        Processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        Processor.Emit(OpCodes.Ldfld, Module.ImportReference(TargetMethod.ArgumentsType().GetField(fieldNames[parameterIndex])));
                        Processor.EmitStind(elementType);
                    }
                    else if (!pointerOnly)
                    {
                        Processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        Processor.Emit(OpCodes.Ldfld, Module.ImportReference(TargetMethod.ArgumentsType().GetField(fieldNames[parameterIndex])));
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

                        Processor.Emit(OpCodes.Ldloc, argumentsVariable);
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
                        Processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        Processor.Emit(OpCodes.Ldc_I4, parameterIndex);
                        Processor.Emit(OpCodes.Call, Module.ImportReference(typeof(Arguments).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()));
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
        /// AspectArgs.Arguments を更新します。
        /// </summary>
        /// <param name="pointerOnly">
        /// ポインタ引数のみを更新対象とするか。
        /// <c>true</c> の場合、in/ref/out 引数のみを更新します。
        /// <c>false</c> の場合、すべての引数を更新します。
        /// </param>
        public void UpdateArgumentsProperty(bool pointerOnly, int argumentsVariable)
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

                        Processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        if (TargetMethod.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        Processor.EmitLdind(elementType);
                        Processor.Emit(OpCodes.Stfld, Module.ImportReference(TargetMethod.ArgumentsType().GetField(fieldNames[parameterIndex])));
                    }
                    else if (!pointerOnly)
                    {
                        Processor.Emit(OpCodes.Ldloc, argumentsVariable);
                        if (TargetMethod.IsStatic)
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex);
                        }
                        else
                        {
                            Processor.Emit(OpCodes.Ldarg, parameterIndex + 1);
                        }
                        Processor.Emit(OpCodes.Stfld, Module.ImportReference(TargetMethod.ArgumentsType().GetField(fieldNames[parameterIndex])));
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

                        Processor.Emit(OpCodes.Ldloc, argumentsVariable);
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
                        Processor.Emit(OpCodes.Callvirt, Module.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.SetArgument))));
                    }
                    else
                    {
                        Processor.Emit(OpCodes.Ldloc, argumentsVariable);
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
                        Processor.Emit(OpCodes.Callvirt, Module.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.SetArgument))));
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
