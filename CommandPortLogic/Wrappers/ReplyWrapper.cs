using System;
using System.Threading.Tasks.Dataflow;
using TypeCreator.Base;

namespace CommandPortLogic.Wrappers
{
    public sealed class ReplyWrapper<M> : CommandWrapper
        where M : Message, new()
    {
        public override void ReceiveReply(Byte[][] message)
        {
            var msg = new M();
            msg.SetProperties(message);
            ReplyReceived?.Invoke(this, new MessageGotEventArgs<Message> { Message = msg });
        }

        public override void SendCommand(Object[] message) =>
            throw new NotImplementedException();

        public override BufferBlock<Object[]> SendBuffer => throw new NotImplementedException();
        public override BufferBlock<Byte[][]> ReceiveBuffer => throw new NotImplementedException();

        private BufferBlock<Object[]> m_sendBuffer;
        private BufferBlock<Byte[][]> m_receiveBuffer;
    }
}
