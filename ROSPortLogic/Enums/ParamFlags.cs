using System;

namespace MiddlewareLogic.Enums
{
    [Flags]
    public enum ParamFlags : Byte
    {
        Normal = 0x00,
        ReadOnly = 0x01,
        Reserved1 = 0x02,
        Reserved2 = 0x04,
        Reserved3 = 0x08,
        Reserved4 = 0x10,
        Reserved5 = 0x20,
        Reserved6 = 0x40,
        Reserved7 = 0x80
    }
}
