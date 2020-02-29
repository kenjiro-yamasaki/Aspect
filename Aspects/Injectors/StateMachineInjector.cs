using Mono.Cecil;
using Mono.Cecil.Cil;
using SoftCube.Asserts;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// ステートマシンへの注入。
    /// </summary>
    public abstract class StateMachineInjector
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

        #region ステートマシン

        /// <summary>
        /// ステートマシンの属性。
        /// </summary>
        public abstract CustomAttribute StateMachineAttribute { get; }

        /// <summary>
        /// ステートマシンの型。
        /// </summary>
        public TypeDefinition StateMachineType { get; }

        #endregion

        #region フィールド

        /// <summary>
        /// State フィールド。
        /// </summary>
        public FieldDefinition StateField { get; }

        /// <summary>
        /// Aspect フィールド。
        /// </summary>
        public FieldDefinition AspectField { get; }

        /// <summary>
        /// AspectArgs フィールド。
        /// </summary>
        public FieldDefinition AspectArgsField { get; }

        /// <summary>
        /// Args フィールド。
        /// </summary>
        public FieldDefinition ArgumentsField { get; }

        /// <summary>
        /// ResumeFlag フィールド。
        /// </summary>
        public FieldDefinition ResumeFlagField { get; }

        #endregion

        #region メソッド

        /// <summary>
        /// Constructor メソッド。
        /// </summary>
        public MethodDefinition Constructor { get; }

        /// <summary>
        /// MoveNext メソッド。
        /// </summary>
        public MethodDefinition MoveNextMethod { get; }

        /// <summary>
        /// MoveNext メソッドの内容を移動したメソッド。
        /// </summary>
        public MethodDefinition OriginalMoveNextMethod { get; private set; }

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspect">アスペクト。</param>
        public StateMachineInjector(MethodDefinition targetMethod, CustomAttribute aspect)
        {
            Aspect           = aspect ?? throw new ArgumentNullException(nameof(aspect));
            AspectType       = Aspect.AttributeType.Resolve();
            TargetMethod     = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            Module           = TargetMethod.Module;
            StateMachineType = (TypeDefinition)StateMachineAttribute.ConstructorArguments[0].Value;

            StateField       = StateMachineType.Fields.Single(f => f.Name == "<>1__state");
            AspectField      = CreateField("*aspect*",     FieldAttributes.Private, Module.ImportReference(Aspect.AttributeType));
            AspectArgsField  = CreateField("*aspectArgs*", FieldAttributes.Private, Module.ImportReference(typeof(MethodExecutionArgs)));
            ArgumentsField   = CreateField("*arguments*",  FieldAttributes.Private, Module.ImportReference(ArgumentsType));
            ResumeFlagField  = CreateField("*resumeFlag*", FieldAttributes.Private, Module.TypeSystem.Boolean);

            Constructor      = StateMachineType.Methods.Single(m => m.Name == ".ctor");
            MoveNextMethod   = StateMachineType.Methods.Single(m => m.Name == "MoveNext");
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 新たなメソッドを生成し、MoveNext メソッドの内容を移動します。
        /// </summary>
        public void ReplaceMoveNext()
        {
            Assert.Null(OriginalMoveNextMethod);

            var moveNextMethod = MoveNextMethod;

            OriginalMoveNextMethod = new MethodDefinition(moveNextMethod.Name + "<Original>", moveNextMethod.Attributes, moveNextMethod.ReturnType);
            foreach (var parameter in moveNextMethod.Parameters)
            {
                OriginalMoveNextMethod.Parameters.Add(parameter);
            }

            OriginalMoveNextMethod.Body = moveNextMethod.Body;

            foreach (var sequencePoint in moveNextMethod.DebugInformation.SequencePoints)
            {
                OriginalMoveNextMethod.DebugInformation.SequencePoints.Add(sequencePoint);
            }

            StateMachineType.Methods.Add(OriginalMoveNextMethod);
        }

        /// <summary>
        /// Aspect フィールドのインスタンスを生成します。
        /// </summary>
        /// <param name="stateMachineType">ステートマシンの型。</param>
        /// <param name="aspect">アスペクト。</param>
        /// <param name="aspectField">アスペクトのフィールド。</param>
        public void CreateAspectInstance()
        {
            var processor    = Constructor.Body.GetILProcessor();
            var instructions = Constructor.Body.Instructions;
            var first        = instructions.First();

            /// アスペクトのインスタンスを生成し、ローカル変数にストアします。
            var aspectVariable = processor.InsertBefore(first, Aspect);

            /// アスペクトのインスタンスをフィールドにストアします。
            processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, aspectVariable));
            processor.InsertBefore(first, processor.Create(OpCodes.Stfld, AspectField));
        }

        /// <summary>
        /// AspectArgs フィールドのインスタンスを生成します。
        /// </summary>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        public void CreateAspectArgsInstance(ILProcessor processor)
        {
            CreateAspectArgsInstance(processor, null);
        }

        /// <summary>
        /// AspectArgs フィールドのインスタンスを生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        public void CreateAspectArgsInstance(ILProcessor processor, Instruction insert)
        {
            processor.InsertBefore(insert, OpCodes.Ldarg_0);

            if (TargetMethod.IsStatic)
            {
                processor.InsertBefore(insert, OpCodes.Ldnull);
            }
            else
            {
                processor.InsertBefore(insert, OpCodes.Ldarg_0);
                processor.InsertBefore(insert, OpCodes.Ldfld, StateMachineType.Fields.Single(f => f.Name == "<>4__this"));
                if (TargetMethod.DeclaringType.IsValueType)
                {
                    processor.InsertBefore(insert, OpCodes.Box, TargetMethod.DeclaringType);
                }
            }

            processor.InsertBefore(insert, OpCodes.Ldarg_0);
            {
                var parameters = TargetMethod.Parameters;
                var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType()).ToArray();

                if (parameters.Count <= 8)
                {
                    for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                    {
                        var parameter = parameters[parameterIndex];
                        processor.InsertBefore(insert, OpCodes.Ldarg_0);
                        processor.InsertBefore(insert, OpCodes.Ldfld, StateMachineType.Fields.Single(f => f.Name == parameter.Name));
                    }
                    processor.InsertBefore(insert, OpCodes.Newobj, Module.ImportReference(ArgumentsType.GetConstructor(parameterTypes)));
                }
                else
                {
                    processor.InsertBefore(insert, OpCodes.Ldc_I4, parameters.Count);
                    processor.InsertBefore(insert, OpCodes.Newarr, Module.ImportReference(typeof(object)));
                    for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                    {
                        var parameter = parameters[parameterIndex];
                        processor.InsertBefore(insert, OpCodes.Dup);
                        processor.InsertBefore(insert, OpCodes.Ldc_I4, parameterIndex);
                        processor.InsertBefore(insert, OpCodes.Ldarg_0);
                        processor.InsertBefore(insert, OpCodes.Ldfld, StateMachineType.Fields.Single(f => f.Name == parameter.Name));
                        if (parameter.ParameterType.IsValueType)
                        {
                            processor.InsertBefore(insert, OpCodes.Box, parameter.ParameterType);
                        }
                        processor.InsertBefore(insert, OpCodes.Stelem_Ref);
                    }
                    processor.InsertBefore(insert, OpCodes.Newobj, Module.ImportReference(ArgumentsType.GetConstructor(new Type[] { typeof(object[]) })));
                }
            }
            processor.InsertBefore(insert, OpCodes.Stfld, ArgumentsField);

            processor.InsertBefore(insert, OpCodes.Ldarg_0);
            processor.InsertBefore(insert, OpCodes.Ldfld, ArgumentsField);
            processor.InsertBefore(insert, OpCodes.Newobj, Module.ImportReference(typeof(MethodExecutionArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) })));
            processor.InsertBefore(insert, OpCodes.Stfld, AspectArgsField);
        }

        /// <summary>
        /// 引数フィールドを設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetArgumentFields(ILProcessor processor)
        {
            SetArgumentFields(processor, null);
        }

        /// <summary>
        /// 引数フィールドを設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        public void SetArgumentFields(ILProcessor processor, Instruction insert)
        {
            var parameters     = TargetMethod.Parameters;
            var parameterTypes = parameters.Select(p => p.ParameterType.ToSystemType()).ToArray();

            if (parameters.Count <= 8)
            {
                var propertyNames = new string[] { "Arg0", "Arg1", "Arg2", "Arg3", "Arg4", "Arg5", "Arg6", "Arg7" };
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter = parameters[parameterIndex];

                    processor.InsertBefore(insert, OpCodes.Ldarg_0);
                    processor.InsertBefore(insert, OpCodes.Dup);
                    processor.InsertBefore(insert, OpCodes.Ldfld, ArgumentsField);

                    processor.InsertBefore(insert, OpCodes.Ldfld, Module.ImportReference(ArgumentsType.GetField(propertyNames[parameterIndex])));
                    processor.InsertBefore(insert, OpCodes.Stfld, StateMachineType.Fields.Single(f => f.Name == parameter.Name));
                }
            }
            else
            {
                for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    var parameter = parameters[parameterIndex];
                    var parameterType = parameter.ParameterType;

                    processor.InsertBefore(insert, OpCodes.Ldarg_0);
                    processor.InsertBefore(insert, OpCodes.Dup);
                    processor.InsertBefore(insert, OpCodes.Ldfld, ArgumentsField);
                    processor.InsertBefore(insert, OpCodes.Ldc_I4, parameterIndex);
                    processor.InsertBefore(insert, OpCodes.Callvirt, Module.ImportReference(ArgumentsType.GetMethod(nameof(ArgumentsArray.GetArgument))));
                    if (parameterType.IsValueType)
                    {
                        processor.InsertBefore(insert, OpCodes.Unbox_Any, parameterType);
                    }
                    processor.InsertBefore(insert, OpCodes.Stfld, StateMachineType.Fields.Single(f => f.Name == parameter.Name));
                }
            }
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
            processor.InsertBefore(insert, OpCodes.Ldarg_0);
            processor.InsertBefore(insert, OpCodes.Ldfld, AspectField);
            processor.InsertBefore(insert, OpCodes.Ldarg_0);
            processor.InsertBefore(insert, OpCodes.Ldfld, AspectArgsField);

            var aspectType = AspectType;
            while (true)
            {
                var eventHandler = aspectType.Methods.SingleOrDefault(m => m.Name == eventHandlerName);
                if (eventHandler != null)
                {
                    processor.InsertBefore(insert, OpCodes.Callvirt, Module.ImportReference(eventHandler));
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
        /// <see cref="MethodArgs.Exception"/> に例外を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="exceptionVariable">例外のローカル変数。</param>
        public void SetException(ILProcessor processor)
        {
            SetException(processor, null);
        }

        /// <summary>
        /// <see cref="MethodArgs.Exception"/> に例外を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        /// <param name="exceptionVariable">例外のローカル変数。</param>
        public void SetException(ILProcessor processor, Instruction insert)
        {
            var variables = processor.Body.Variables;
            int exceptionVariable = variables.Count;
            variables.Add(new VariableDefinition(Module.ImportReference(typeof(Exception))));

            processor.InsertBefore(insert, OpCodes.Stloc, exceptionVariable);

            processor.InsertBefore(insert, OpCodes.Ldarg_0);
            processor.InsertBefore(insert, OpCodes.Ldfld, AspectArgsField);
            processor.InsertBefore(insert, OpCodes.Ldloc, exceptionVariable);
            processor.InsertBefore(insert, OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.Exception)).GetSetMethod()));
        }

        /// <summary>
        /// フィールドを生成します。
        /// </summary>
        /// <param name="fieldName">フィールドの名前。</param>
        /// <param name="fieldAttributes">フィールドの属性。</param>
        /// <param name="fieldType">フィールドの型。</param>
        /// <returns>フィールド。</returns>
        protected FieldDefinition CreateField(string fieldName, FieldAttributes fieldAttributes, TypeReference fieldType)
        {
            var field = new FieldDefinition(fieldName, fieldAttributes, fieldType);
            StateMachineType.Fields.Add(field);
            return field;
        }

        #endregion
    }
}
