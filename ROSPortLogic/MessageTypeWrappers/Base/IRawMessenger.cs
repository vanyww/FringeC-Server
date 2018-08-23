using System;
using System.Collections.Generic;

namespace MiddlewareLogic.MessageTypeWrappers.Base
{
    public interface IRawMessenger
    {
        void ReceiveMessage(IList<Byte[]> message);
    }
}
