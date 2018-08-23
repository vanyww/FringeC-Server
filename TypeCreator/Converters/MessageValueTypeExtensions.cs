using System;
using System.Reflection;
using System.Text;
using TypeCreator.Base;
using TypeCreator.Enums;

namespace TypeCreator.Converters
{
    public static class MessageValueTypeExtensions
    {
        public static class ConvertersInfo
        {
            public static Object GetSByte(Byte[] bytes)
            { return (SByte)bytes[0]; }

            public static Object GetByte(Byte[] bytes)
            { return bytes[0]; }

            public static Object GetBoolean(Byte[] bytes)
            { return BitConverter.ToBoolean(bytes, 0); }

            public static Object GetInt16(Byte[] bytes)
            { return BitConverter.ToInt16(bytes, 0); }

            public static Object GetUInt16(Byte[] bytes)
            { return BitConverter.ToUInt16(bytes, 0); }

            public static Object GetInt32(Byte[] bytes)
            { return BitConverter.ToInt32(bytes, 0); }

            public static Object GetUInt32(Byte[] bytes)
            { return BitConverter.ToUInt32(bytes, 0); }

            public static Object GetInt64(Byte[] bytes)
            { return BitConverter.ToInt64(bytes, 0); }

            public static Object GetUInt64(Byte[] bytes)
            { return BitConverter.ToUInt64(bytes, 0); }

            public static Object GetSingle(Byte[] bytes)
            { return BitConverter.ToSingle(bytes, 0); }

            public static Object GetDouble(Byte[] bytes)
            { return BitConverter.ToDouble(bytes, 0); }

            public static Object GetString(Byte[] bytes)
            { return Encoding.ASCII.GetString(bytes); }

            public static Object GetBytes(Byte[] bytes)
            { return bytes; }

            public static readonly MethodInfo
                GetSByteMethod =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetSByte)),

                GetByteMethod =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetByte)),

                GetBooleanMethod =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetBoolean)),

                GetInt16Method =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetInt16)),

                GetUInt16Method =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetUInt16)),

                GetInt32Method =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetInt32)),

                GetUInt32Method =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetUInt32)),

                GetInt64Method =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetInt64)),

                GetUInt64Method =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetUInt64)),

                GetSingleMethod =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetSingle)),

                GetDoubleMethod =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetDouble)),

                GetStringMethod =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetString)),

                GetBytesMethod =
                typeof(ConvertersInfo).
                    GetMethod(nameof(GetBytes));
        }

        public static Func<Byte[], Object> GetConvertMethod(this MessageValueType me)
        {
            switch (me)
            {
                case MessageValueType.Raw:
                    return ConvertersInfo.GetBytes;

                case MessageValueType.Boolean:
                    return ConvertersInfo.GetBoolean;

                case MessageValueType.SByte:
                    return ConvertersInfo.GetSByte;

                case MessageValueType.Byte:
                    return ConvertersInfo.GetByte;

                case MessageValueType.Int16:
                    return ConvertersInfo.GetInt16;

                case MessageValueType.UInt16:
                    return ConvertersInfo.GetUInt16;

                case MessageValueType.Int32:
                    return ConvertersInfo.GetInt32;

                case MessageValueType.UInt32:
                    return ConvertersInfo.GetUInt32;

                case MessageValueType.Int64:
                    return ConvertersInfo.GetInt64;

                case MessageValueType.UInt64:
                    return ConvertersInfo.GetUInt64;

                case MessageValueType.Single:
                    return ConvertersInfo.GetSingle;

                case MessageValueType.Double:
                    return ConvertersInfo.GetDouble;

                case MessageValueType.String:
                    return ConvertersInfo.GetString;

                default:
                    return null;
            }
        }

        public static Func<Object, Byte[]> GetConvertBackMethod(this MessageValueType me)
        {
            switch (me)
            {
                case MessageValueType.Boolean:
                    return (e) => { var val = Boolean.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.SByte:
                    return (e) => { var val = SByte.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.Byte:
                    return (e) => { var val = Byte.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.Int16:
                    return (e) => { var val = Int16.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.UInt16:
                    return (e) => { var val = UInt16.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.Int32:
                    return (e) => { var val = Int32.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.UInt32:
                    return (e) => { var val = UInt32.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.Int64:
                    return (e) => { var val = Int64.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.UInt64:
                    return (e) => { var val = UInt64.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.Single:
                    return (e) => { var val = Single.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.Double:
                    return (e) => { var val = Double.Parse((String)e); return ToBytesConverter.GetBytes(val); };

                case MessageValueType.String:
                    return (e) => { var val = (String)e; return ToBytesConverter.GetBytes(val); };

                default:
                    return null;
            }
        }

        public static Type GetValueType(this MessageValueType me)
        {
            if (me == MessageValueType.Raw)
                return typeof(Byte[]);
            return Type.GetType($"System.{me.ToString()}");
        }

        public static MethodInfo GetConverterMethod(this MessageValueType me)
        {
            switch (me)
            {
                case MessageValueType.Raw:
                    return ConvertersInfo.GetBytesMethod;

                case MessageValueType.Boolean:
                    return ConvertersInfo.GetBooleanMethod;

                case MessageValueType.SByte:
                    return ConvertersInfo.GetSByteMethod;

                case MessageValueType.Byte:
                    return ConvertersInfo.GetByteMethod;

                case MessageValueType.Int16:
                    return ConvertersInfo.GetInt16Method;

                case MessageValueType.UInt16:
                    return ConvertersInfo.GetUInt16Method;

                case MessageValueType.Int32:
                    return ConvertersInfo.GetInt32Method;

                case MessageValueType.UInt32:
                    return ConvertersInfo.GetUInt32Method;

                case MessageValueType.Int64:
                    return ConvertersInfo.GetInt64Method;

                case MessageValueType.UInt64:
                    return ConvertersInfo.GetUInt64Method;

                case MessageValueType.Single:
                    return ConvertersInfo.GetSingleMethod;

                case MessageValueType.Double:
                    return ConvertersInfo.GetDoubleMethod;

                case MessageValueType.String:
                    return ConvertersInfo.GetStringMethod;

                default:
                    return null;
            }
        }
    }
}
