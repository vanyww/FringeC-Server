using System;

namespace SystemSignalsLogic.Enums
{
    public enum INSignal : Byte
    {
        MessangerInitialization = 0x80,
        CommandInitialization = 0x81,
        Pong = 0x82,
        Null = 0x83
    }
}
