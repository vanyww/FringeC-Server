using MiddlewareLogic.Enums;
using System;
using System.Collections.Generic;
using TypeCreator.Base;
using Utils;

namespace MiddlewareLogic.MessageTypeWrappers.Base
{
    public struct MessengerInfo
    {
        public MessengerType Type;
        public DeviceType Device;
        public TransportProtocol Protocol;
        public String Name;
        public String DeviceName;
        public Byte Id;
        public IList<MessagePartDescription>[] MessageDescriptions;
    }
}