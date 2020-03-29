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
        sealed public override void InjectAdvice()
        {
            using var profile = Profiling.Profiler.Start($"{nameof(OnMethodBoundaryAspect)}.{nameof(InjectAdvice)}");

            //
            var iteratorStateMachineAttribute = TargetMethod.GetIteratorStateMachineAttribute();
            var asyncStateMachineAttribute    = TargetMethod.GetAsyncStateMachineAttribute();
            if (iteratorStateMachineAttribute != null)
            {
                RewriteMoveNextMethod(new IteratorStateMachineRewriter(TargetMethod, CustomAttribute, typeof(MethodExecutionArgs)));
            }
            else if (asyncStateMachineAttribute != null)
            {
                RewriteMoveNextMethod(new AsyncStateMachineRewriter(TargetMethod, CustomAttribute, typeof(MethodExecutionArgs)));
            }
            else
            {
                RewriteTargetMethod();
            }
        }

        #region 通常のメソッド

        /// <summary>
        /// ターゲットメソッドを書き換えます。
        /// </summary>
        private void RewriteTargetMethod()
        {
            // ターゲットメソッドを複製します。
            var clonedTargetMethod = TargetMethod.Clone();

            // ターゲットメソッドを書き換えます。
            int aspectVariable     = default;
            int argumentsVariable  = default;
            int aspectArgsVariable = default;
            int exceptionVariable  = default;

            var processor = TargetMethod.Body.GetILProcessor();
            var onEntry = new Action(() =>
            {
                // var aspectAttribute = new AspectAttribute(...) {...};
                // var arguments       = new Arguments(...);
                // var aspectArgs      = new MethodExecutionArgs(this, arguments);
                // aspectArgs.Method = MethodBase.GetCurrentMethod();
                // aspectAttribute.OnEntry(aspectArgs);
                // arg0 = (TArg0)arguments[0];
                // arg1 = (TArg1)arguments[1];
                // ...
                aspectVariable     = TargetMethod.AddVariable(CustomAttributeType);
                argumentsVariable  = TargetMethod.AddVariable(typeof(Arguments));
                aspectArgsVariable = TargetMethod.AddVariable(typeof(MethodExecutionArgs));
                exceptionVariable  = TargetMethod.AddVariable(typeof(Exception));

                processor.NewAspectAttribute(CustomAttribute);
                processor.Store(aspectVariable);

                processor.NewArguments();
                processor.Store(argumentsVariable);

                if (TargetMethod.IsStatic)
                {
                    processor.LoadNull();
                }
                else
                {
                    processor.LoadThis();
                }
                processor.Load(argumentsVariable);
                processor.New(typeof(MethodExecutionArgs), typeof(object), typeof(Arguments));
                processor.Store(aspectArgsVariable);

                processor.Load(aspectArgsVariable);
                processor.CallStatic(typeof(MethodBase), nameof(MethodBase.GetCurrentMethod));
                processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Method));

                processor.Load(aspectVariable);
                processor.Load(aspectArgsVariable);
                processor.CallVirtual(CustomAttributeType, nameof(OnEntry));
                processor.UpdateArguments(argumentsVariable, pointerOnly: false);
            });

            var onInvoke = new Action(() =>
            {
                // var returnValue = OriginalMethod(arg0, arg1, ...);
                // aspectArgs.ReturnValue = returnValue;
                // arguments[0] = arg0;
                // arguments[1] = arg1;
                // ...
                // aspectAttribute.OnSuccess(aspectArgs);
                if (TargetMethod.HasReturnValue())
                {
                    processor.Load(aspectArgsVariable);
                    processor.Append(clonedTargetMethod);
                    processor.Box(TargetMethod.ReturnType);
                    processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.ReturnValue));
                }
                else
                {
                    processor.Append(clonedTargetMethod);
                }
                processor.UpdateArgumentsProperty(argumentsVariable, pointerOnly: true);

                processor.Load(aspectVariable);
                processor.Load(aspectArgsVariable);
                processor.CallVirtual(CustomAttributeType, nameof(OnSuccess));
            });

            var onException = new Action(() =>
            {
                // aspectArgs.Exception = ex;
                // aspectAttribute.OnException(aspectArgs);
                // rethrow;
                processor.Store(exceptionVariable);
                processor.Load(aspectArgsVariable);
                processor.Load(exceptionVariable);
                processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Exception));

                processor.Load(aspectVariable);
                processor.Load(aspectArgsVariable);
                processor.CallVirtual(CustomAttributeType, nameof(OnException));
                processor.Rethrow();
            });

            var onExit = new Action(() =>
            {
                // aspectAttribute.OnExit(aspectArgs);
                // arg0 = (TArg0)arguments[0];
                // arg1 = (TArg1)arguments[1];
                // ...
                processor.Load(aspectVariable);
                processor.Load(aspectArgsVariable);
                processor.CallVirtual(CustomAttributeType, nameof(OnExit));
                processor.UpdateArguments(argumentsVariable, pointerOnly: true);
            });

            var onReturn = new Action(() =>
            {
                // return (TResult)aspectArgs.ReturnValue;
                if (TargetMethod.HasReturnValue())
                {
                    processor.Load(aspectArgsVariable);
                    processor.GetProperty(typeof(MethodArgs), nameof(MethodArgs.ReturnValue));
                    processor.Unbox(TargetMethod.ReturnType);
                }
                processor.Return();
            });

            processor.RewriteMethod(onEntry, onInvoke, onException, onExit, onReturn);
        }

        #endregion

        #region イテレーターメソッド

        /// <summary>
        /// MoveNext メソッドを書き換えます。
        /// </summary>
        /// <param name="rewriter">イテレーターステートマシンの書き換え。</param>
        private void RewriteMoveNextMethod(IteratorStateMachineRewriter rewriter)
        {
            var module               = rewriter.Module;
            var customAttribute      = rewriter.CustomAttribute;
            var aspectAttributeType  = rewriter.AspectAttributeType;
            var aspectArgsType       = rewriter.AspectArgsType;
            var moveNextMethod       = rewriter.MoveNextMethod;
            var targetMethod         = rewriter.TargetMethod;
            var stateMachineType     = rewriter.StateMachineType;

            var thisField            = rewriter.ThisField;
            var methodField          = rewriter.MethodField;
            var currentField         = rewriter.CurrentField;
            var aspectAttributeField = rewriter.CreateField("*aspect", Mono.Cecil.FieldAttributes.Private, module.ImportReference(customAttribute.AttributeType));
            var argumentsField       = rewriter.CreateField("*arguments", Mono.Cecil.FieldAttributes.Private, module.ImportReference(typeof(Arguments)));
            var aspectArgsField      = rewriter.CreateField("*aspectArgs", Mono.Cecil.FieldAttributes.Private, module.ImportReference(aspectArgsType));
            int exceptionVariable    = -1;

            var onEntry = new Action<ILProcessor>(processor =>
            {
                // _aspectAttribute = new AspectAttribute(...) {...};
                // _arguments       = new Arguments(...);
                // _aspectArgs      = new MethodExecutionArgs(<>4__this, _arguments);
                // _aspectAttribute.OnEntry(_aspectArgs);
                // arg0 = _arguments.Arg0;
                // arg1 = _arguments.Arg1;
                // ...
                processor.LoadThis();
                processor.NewAspectAttribute(customAttribute);
                processor.Store(aspectAttributeField);

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
                processor.New(typeof(MethodExecutionArgs), typeof(object), typeof(Arguments));
                processor.Store(aspectArgsField);

                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.LoadThis();
                processor.Load(methodField);
                processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Method));

                processor.LoadThis();
                processor.Load(aspectAttributeField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(aspectAttributeType, nameof(OnEntry));
                processor.UpdateArguments(argumentsField, targetMethod);
            });

            var onResume = new Action<ILProcessor>(processor =>
            {
                // _aspectAttribute.OnResume(_aspectArgs);
                // arg0 = _arguments.Arg0;
                // arg1 = _arguments.Arg1;
                // ...
                processor.LoadThis();
                processor.Load(aspectAttributeField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(aspectAttributeType, nameof(OnResume));
                processor.UpdateArguments(argumentsField, targetMethod);
            });

            var onYield = new Action<ILProcessor>(processor =>
            {
                // _aspectArgs.YieldValue = <> 2__current;
                // _aspectAttribute.OnYield(_aspectArgs);
                // <>2__current = (TResult)_aspectArgs.YieldValue;
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.LoadThis();
                processor.Load(currentField);
                processor.Box(currentField.FieldType);
                processor.SetProperty(typeof(MethodExecutionArgs), nameof(MethodExecutionArgs.YieldValue));

                processor.LoadThis();
                processor.Load(aspectAttributeField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(aspectAttributeType, nameof(OnYield));

                processor.LoadThis();
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.GetProperty(typeof(MethodExecutionArgs), nameof(MethodExecutionArgs.YieldValue));
                processor.Unbox(currentField.FieldType);
                processor.Store(currentField);
            });

            var onSuccess = new Action<ILProcessor>(processor =>
            {
                // _aspectAttribute.OnSuccess(_aspectArgs);
                processor.LoadThis();
                processor.Load(aspectAttributeField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(aspectAttributeType, nameof(OnSuccess));
            });

            var onException = new Action<ILProcessor>(processor =>
            {
                // _aspectArgs.Exception = exception;
                // _aspectAttribute.OnException(_aspectArgs);
                exceptionVariable = moveNextMethod.AddVariable(typeof(Exception));
                processor.Store(exceptionVariable);

                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.Load(exceptionVariable);
                processor.SetProperty(typeof(MethodArgs), nameof(MethodArgs.Exception));

                processor.LoadThis();
                processor.Load(aspectAttributeField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(aspectAttributeType, nameof(OnException));
            });

            var onExit = new Action<ILProcessor>(processor =>
            {
                // _aspectAttribute.OnExit(_aspectArgs);
                processor.LoadThis();
                processor.Load(aspectAttributeField);
                processor.LoadThis();
                processor.Load(aspectArgsField);
                processor.CallVirtual(aspectAttributeType, nameof(OnExit));
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
            var module               = rewriter.Module;
            var customAttribute      = rewriter.CustomAttribute;
            var aspectAttributeType  = rewriter.AspectAttributeType;
            var aspectArgsType       = rewriter.AspectArgsType;
            var moveNextMethod       = rewriter.MoveNextMethod;
            var targetMethod         = rewriter.TargetMethod;
            var stateMachineType     = rewriter.StateMachineType;

            var thisField            = rewriter.ThisField;
            var aspectAttributeField = rewriter.CreateField("*aspect", Mono.Cecil.FieldAttributes.Private, module.ImportReference(customAttribute.AttributeType));
            var argumentsField       = rewriter.CreateField("*arguments", Mono.Cecil.FieldAttributes.Private, module.ImportReference(typeof(Arguments)));
            var aspectArgsField      = rewriter.CreateField("*aspectArgs", Mono.Cecil.FieldAttributes.Private, module.ImportReference(aspectArgsType));
            var exceptionVariable    = moveNextMethod.AddVariable(typeof(Exception));

            var onEntry = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                // _aspectAttribute = new AspectAttribute(...) {...};
                // _arguments       = new Arguments(...);
                // _aspectArgs      = new MethodExecutionArgs(4__this, _arguments);
                // _aspectAttribute.OnEntry(_aspectArgs);
                // arg0 = _arguments.Arg0;
                // arg1 = _arguments.Arg1;
                // ...
                processor.LoadThis(insert);
                processor.NewAspectAttribute(insert, customAttribute);
                processor.Store(insert, aspectAttributeField);

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
                processor.Load(insert, aspectAttributeField);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.CallVirtual(insert, aspectAttributeType, nameof(OnEntry));
                processor.UpdateArguments(insert, argumentsField, targetMethod);
            });

            var onResume = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                // _aspectAttribute.OnResume(_aspectArgs);
                // arg0 = _arguments.Arg0;
                // arg1 = _arguments.Arg1;
                // ...
                processor.LoadThis(insert);
                processor.Load(insert, aspectAttributeField);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.CallVirtual(insert, aspectAttributeType, nameof(OnResume));
                processor.UpdateArguments(insert, argumentsField, targetMethod);
            });

            var onYield = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                // _aspectAttribute.OnYield(_aspectArgs);
                processor.LoadThis(insert);
                processor.Load(insert, aspectAttributeField);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.CallVirtual(insert, aspectAttributeType, nameof(OnYield));
            });

            var onSuccess = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                // _aspectArgs.ReturnValue = result;
                // _aspectAttribute.OnSuccess(_aspectArgs);
                // result = (TResult)_aspectArgs.ReturnValue;
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
                    processor.Load(insert, aspectAttributeField);
                    processor.LoadThis(insert);
                    processor.Load(insert, aspectArgsField);
                    processor.CallVirtual(insert, aspectAttributeType, nameof(OnSuccess));

                    processor.LoadThis(insert);
                    processor.Load(insert, aspectArgsField);
                    processor.GetProperty(insert, typeof(MethodArgs), nameof(MethodArgs.ReturnValue));
                    processor.Unbox(insert, returnType);
                    processor.Store(insert, resultVariable);
                }
                else
                {
                    processor.LoadThis(insert);
                    processor.Load(insert, aspectAttributeField);
                    processor.LoadThis(insert);
                    processor.Load(insert, aspectArgsField);
                    processor.CallVirtual(insert, aspectAttributeType, nameof(OnSuccess));
                }
            });

            var onException = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                // _aspectArgs.Exception = exception;
                // _aspectAttribute.OnException(_aspectArgs);
                processor.Store(insert, exceptionVariable);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.Load(insert, exceptionVariable);
                processor.SetProperty(insert, typeof(MethodArgs), nameof(MethodArgs.Exception));

                processor.LoadThis(insert);
                processor.Load(insert, aspectAttributeField);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.CallVirtual(insert, aspectAttributeType, nameof(OnException));
            });

            var onExit = new Action<ILProcessor, Instruction>((processor, insert) =>
            {
                // _aspectAttribute.OnExit(_aspectArgs);
                processor.LoadThis(insert);
                processor.Load(insert, aspectAttributeField);
                processor.LoadThis(insert);
                processor.Load(insert, aspectArgsField);
                processor.CallVirtual(insert, aspectAttributeType, nameof(OnExit));
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
