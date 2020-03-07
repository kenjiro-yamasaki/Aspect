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
    public abstract class StateMachineRewriter
    {
        #region プロパティ

        /// <summary>
        /// アスペクト属性。
        /// </summary>
        public CustomAttribute AspectAttribute { get; }

        /// <summary>
        /// アスペクト引数の型。
        /// </summary>
        public Type AspectArgsType { get; }

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
        /// ステートマシン属性。
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
        /// オリジナル MoveNext メソッド。
        /// </summary>
        /// <remarks>
        /// MoveNext メソッドの元々のコードをコピーしたメソッド。
        /// </remarks>
        /// <seealso cref="CreateOriginalMoveNextMethod"/>
        public MethodDefinition OriginalMoveNextMethod { get; private set; }

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="targetMethod">ターゲットメソッド。</param>
        /// <param name="aspectAttribute">アスペクト属性。</param>
        /// <param name="aspectArgsType">アスペクト引数の型。</param>
        public StateMachineRewriter(MethodDefinition targetMethod, CustomAttribute aspectAttribute, Type aspectArgsType)
        {
            AspectAttribute  = aspectAttribute ?? throw new ArgumentNullException(nameof(aspectAttribute));
            AspectArgsType   = aspectArgsType ?? throw new ArgumentNullException(nameof(aspectArgsType));
            TargetMethod     = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            Module           = TargetMethod.Module;
            StateMachineType = (TypeDefinition)StateMachineAttribute.ConstructorArguments[0].Value;

            StateField       = StateMachineType.Fields.Single(f => f.Name == "<>1__state");
            AspectField      = CreateField("*aspect*",     FieldAttributes.Private, Module.ImportReference(AspectAttribute.AttributeType));
            AspectArgsField  = CreateField("*aspectArgs*", FieldAttributes.Private, Module.ImportReference(AspectArgsType));
            ArgumentsField   = CreateField("*arguments*",  FieldAttributes.Private, Module.ImportReference(ArgumentsType));
            ResumeFlagField  = CreateField("*resumeFlag*", FieldAttributes.Private, Module.TypeSystem.Boolean);

            Constructor      = StateMachineType.Methods.Single(m => m.Name == ".ctor");
            MoveNextMethod   = StateMachineType.Methods.Single(m => m.Name == "MoveNext");
        }

        #endregion

        #region メソッド

        /// <summary>
        /// オリジナル MoveNext メソッド (MoveNext メソッドの元々のコード) を生成します。
        /// </summary>
        /// <seealso cref="OriginalMoveNextMethod"/>
        public void CreateOriginalMoveNextMethod()
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
        /// Aspect 属性のインスタンスを生成し、フォール度にストアします。
        /// </summary>
        public void NewAspectAttribute()
        {
            var processor    = Constructor.Body.GetILProcessor();
            var variables    = Constructor.Body.Variables;
            var instructions = Constructor.Body.Instructions;
            var first        = instructions.First();

            /// アスペクト属性のインスタンスを生成し、ローカル変数にストアします。
            var aspectAttributeVariable = Constructor.Body.Variables.Count();
            var attributeType = AspectAttribute.AttributeType.ToSystemType();
            variables.Add(new VariableDefinition(Module.ImportReference(attributeType)));

            processor.NewAspectAttribute(first, AspectAttribute);
            processor.InsertBefore(first, OpCodes.Stloc, aspectAttributeVariable);

            /// アスペクト属性のインスタンスをフィールドにストアします。
            processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, aspectAttributeVariable));
            processor.InsertBefore(first, processor.Create(OpCodes.Stfld, AspectField));
        }

        /// <summary>
        /// AspectArgs フィールドのインスタンスを生成します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void NewAspectArgs(ILProcessor processor)
        {
            NewAspectArgs(processor, null);
        }

        /// <summary>
        /// AspectArgs フィールドのインスタンスを生成し、フィールドにストアします。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        public void NewAspectArgs(ILProcessor processor, Instruction insert)
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
        /// AspectArgs.Exception に例外を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        public void SetException(ILProcessor processor)
        {
            SetException(processor, null);
        }

        /// <summary>
        /// AspectArgs.Exception" に例外を設定します。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
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
        /// アスペクトハンドラーを呼びだします。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="aspectHandlerName">アスペクトハンドラー名。</param>
        public void InvokeAspectHandler(ILProcessor processor, string aspectHandlerName)
        {
            InvokeAspectHandler(processor, null, aspectHandlerName);
        }

        /// <summary>
        /// アスペクトハンドラーを呼びだします。
        /// </summary>
        /// <param name="processor">IL プロセッサー。</param>
        /// <param name="insert">挿入位置を示す命令 (この命令の前にコードを注入します)。</param>
        /// <param name="aspectHandlerName">アスペクトハンドラー名。</param>
        public void InvokeAspectHandler(ILProcessor processor, Instruction insert, string aspectHandlerName)
        {
            processor.InsertBefore(insert, OpCodes.Ldarg_0);
            processor.InsertBefore(insert, OpCodes.Ldfld, AspectField);
            processor.InsertBefore(insert, OpCodes.Ldarg_0);
            processor.InsertBefore(insert, OpCodes.Ldfld, AspectArgsField);

            var aspectType = AspectAttribute.AttributeType.Resolve();
            while (true)
            {
                var aspectHandler = aspectType.Methods.SingleOrDefault(m => m.Name == aspectHandlerName);
                if (aspectHandler != null)
                {
                    processor.InsertBefore(insert, OpCodes.Callvirt, Module.ImportReference(aspectHandler));
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
        /// フィールドを生成します。
        /// </summary>
        /// <param name="fieldName">フィールドの名前。</param>
        /// <param name="fieldAttributes">フィールドの属性。</param>
        /// <param name="fieldType">フィールドの型。</param>
        /// <returns>フィールド。</returns>
        public FieldDefinition CreateField(string fieldName, FieldAttributes fieldAttributes, TypeReference fieldType)
        {
            var field = new FieldDefinition(fieldName, fieldAttributes, fieldType);
            StateMachineType.Fields.Add(field);
            return field;
        }

        #endregion
    }
}
