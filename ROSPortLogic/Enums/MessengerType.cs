using System;

namespace MiddlewareLogic.Enums
{
    public enum MessengerType : Byte
    {
        None = 0xFF,
        Raw = 0,
        Topic = 1,
        Service = 2,
        Action = 3
    }
}
