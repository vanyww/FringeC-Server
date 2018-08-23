using System;
using System.Threading.Tasks.Dataflow;

namespace CommandPortLogic.Wrappers
{
    public abstract class CommandWrapper
    {
        public abstract void SendCommand(Object[] message);
        public abstract void ReceiveReply(Byte[][] message);

        public abstract BufferBlock<Object[]> SendBuffer { get; }
        public abstract BufferBlock<Byte[][]> ReceiveBuffer { get; }
    }
}
