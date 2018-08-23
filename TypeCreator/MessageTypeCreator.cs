using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TypeCreator.Base;
using TypeCreator.Converters;

namespace TypeCreator
{
    public sealed class MessageTypeCreator
    {
        private const String DYNAMIC_ASSEMBLY_NAME = "DynamicAssembly",
                             DYNAMIC_MODULE_NAME = "DynamicModule",
                             SET_PROPERTIES_LIST_PARAM_NAME = "valuesList",
                             SET_PROPERTIES_ARRAY_PARAM_NAME = "valuesArray",
                             GETTER_INDEXER_NAME = "get_Item";

        private MessageTypeCreator()
        {
            m_createdTypes = new Dictionary<String, Type>();
            m_dynamicAssembly =
                AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(DYNAMIC_ASSEMBLY_NAME),
                    AssemblyBuilderAccess.Run);

            m_dynamicModule =
                m_dynamicAssembly.DefineDynamicModule(DYNAMIC_MODULE_NAME, false);
        }

        public Type CreateMessageType(String name, IEnumerable<MessagePartDescription> parts)
        {
            //not the best policy
            if (m_createdTypes.TryGetValue(name, out Type result))
                return result;

            Int32 partsNumber = parts.Count();
            var overrideMethodAttributes = MethodAttributes.Virtual |
                                               MethodAttributes.HideBySig |
                                               MethodAttributes.Public |
                                               MethodAttributes.NewSlot |
                                               MethodAttributes.Final;

            var messageType = m_dynamicModule.DefineType(
                name,
                TypeAttributes.Public |
                    TypeAttributes.Sealed,
                typeof(Message));

            var getBytesMethod =
                messageType.DefineMethod(
                    nameof(Message.GetPropertiesBytes),
                    overrideMethodAttributes,
                    typeof(IList<Byte[]>),
                    Type.EmptyTypes);

            var getObjectsMethod =
                messageType.DefineMethod(
                    nameof(Message.GetPropertiesObjects),
                    overrideMethodAttributes,
                    typeof(Object[]),
                    Type.EmptyTypes);

            var setPropertiesMethod_obj =
                messageType.DefineMethod(
                    nameof(Message.SetProperties),
                    overrideMethodAttributes,
                    typeof(void),
                    new[] { typeof(Object[]) });

            var setPropertiesMethod_byte =
                messageType.DefineMethod(
                    nameof(Message.SetProperties),
                    overrideMethodAttributes,
                    typeof(void),
                    new[] { typeof(IList<Byte[]>) });

            var propsNamesProp = messageType.DefineProperty(
                    nameof(Message.PropertiesNames),
                    PropertyAttributes.None,
                    typeof(String[]),
                    Type.EmptyTypes);

            var propsNamesGetter = messageType.DefineMethod(
                    $"get_{nameof(Message.PropertiesNames)}",
                    MethodAttributes.Public |
                        MethodAttributes.HideBySig |
                        MethodAttributes.SpecialName |
                        MethodAttributes.Virtual,
                    typeof(String[]),
                    Type.EmptyTypes);

            var propsNumberGetter = messageType.DefineMethod(
                    $"get_{nameof(Message.PropertiesNumber)}",
                    MethodAttributes.Public |
                        MethodAttributes.HideBySig |
                        MethodAttributes.SpecialName |
                        MethodAttributes.Virtual,
                    typeof(Int32),
                    Type.EmptyTypes);

            setPropertiesMethod_byte.DefineParameter(
                0,
                ParameterAttributes.None,
                SET_PROPERTIES_LIST_PARAM_NAME);

            setPropertiesMethod_obj.DefineParameter(
                0,
                ParameterAttributes.None,
                SET_PROPERTIES_ARRAY_PARAM_NAME);

            var ilMethodGen_setProperties_byte = setPropertiesMethod_byte.GetILGenerator();
            var ilMethodGen_getBytes = getBytesMethod.GetILGenerator();
            var ilMethodGen_getObjects = getObjectsMethod.GetILGenerator();
            var ilMethodGen_setProperties_obj = setPropertiesMethod_obj.GetILGenerator();
            var ilMethodGen_propsNamesGetter = propsNamesGetter.GetILGenerator();
            var ilMethodGen_propsNumberGetter = propsNumberGetter.GetILGenerator();

            #region INIT_EMIT

            //init Byte[][] GetPropertiesBytes()
            ilMethodGen_getBytes.Emit(OpCodes.Ldc_I4, partsNumber);
            ilMethodGen_getBytes.Emit(OpCodes.Newarr, typeof(Byte[]));

            //init Object[] GetPropertiesObjects()
            ilMethodGen_getObjects.Emit(OpCodes.Ldc_I4, partsNumber);
            ilMethodGen_getObjects.Emit(OpCodes.Newarr, typeof(Object));

            //init SetProperties(Byte[][] values)
            var labelLengthTest1 = ilMethodGen_setProperties_byte.DefineLabel();

            ilMethodGen_setProperties_byte.Emit(OpCodes.Ldarg_1);
            ilMethodGen_setProperties_byte.Emit(OpCodes.Callvirt, typeof(ICollection<Byte[]>).GetMethod($"get_{nameof(ICollection<Byte[]>.Count)}"));
            //ilMethodGen_setProperties_byte.Emit(OpCodes.Conv_I4);
            ilMethodGen_setProperties_byte.Emit(OpCodes.Ldc_I4, partsNumber);
            ilMethodGen_setProperties_byte.Emit(OpCodes.Beq_S, labelLengthTest1);
            ilMethodGen_setProperties_byte.Emit(OpCodes.Ldstr, "Arguments number error!");
            ilMethodGen_setProperties_byte.Emit(OpCodes.Newobj,
                typeof(ArgumentException).GetConstructor(new[] { typeof(String) }));
            ilMethodGen_setProperties_byte.Emit(OpCodes.Throw);

            ilMethodGen_setProperties_byte.MarkLabel(labelLengthTest1);

            //init SetProperties(Object[] values)
            var labelLengthTest2 = ilMethodGen_setProperties_obj.DefineLabel();

            ilMethodGen_setProperties_obj.Emit(OpCodes.Ldarg_1);
            ilMethodGen_setProperties_obj.Emit(OpCodes.Ldlen);
            ilMethodGen_setProperties_obj.Emit(OpCodes.Conv_I4);
            ilMethodGen_setProperties_obj.Emit(OpCodes.Ldc_I4, partsNumber);
            ilMethodGen_setProperties_obj.Emit(OpCodes.Beq_S, labelLengthTest2);
            ilMethodGen_setProperties_obj.Emit(OpCodes.Ldstr, "Arguments number error!");
            ilMethodGen_setProperties_obj.Emit(OpCodes.Newobj,
                typeof(ArgumentException).GetConstructor(new[] { typeof(String) }));
            ilMethodGen_setProperties_obj.Emit(OpCodes.Throw);

            ilMethodGen_setProperties_obj.MarkLabel(labelLengthTest2);

            //init String[] PropertiesNames { get; }
            ilMethodGen_propsNamesGetter.Emit(OpCodes.Ldc_I4, partsNumber);
            ilMethodGen_propsNamesGetter.Emit(OpCodes.Newarr, typeof(String));

            //init Int32 PropertiesNumber { get; }
            ilMethodGen_propsNumberGetter.Emit(OpCodes.Ldc_I4, partsNumber);

            #endregion INIT_EMIT

            Int32 num = 0;

            foreach (var part in parts)
            {
                ProcessPart(part, num);
                num++;
            }

            #region CLOSE_EMIT

            ilMethodGen_setProperties_byte.Emit(OpCodes.Ret);

            ilMethodGen_setProperties_obj.Emit(OpCodes.Ret);

            ilMethodGen_getBytes.Emit(OpCodes.Castclass, typeof(IList<Byte[]>));
            ilMethodGen_getBytes.Emit(OpCodes.Ret);

            ilMethodGen_getObjects.Emit(OpCodes.Ret);

            ilMethodGen_propsNamesGetter.Emit(OpCodes.Ret);

            ilMethodGen_propsNumberGetter.Emit(OpCodes.Ret);

            #endregion CLOSE_EMIT

            messageType.DefineMethodOverride(
                setPropertiesMethod_byte,
                typeof(Message).
                    GetMethod(nameof(Message.SetProperties), new[] { typeof(IList<Byte[]>) }));

            messageType.DefineMethodOverride(
                setPropertiesMethod_obj,
                typeof(Message).
                    GetMethod(nameof(Message.SetProperties), new[] { typeof(Object[]) }));

            messageType.DefineMethodOverride(
                getBytesMethod,
                typeof(Message).
                    GetMethod(nameof(Message.GetPropertiesBytes)));

            messageType.DefineMethodOverride(
                getObjectsMethod,
                typeof(Message).
                    GetMethod(nameof(Message.GetPropertiesObjects)));

            propsNamesProp.SetGetMethod(propsNamesGetter);

            var res = messageType.CreateType();
            m_createdTypes.Add(name, res);
            return res;

            void ProcessPart(MessagePartDescription part, Int32 index)
            {
                Type type = part.Type.GetValueType();

                var messageProperty = messageType.DefineProperty(
                    part.Name,
                    PropertyAttributes.None,
                    type,
                    Type.EmptyTypes);

                var messagePropertyFld = messageType.DefineField(
                    $"m_{part.Name}",
                    type,
                    FieldAttributes.Private);

                var messagePropertyGetter = messageType.DefineMethod(
                    $"get_{part.Name}",
                    MethodAttributes.Public |
                        MethodAttributes.HideBySig |
                        MethodAttributes.SpecialName,
                    type,
                    Type.EmptyTypes);

                var ilGetterGen = messagePropertyGetter.GetILGenerator();
                ilGetterGen.Emit(OpCodes.Ldarg_0);
                ilGetterGen.Emit(OpCodes.Ldfld, messagePropertyFld);
                ilGetterGen.Emit(OpCodes.Ret);

                var messagePropertySetter = messageType.DefineMethod(
                    $"set_{part.Name}",
                    MethodAttributes.Public |
                        MethodAttributes.HideBySig |
                        MethodAttributes.SpecialName,
                    typeof(void),
                    new[] { type });

                var ilSetterGen = messagePropertySetter.GetILGenerator();
                ilSetterGen.Emit(OpCodes.Ldarg_0);
                ilSetterGen.Emit(OpCodes.Ldarg_1);
                ilSetterGen.Emit(OpCodes.Stfld, messagePropertyFld);
                ilSetterGen.Emit(OpCodes.Ret);

                messageProperty.SetSetMethod(messagePropertySetter);
                messageProperty.SetGetMethod(messagePropertyGetter);

                GenPropertyMethod_GetBytes();
                GenPropertyMethod_GetObjects();
                GenPropertyMethod_SetPropsByte();
                GenPropertyMethod_SetPropsObj();
                PropertiesNames_Getter();

                #region MAIN_EMIT

                void GenPropertyMethod_GetBytes()
                {
                    ilMethodGen_getBytes.Emit(OpCodes.Dup);
                    ilMethodGen_getBytes.Emit(OpCodes.Ldc_I4, index);
                    ilMethodGen_getBytes.Emit(OpCodes.Ldarg_0);
                    ilMethodGen_getBytes.Emit(OpCodes.Call, messagePropertyGetter);
                    ilMethodGen_getBytes.Emit(OpCodes.Call,
                        typeof(ToBytesConverter).
                            GetMethod(nameof(ToBytesConverter.GetBytes), new[] { type }));
                    ilMethodGen_getBytes.Emit(OpCodes.Stelem_Ref);
                }

                void GenPropertyMethod_GetObjects()
                {
                    ilMethodGen_getObjects.Emit(OpCodes.Dup);
                    ilMethodGen_getObjects.Emit(OpCodes.Ldc_I4, index);
                    ilMethodGen_getObjects.Emit(OpCodes.Ldarg_0);
                    ilMethodGen_getObjects.Emit(OpCodes.Call, messagePropertyGetter);
                    if (type.IsValueType)
                        ilMethodGen_getObjects.Emit(OpCodes.Box, type);
                    ilMethodGen_getObjects.Emit(OpCodes.Stelem_Ref);
                }

                void GenPropertyMethod_SetPropsByte()
                {
                    ilMethodGen_setProperties_byte.Emit(OpCodes.Ldarg_0);
                    ilMethodGen_setProperties_byte.Emit(OpCodes.Ldarg_1);
                    ilMethodGen_setProperties_byte.Emit(OpCodes.Ldc_I4, index);
                    ilMethodGen_setProperties_byte.Emit(OpCodes.Callvirt, typeof(IList<Byte[]>).GetMethod(GETTER_INDEXER_NAME));
                    ilMethodGen_setProperties_byte.Emit(OpCodes.Call,
                        part.Type.GetConverterMethod());
                    ilMethodGen_setProperties_byte.Emit(
                        (type.IsValueType) ?
                            OpCodes.Unbox_Any :
                            OpCodes.Castclass,
                        type);
                    ilMethodGen_setProperties_byte.Emit(OpCodes.Call, messagePropertySetter);
                }

                void GenPropertyMethod_SetPropsObj()
                {
                    ilMethodGen_setProperties_obj.Emit(OpCodes.Ldarg_0);
                    ilMethodGen_setProperties_obj.Emit(OpCodes.Ldarg_1);
                    ilMethodGen_setProperties_obj.Emit(OpCodes.Ldc_I4, index);
                    ilMethodGen_setProperties_obj.Emit(OpCodes.Ldelem_Ref);
                    ilMethodGen_setProperties_obj.Emit(
                        (type.IsValueType) ?
                            OpCodes.Unbox_Any :
                            OpCodes.Castclass,
                        type);
                    ilMethodGen_setProperties_obj.Emit(OpCodes.Call, messagePropertySetter);
                }

                void PropertiesNames_Getter()
                {
                    ilMethodGen_propsNamesGetter.Emit(OpCodes.Dup);
                    ilMethodGen_propsNamesGetter.Emit(OpCodes.Ldc_I4, index);
                    ilMethodGen_propsNamesGetter.Emit(OpCodes.Ldstr, part.Name);
                    ilMethodGen_propsNamesGetter.Emit(OpCodes.Stelem_Ref);
                }

                #endregion MAIN_EMIT
           
            }
        }

        public static MessageTypeCreator Instance =>
            m_instance ?? (m_instance = new MessageTypeCreator());

        private static MessageTypeCreator m_instance;

        private AssemblyBuilder m_dynamicAssembly;
        private ModuleBuilder m_dynamicModule;

        private Dictionary<String, Type> m_createdTypes;
    }
}
