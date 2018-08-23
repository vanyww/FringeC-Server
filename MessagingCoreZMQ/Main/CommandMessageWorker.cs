using MessagingCore.Base;
using MessagingCore.Main;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using ZeroMQ;

namespace MessagingCore
{
    public sealed class CommandMessageWorker : MonitoredSocketUser
    {
        private const String TCPPort = "5558",
                             MonitorAdress = "inproc://monitor_c";

        public CommandMessageWorker() :
            base(MonitorAdress, ZSocketType.ROUTER, MessagingCoreSettings.Default.CommandSocketReceivePollRate)
        {
            RecvBuffer = new TransformBlock<Object, MessageData>(
                   (msg) => MessageCodec.DecodeMessage((ZMessage)msg));
            SendBuffer = new TransformBlock<(Int32 NodeId, IList<Byte[]> Msg), Object>(
                   (msg) => MessageCodec.EncodeCommandMessage(msg.NodeId, msg.Msg));
            m_sendTransform = new TransformBlock<Object, ZMessage>(obj => (ZMessage)obj);

            SendBuffer.LinkTo(m_sendTransform);
            m_sendTransform.LinkTo(m_sendAction);
            m_receiveBuffer.LinkTo(RecvBuffer);
        }

        public void Start()
        {
            var port = MessagingCoreSettings.Default.CommandSocket ?? TCPPort;
            StartWork(port);
        }

        public void Close() =>
            StopWork();

        public TransformBlock<Object, MessageData> RecvBuffer { get; }
        public TransformBlock<(Int32, IList<Byte[]>), Object> SendBuffer { get; }

        private TransformBlock<Object, ZMessage> m_sendTransform;
    }
}
