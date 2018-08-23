using MessagingCore.Enums;
using System;
using System.Collections.Generic;
using Utils;

namespace MessagingCore.Main
{
    public struct MessageData
    {
        public Int32 Identity;
        public TransportProtocol Protocol;
        public IList<Byte[]> Data;
        public MessageType Type;
    }
}
