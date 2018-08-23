using System;

namespace MessagingCore.Enums
{
    public enum MessageType : Byte
    {
        Common = 0,
        NodeSignal = 1,
        Command = 2,
        NodeInitialization = 3
    }
}
