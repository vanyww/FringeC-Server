using System;
using System.Text;

namespace TypeCreator.Converters
{
    public static class ToBytesConverter
    {
        public static Byte[] GetBytes(Boolean value) =>
            BitConverter.GetBytes(value);

        public static Byte[] GetBytes(Byte value) => 
            new[] { value };

        public static Byte[] GetBytes(SByte value) =>
            BitConverter.GetBytes(value);

        public static Byte[] GetBytes(Int16 value) =>
            BitConverter.GetBytes(value);

        public static Byte[] GetBytes(UInt16 value) =>
            BitConverter.GetBytes(value);

        public static Byte[] GetBytes(Int32 value) =>
            BitConverter.GetBytes(value);

        public static Byte[] GetBytes(UInt32 value) =>
            BitConverter.GetBytes(value);

        public static Byte[] GetBytes(Int64 value) =>
            BitConverter.GetBytes(value);

        public static Byte[] GetBytes(UInt64 value) =>
            BitConverter.GetBytes(value);

        public static Byte[] GetBytes(Single value) =>
            BitConverter.GetBytes(value);

        public static Byte[] GetBytes(Double value) =>
            BitConverter.GetBytes(value);

        public static Byte[] GetBytes(String value) =>
            Encoding.ASCII.GetBytes(value);
    }
}