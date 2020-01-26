using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace SoftCube.Aspects
{
    /// <summary>
    /// ステートマシン。
    /// </summary>
    public abstract class StateMachine
    {
        #region プロパティ

        /// <summary>
        /// モジュール。
        /// </summary>
        public ModuleDefinition Module { get; }

        /// <summary>
        /// ターゲットメソッド。
        /// </summary>
        public MethodDefinition TargetMethod { get; }

        #region アスペクト

        /// <summary>
        /// アスペクト。
        /// </summary>
        public CustomAttribute Aspect { get; }

        /// <summary>
        /// アスペクトの型。
        /// </summary>
        public TypeDefinition AspectType => Aspect.AttributeType.Resolve();

        #endregion

        #region 属性

        /// <summary>
        /// ステートマシンの属性。
        /// </summary>
        public abstract CustomAttribute StateMachineAttribute { get; }

        /// <summary>
        /// ステートマシンの型。
        /// </summary>
        public TypeDefinition StateMachineType => (TypeDefinition)StateMachineAttribute.ConstructorArguments[0].Value;

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
        public FieldDefinition ArgsField { get; }

        /// <summary>
        /// ResumeFlag フィールド。
        /// </summary>
        public FieldDefinition ResumeFlagField { get; }

        #endregion

        #region メソッド

        /// <summary>
        /// MoveNext メソッド。
        /// </summary>
        public MethodDefinition MoveNextMethod => StateMachineType.Methods.Single(m => m.Name == "MoveNext");

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="aspect">アスペクト。</param>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        public StateMachine(CustomAttribute aspect, MethodDefinition targetMethod)
        {
            Aspect       = aspect ?? throw new ArgumentNullException(nameof(aspect));
            TargetMethod = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            Module       = StateMachineType.Module;

            StateField      = StateMachineType.Fields.Single(f => f.Name == "<>1__state");
            AspectField     = CreateField("*aspect*",     FieldAttributes.Private, Module.ImportReference(Aspect.AttributeType));
            AspectArgsField = CreateField("*aspectArgs*", FieldAttributes.Private, Module.ImportReference(typeof(MethodExecutionArgs)));
            ArgsField       = CreateField("*args*",       FieldAttributes.Private, Module.ImportReference(typeof(Arguments)));
            ResumeFlagField = CreateField("*resumeFlag*", FieldAttributes.Private, Module.TypeSystem.Boolean);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// Aspect フィールドのインスタンスを生成します。
        /// </summary>
        /// <param name="stateMachineType">ステートマシンの型。</param>
        /// <param name="aspect">アスペクト。</param>
        /// <param name="aspectField">アスペクトのフィールド。</param>
        public void CreateAspectInstance()
        {
            var constructor  = StateMachineType.Methods.Single(m => m.Name == ".ctor");
            var processor    = constructor.Body.GetILProcessor();
            var instructions = constructor.Body.Instructions;
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
            var module = TargetMethod.Module;

            processor.Emit(insert, OpCodes.Ldarg_0);
            processor.Emit(insert, OpCodes.Ldarg_0);
            processor.Emit(insert, OpCodes.Ldfld, StateMachineType.Fields.Single(f => f.Name == "<>4__this"));
            if (TargetMethod.DeclaringType.IsValueType)
            {
                processor.Emit(insert, OpCodes.Box, TargetMethod.DeclaringType);
            }
            processor.Emit(insert, OpCodes.Ldarg_0);
            {
                var parameters = TargetMethod.Parameters;
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
                        processor.Emit(insert, OpCodes.Ldarg_0);
                        processor.Emit(insert, OpCodes.Ldfld, StateMachineType.Fields.Single(f => f.Name == parameter.Name));
                    }
                    processor.Emit(insert, OpCodes.Newobj, module.ImportReference(argumentsType.GetConstructor(parameterTypes)));
                }
                else
                {
                    processor.Emit(insert, OpCodes.Ldc_I4, parameters.Count);
                    processor.Emit(insert, OpCodes.Newarr, module.ImportReference(typeof(object)));
                    for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                    {
                        var parameter = parameters[parameterIndex];
                        processor.Emit(insert, OpCodes.Dup);
                        processor.Emit(insert, OpCodes.Ldc_I4, parameterIndex);
                        processor.Emit(insert, OpCodes.Ldarg_0);
                        processor.Emit(insert, OpCodes.Ldfld, StateMachineType.Fields.Single(f => f.Name == parameter.Name));
                        if (parameter.ParameterType.IsValueType)
                        {
                            processor.Emit(insert, OpCodes.Box, parameter.ParameterType);
                        }
                        processor.Emit(insert, OpCodes.Stelem_Ref);
                    }
                    processor.Emit(insert, OpCodes.Newobj, module.ImportReference(argumentsType.GetConstructor(new Type[] { typeof(object[]) })));
                }
            }
            processor.Emit(insert, OpCodes.Stfld, ArgsField);

            processor.Emit(insert, OpCodes.Ldarg_0);
            processor.Emit(insert, OpCodes.Ldfld, ArgsField);
            processor.Emit(insert, OpCodes.Newobj, module.ImportReference(typeof(MethodExecutionArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) })));
            processor.Emit(insert, OpCodes.Stfld, AspectArgsField);
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
            processor.Emit(insert, OpCodes.Ldarg_0);
            processor.Emit(insert, OpCodes.Ldfld, AspectField);
            processor.Emit(insert, OpCodes.Ldarg_0);
            processor.Emit(insert, OpCodes.Ldfld, AspectArgsField);
            processor.Emit(insert, OpCodes.Callvirt, AspectType.Methods.Single(m => m.Name == eventHandlerName));
        }

        /// <summary>
        /// <see cref="MethodArgs.Exception"/> に例外を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="exceptionVariable">例外のローカル変数。</param>
        public void SetException(ILProcessor processor, int exceptionVariable)
        {
            SetException(processor, null, exceptionVariable);
        }

        /// <summary>
        /// <see cref="MethodArgs.Exception"/> に例外を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        /// <param name="exceptionVariable">例外のローカル変数。</param>
        public void SetException(ILProcessor processor, Instruction insert, int exceptionVariable)
        {
            processor.Emit(insert, OpCodes.Ldarg_0);
            processor.Emit(insert, OpCodes.Ldfld, AspectArgsField);
            processor.Emit(insert, OpCodes.Ldloc, exceptionVariable);
            processor.Emit(insert, OpCodes.Call, Module.ImportReference(typeof(MethodArgs).GetProperty(nameof(MethodArgs.Exception)).GetSetMethod()));
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
