using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Reflection;

namespace SoftCube.Aspects
{
    /// <summary>
    /// メソッド境界アスペクト。
    /// </summary>
    public abstract class OnMethodBoundaryAspect : MethodLevelAspect
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        protected OnMethodBoundaryAspect()
        {
        }

        #endregion

        #region メソッド

        #region アドバイスの注入

        /// <summary>
        /// アドバイスを注入します。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        sealed protected override void InjectAdvice(MethodDefinition targetMethod, CustomAttribute aspectAttribute)
        {
            var iteratorStateMachineAttribute = targetMethod.GetIteratorStateMachineAttribute();
            var asyncStateMachineAttribute    = targetMethod.GetAsyncStateMachineAttribute();

            if (iteratorStateMachineAttribute != null)
            {
                RewriteMoveNextMethod(new IteratorStateMachineRewriter(targetMethod, aspectAttribute, typeof(MethodExecutionArgs)));
            }
            else if (asyncStateMachineAttribute != null)
            {
                RewriteMoveNextMethod(new AsyncStateMachineRewriter(targetMethod, aspectAttribute, typeof(MethodExecutionArgs)));
            }
            else
            {
                RewriteTargetMethod(new MethodRewriter(targetMethod, aspectAttribute));
            }
        }

        #region 通常のメソッド

        /// <summary>
        /// ターゲットメソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">ターゲットメソッドの書き換え。</param>
        private void RewriteTargetMethod(MethodRewriter rewriter)
        {
            /// オリジナルターゲットメソッド (ターゲットメソッドの元々のコード) を生成します。
            rewriter.CreateOriginalTargetMethod();

            /// ターゲットメソッドを書き換えます。
            var targetMethod            = rewriter.TargetMethod;
            var originalTargetMethod    = rewriter.OriginalTargetMethod;
            var aspectAttribute         = rewriter.AspectAttribute;
            var aspectAttributeType     = rewriter.AspectAttributeType;

            var aspectAttributeVariable = targetMethod.AddVariable(aspectAttributeType);
            var argumentsVariable       = targetMethod.AddVariable(targetMethod.ArgumentsType());
            var aspectArgsVariable      = targetMethod.AddVariable(typeof(MethodExecutionArgs));
            var exceptionVariable       = targetMethod.AddVariable(typeof(Exception));

            var onEntry = new Action<ILProcessor>(processor =>
            {
                /// var aspectAttribute = new AspectAttribute(...) {...};
                /// var arguments       = new Arguments(...);
                /// var aspectArgs      = new MethodExecutionArgs(this, arguments);
                /// aspectArgs.Method = MethodBase.GetCurrentMethod();
                /// aspectAttribute.OnEntry(aspectArgs);
                /// arg0 = (TArg0)arguments[0];
                /// arg1 = (TArg1)arguments[1];
                /// ...
                processor.NewAspectAttribute(aspectAttribute);
                processor.Store(aspectAttributeVariable);

                processor.NewArguments();
                processor.Store(argumentsVariable);

                if (targetMethod.IsStatic)
                {
                    processor.LoadNull();
                }
                else
                {
                    processor.LoadThis();
                }
                processor.Load(argumentsVariable);
                processor.New<MethodExecutionArgs>(typeof(object), typeof(Arguments));
                processor.Store(aspectArgsVariable);

                processor.Load(aspectArgsVariable);
                processor.CallStatic(typeof(MethodBase), nameof(MethodBase.GetCurrentMethod));
                processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Method));

                processor.Load(aspectAttributeVariable);
                processor.Load(aspectArgsVariable);
                processor.CallVirtual(GetType(), nameof(OnEntry));
                processor.UpdateArguments(argumentsVariable, pointerOnly: false);
            });

            var onInvoke = new Action<ILProcessor>(processor =>
            {
                /// var returnValue = OriginalMethod(arg0, arg1, ...);
                /// aspectArgs.ReturnValue = returnValue;
                /// arguments[0] = arg0;
                /// arguments[1] = arg1;
                /// ...
                /// aspectAttribute.OnSuccess(aspectArgs);
                if (targetMethod.HasReturnValue())
                {
                    processor.Load(aspectArgsVariable);
                    processor.LoadThis();
                    processor.LoadArguments();
                    processor.Call(originalTargetMethod);
                    processor.Box(targetMethod.ReturnType);
                    processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.ReturnValue));
                }
                else
                {
                    processor.LoadThis();
                    processor.LoadArguments();
                    processor.Call(originalTargetMethod);
                }
                processor.UpdateArgumentsProperty(argumentsVariable, pointerOnly: true);

                processor.Load(aspectAttributeVariable);
                processor.Load(aspectArgsVariable);
                processor.CallVirtual(GetType(), nameof(OnSuccess));
            });

            var onException = new Action<ILProcessor>(processor =>
            {
                /// aspectArgs.Exception = ex;
                /// aspectAttribute.OnException(aspectArgs);
                /// rethrow;
                processor.Store(exceptionVariable);
                processor.Load(aspectArgsVariable);
                processor.Load(exceptionVariable);
                processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Exception));

                processor.Load(aspectAttributeVariable);
                processor.Load(aspectArgsVariable);
                processor.CallVirtual(GetType(), nameof(OnException));
                processor.Rethrow();
            });

            var onExit = new Action<ILProcessor>(processor =>
            {
                /// aspectAttribute.OnExit(aspectArgs);
                /// arg0 = (TArg0)arguments[0];
                /// arg1 = (TArg1)arguments[1];
                /// ...
                processor.Load(aspectAttributeVariable);
                processor.Load(aspectArgsVariable);
                processor.CallVirtual(GetType(), nameof(OnExit));
                processor.UpdateArguments(argumentsVariable, pointerOnly: true);
            });

            var onReturn = new Action<ILProcessor>(processor =>
            {
                /// return (TResult)aspectArgs.ReturnValue;
                if (targetMethod.HasReturnValue())
                {
                    processor.Load(aspectArgsVariable);
                    processor.GetProperty(typeof(MethodArgs), nameof(MethodArgs.ReturnValue));
                    processor.Unbox(targetMethod.ReturnType);
                }
                processor.Return();
            });

            rewriter.RewriteMethod(onEntry, onInvoke, onException, onExit, onReturn);
        }

        #endregion

        #region イテレーターメソッド

        /// <summary>
        /// MoveNext メソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">イテレーターステートマシンの書き換え。</param>
        private void RewriteMoveNextMethod(IteratorStateMachineRewriter rewriter)
        {
            var module            = rewriter.Module;
            var aspectAttribute   = rewriter.AspectAttribute;
            var aspectArgsType    = rewriter.AspectArgsType;
            var moveNextMethod    = rewriter.MoveNextMethod;
            var targetMethod      = rewriter.TargetMethod;
            var stateMachineType  = rewriter.StateMachineType;

            var thisField         = rewriter.ThisField;
            var currentField      = rewriter.CurrentField;
            var aspectField       = rewriter.CreateField("*aspect", Mono.Cecil.FieldAttributes.Private, module.ImportReference(aspectAttribute.AttributeType));
            var argumentsField    = rewriter.CreateField("*arguments", Mono.Cecil.FieldAttributes.Private, module.ImportReference(targetMethod.ArgumentsType()));
            var aspectArgsField   = rewriter.CreateField("*aspectArgs", Mono.Cecil.FieldAttributes.Private, module.ImportReference(aspectArgsType));
            var exceptionVariable = moveNextMethod.AddVariable(typeof(Exception));

            var onEntry = new Action<ILProcessor>(processor =>
            {
                /// _arguments  = new Arguments(...);
                /// _aspectArgs = new MethodExecutionArgs(instance, arguments);
                /// _aspect.OnEntry(aspectArgs);
                /// arg0 = _arguments.Arg0;
                /// arg1 = _arguments.Arg1;
                processor.LoadThis();
                processor.NewAspectAttribute(aspectAttribute);
                processor.Store(aspectField);

                processor.LoadThis();
                processor.NewArguments(targetMethod);
                processor.Store(argumentsField);

                processor.LoadThis();
                if (targetMethod.IsStatic)
                {
                    processor.LoadNull();
                }
                else
                {
                    processor.LoadThis();
                    processor.Load(thisField);
                    processor.Box(targetMethod.DeclaringType);
                }
                processor.LoadThis();
                processor.Load(argumentsField);
                processor.New<MethodExecutionArgs>(typeof(object), typeof(Arguments));
                processor.Store(aspectArgsField);

                processor.LoadThis();
                processor.Load(aspectField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(GetType(), nameof(OnEntry));
                processor.UpdateArgumentsFields(argumentsField, targetMethod);
            });

            var onResume = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnResume(aspectArgs);
                /// arg0 = _arguments.Arg0;
                /// arg1 = _arguments.Arg1;
                processor.LoadThis();
                processor.Load(aspectField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(GetType(), nameof(OnResume));
                processor.UpdateArgumentsFields(argumentsField, targetMethod);
            });

            var onYield = new Action<ILProcessor>(processor =>
            {
                /// _aspectArgs.YieldValue = <> 2__current;
                /// _aspect.OnYield(aspectArgs);
                /// <>2__current = (TResult)aspectArgs.YieldValue;
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.LoadThis();
                processor.Load(currentField);
                processor.Box(currentField.FieldType);
                processor.SetProperty(typeof(MethodExecutionArgs), nameof(MethodExecutionArgs.YieldValue));

                processor.LoadThis();
                processor.Load(aspectField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(GetType(), nameof(OnYield));

                processor.LoadThis();
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.GetProperty(typeof(MethodExecutionArgs), nameof(MethodExecutionArgs.YieldValue));
                processor.Unbox(currentField.FieldType);
                processor.Store(currentField);
            });

            var onSuccess = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnSuccess(aspectArgs);
                processor.LoadThis();
                processor.Load(aspectField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(GetType(), nameof(OnSuccess));
            });

            var onException = new Action<ILProcessor>(processor =>
            {
                /// _aspectArgs.Exception = exception;
                /// _aspect.OnException(aspectArgs);
                processor.Store(exceptionVariable);

                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.Load(exceptionVariable);
                processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Exception));

                processor.LoadThis();
                processor.Load(aspectField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(GetType(), nameof(OnException));
            });

            var onExit = new Action<ILProcessor>(processor =>
            {
                /// _aspect.OnExit(aspectArgs);
                processor.LoadThis();
                processor.Load(aspectField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(GetType(), nameof(OnExit));
            });

            rewriter.RewriteMoveNextMethod(onEntry, onResume, onYield, onSuccess, onException, onExit);
        }

        #endregion

        #region 非同期メソッド

        /// <summary>
        /// MoveNext メソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">非同期ステートマシンの書き換え。</param>
        private void RewriteMoveNextMethod(AsyncStateMachineRewriter rewriter)
        {
            var module            = rewriter.Module;
            var aspectAttribute   = rewriter.AspectAttribute;
            var aspectArgsType    = rewriter.AspectArgsType;
            var moveNextMethod    = rewriter.MoveNextMethod;
            var targetMethod      = rewriter.TargetMethod;
            var stateMachineType  = rewriter.StateMachineType;

            var thisField         = rewriter.ThisField;
            var aspectField       = rewriter.CreateField("*aspect", Mono.Cecil.FieldAttributes.Private, module.ImportReference(aspectAttribute.AttributeType));
            var argumentsField    = rewriter.CreateField("*arguments", Mono.Cecil.FieldAttributes.Private, module.ImportReference(targetMethod.ArgumentsType()));
            var aspectArgsField   = rewriter.CreateField("*aspectArgs", Mono.Cecil.FieldAttributes.Private, module.ImportReference(aspectArgsType));
            var exceptionVariable = moveNextMethod.AddVariable(typeof(Exception));

            var onEntry = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// var instance = <> 4__this;
                /// arguments = new Arguments(...);
                /// aspectArgs = new MethodExecutionArgs(instance, arguments);
                /// aspect.OnEntry(aspectArgs);
                /// arg0 = arguments.Arg0;
                /// arg1 = arguments.Arg1;
                /// ...
                processor.LoadThis(insert);
                processor.NewAspectAttribute(insert, aspectAttribute);
                processor.Store(insert, aspectField);

                processor.LoadThis(insert);
                processor.NewArguments(insert, targetMethod);
                processor.Store(insert, argumentsField);

                processor.LoadThis(insert);
                if (targetMethod.IsStatic)
                {
                    processor.LoadNull(insert);
                }
                else
                {
                    processor.LoadThis(insert);
                    processor.Load(insert, thisField);
                    processor.Box(insert, targetMethod.DeclaringType);
                }
                processor.LoadThis(insert);
                processor.Load(insert, argumentsField);
                processor.New<MethodExecutionArgs>(insert, typeof(object), typeof(Arguments));
                processor.Store(insert, aspectArgsField);

                processor.LoadThis(insert);
                processor.Load(insert, aspectField);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.CallVirtual(insert, GetType(), nameof(OnEntry));
                processor.UpdateArgumentsFields(insert, argumentsField, targetMethod);
            });

            var onResume = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnResume(aspectArgs);
                /// arg0 = arguments.Arg0;
                /// arg1 = arguments.Arg1;
                /// ...
                processor.LoadThis(insert);
                processor.Load(insert, aspectField);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.CallVirtual(insert, GetType(), nameof(OnResume));
                processor.UpdateArgumentsFields(insert, argumentsField, targetMethod);
            });

            var onYield = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnYield(aspectArgs);
                processor.LoadThis(insert);
                processor.Load(insert, aspectField);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.CallVirtual(insert, GetType(), nameof(OnYield));
            });

            var onSuccess = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspectArgs.ReturnValue = result;
                /// aspect.OnSuccess(aspectArgs);
                /// result = (TResult)aspectArgs.ReturnValue;
                int resultVariable = 1;
                if (targetMethod.ReturnType is GenericInstanceType genericReturnType)
                {
                    var returnType = genericReturnType.GenericArguments[0];
                    processor.LoadThis(insert);
                    processor.Load(insert, aspectArgsField);
                    processor.Load(insert, resultVariable);
                    processor.Box(insert, returnType);
                    processor.SetProperty(insert, typeof(MethodArgs), nameof(MethodArgs.ReturnValue));

                    processor.LoadThis(insert);
                    processor.Load(insert, aspectField);
                    processor.LoadThis(insert);
                    processor.Load(insert, aspectArgsField);
                    processor.CallVirtual(insert, GetType(), nameof(OnSuccess));

                    processor.LoadThis(insert);
                    processor.Load(insert, aspectArgsField);
                    processor.GetProperty(insert, typeof(MethodArgs), nameof(MethodArgs.ReturnValue));
                    processor.Unbox(insert, returnType);
                    processor.Store(insert, resultVariable);
                }
                else
                {
                    processor.LoadThis(insert);
                    processor.Load(insert, aspectField);
                    processor.LoadThis(insert);
                    processor.Load(insert, aspectArgsField);
                    processor.CallVirtual(insert, GetType(), nameof(OnSuccess));
                }
            });

            var onException = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspectArgs.Exception = exception;
                /// aspect.OnException(aspectArgs);
                processor.Store(insert, exceptionVariable);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.Load(insert, exceptionVariable);
                processor.SetProperty(insert, typeof(MethodArgs), nameof(MethodArgs.Exception));

                processor.LoadThis(insert);
                processor.Load(insert, aspectField);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.CallVirtual(insert, GetType(), nameof(OnException));
            });

            var onExit = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                /// aspect.OnExit(aspectArgs);
                processor.LoadThis(insert);
                processor.Load(insert, aspectField);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.CallVirtual(insert, GetType(), nameof(OnExit));
            });

            rewriter.RewriteMoveNextMethod(onEntry, onResume, onYield, onSuccess, onException, onExit);
        }

        #endregion

        #endregion

        #region イベントハンドラー

        /// <summary>
        /// メッソドが開始されたときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnEntry(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// <c>yield return</c> または <c>await</c> ステートメントの結果として、ステートマシンが結果を出力するときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        /// <remarks>
        /// イテレーターメソッドでは、アドバイスは <c>yield return</c> ステートメントで呼びだされます。
        /// 非同期メソッドでは、<c>await</c> ステートメントの結果としてステートマシンが待機を開始した直後にアドバイスが呼びだされます。
        /// <c>await</c> ステートメントのオペランドが同期的に完了した操作である場合、ステートマシンは結果を出力せず、<see cref="OnYield(MethodExecutionArgs)"/> アドバイスは呼び出されません。
        /// </remarks>
        public virtual void OnYield(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// <c>yield return</c> または <c>await</c> ステートメントの後にステートマシンが実行を再開するときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        /// <remarks>
        /// イテレーターメソッドの場合、このアドバイスは MoveNext() メソッドの前に呼びだされます。
        /// ただし、MoveNext() の最初の呼び出しは <see cref="OnEntry(MethodExecutionArgs)"/> にマップされます。
        /// 非同期メソッドでは、<c>await</c> ステートメントの結果として待機した後、ステートマシンが実行を再開した直後にアドバイスが呼びだされます。
        /// </remarks>
        public virtual void OnResume(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドが正常終了したときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnSuccess(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドが例外終了したときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnException(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// メッソドが終了したときに呼びだされます。
        /// </summary>
        /// <param name="args">メソッド実行引数。</param>
        public virtual void OnExit(MethodExecutionArgs args)
        {
        }

        #endregion

        #endregion
    }
}
