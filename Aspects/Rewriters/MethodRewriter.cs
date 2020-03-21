using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Asserts;
using System;

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
        public TypeDefinition AspectAttributeType => AspectAttribute.AttributeType.Resolve();

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
        public MethodDefinition OriginalTargetMethod { get; private set; }

        /// <summary>
        /// モジュール。
        /// </summary>
        public ModuleDefinition Module => TargetMethod.Module;

        /// <summary>
        /// IL プロセッサー。
        /// </summary>
        public ILProcessor Processor => TargetMethod.Body.GetILProcessor();

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        public MethodRewriter(MethodDefinition targetMethod, CustomAttribute aspectAttribute)
        {
            TargetMethod    = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
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
            onEntry(Processor);

            /// try
            /// {
            ///     ...OnInvoke アドバイス...
            @catch.TryStart = @finally.TryStart = Processor.EmitNop();
            onInvoke(Processor);
            var leave = Processor.EmitLeave(OpCodes.Leave);

            /// }
            /// catch (Exception ex)
            /// {
            ///     ...OnException アドバイス...
            /// }
            @catch.TryEnd = @catch.HandlerStart = Processor.EmitNop();
            onException(Processor);

            /// finally
            /// {
            ///     ...OnFinally アドバイス...
            /// }
            @catch.HandlerEnd = @finally.TryEnd = @finally.HandlerStart = Processor.EmitNop();
            onExit(Processor);
            Processor.Emit(OpCodes.Endfinally);

            /// ...OnReturn アドバイス...
            leave.Operand = @finally.HandlerEnd = Processor.EmitNop();
            onReturn(Processor);

            /// IL コードを最適化します。
            TargetMethod.Optimize();
        }

        /// <summary>
        /// オリジナルターゲットメソッド (ターゲットメソッドの元々のコード) を生成します。
        /// </summary>
        /// <seealso cref="OriginalTargetMethod"/>
        public void CreateOriginalTargetMethod()
        {
            Assert.Null(OriginalTargetMethod);

            // オリジナルターゲットメソッドを生成します。
            {
                var methodAttribute =  TargetMethod.Attributes & ~(MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

                OriginalTargetMethod = new MethodDefinition("*" + TargetMethod.Name, methodAttribute, TargetMethod.ReturnType);
                OriginalTargetMethod.Body = TargetMethod.Body;
                foreach (var parameter in TargetMethod.Parameters)
                {
                    OriginalTargetMethod.Parameters.Add(parameter);
                }
                foreach (var sequencePoint in TargetMethod.DebugInformation.SequencePoints)
                {
                    OriginalTargetMethod.DebugInformation.SequencePoints.Add(sequencePoint);
                }
                TargetMethod.DeclaringType.Methods.Add(OriginalTargetMethod);
            }

            // ターゲットメソッドの Body を新規作成します。
            TargetMethod.Body = new MethodBody(TargetMethod);
        }

        #endregion
    }
}
