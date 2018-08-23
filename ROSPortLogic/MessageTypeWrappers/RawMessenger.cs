using MiddlewareLogic.MessageTypeWrappers.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace MiddlewareLogic.MessageTypeWrappers
{
    public sealed class RawMessenger : Messenger, IRawMessenger
    {
        public RawMessenger(MessengerInfo info) : base(info) =>
            m_messageBuffer = new BufferBlock<IList<Byte[]>>();

        public void ReceiveMessage(IList<Byte[]> message) =>
            m_messageBuffer.Post(message);

        public BufferBlock<IList<Byte[]>> MessageBuffer => m_messageBuffer;

        private BufferBlock<IList<Byte[]>> m_messageBuffer;
    }
}
