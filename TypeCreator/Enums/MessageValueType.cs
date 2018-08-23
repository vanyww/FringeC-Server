using System;

namespace TypeCreator.Enums
{
    public enum MessageValueType : Byte
    {
        None = 0,
        Boolean = 1,
        SByte = 2,
        Byte = 3,
        Int16 = 4,
        UInt16 = 5,
        Int32 = 6,
        UInt32 = 7,
        Int64 = 8,
        UInt64 = 9,
        Single = 10,
        Double = 11,
        String = 12,

        Raw = 0xFF
    }
}
