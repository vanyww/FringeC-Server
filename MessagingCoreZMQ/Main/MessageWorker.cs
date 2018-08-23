using MessagingCore.Base;
using MessagingCore.Enums;
using MessagingCore.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Utils;
using ZeroMQ;

namespace MessagingCore.Main
{
    public sealed class MessageWorker : MonitoredDoubleSocketUser
    {
        private const String Port = "5557",
                             TCPMonitorAdress = "inproc://monitor_tcp_m",
                             UDPMonitorAdress = "inproc://monitor_udp_m";

        public MessageWorker() :
            base(TCPMonitorAdress, UDPMonitorAdress, ZSocketType.ROUTER, MessagingCoreSettings.Default.CoreSocketReceivePollRate)
        {
            m_cmdWorker = new CommandMessageWorker();
            m_processor = new MessageProcessor();

            m_decodeTransform = new TransformBlock<(TransportProtocol Protocol, ZMessage Message), MessageData>
                    ((msg) =>
                    {
                        var decodedMsg = MessageCodec.DecodeMessage(msg.Message);
                        decodedMsg.Protocol = msg.Protocol;
                        msg.Message.Dispose();
                        return decodedMsg;
                    });

            m_decodedCommandMessageProcessingAction = new ActionBlock<MessageData>(msg => m_processor.ProcessCommandMessage(msg));
            m_decodedMessageProcessingAction = new ActionBlock<MessageData>(msg => m_processor.ProcessMessage(msg));

            m_encodeTransform = new TransformBlock<(Node Node, IList<Byte[]> Msg), (TransportProtocol Protocol, ZMessage Msg)>
                    (msg => (TransportProtocol.TCP, MessageCodec.EncodeMessage(msg.Node.Identity, msg.Msg)));
            m_encodeSignalTransform = new TransformBlock<(Node Node, IList<Byte[]> Msg), (TransportProtocol Protocol, ZMessage Msg)>
                    (msg => (TransportProtocol.TCP, MessageCodec.EncodeNodeSignalMessage(msg.Node.Identity, msg.Msg)));

            m_sendAction = new ActionBlock<(TransportProtocol Protocol, ZMessage Msg)>(msg =>
            {
                if (msg.Protocol == TransportProtocol.TCP)
                    m_tcpSocketSendAction.Post(msg.Msg);
                //else
                //    m_udpSocketSendAction.Post(msg.Msg);
            });

            m_receiveBuffer.LinkTo(m_decodeTransform);
            m_decodeTransform.LinkTo(m_decodedMessageProcessingAction);
            m_cmdWorker.RecvBuffer.LinkTo(m_decodedCommandMessageProcessingAction);

            m_processor.SendMessageBuffer.LinkTo(m_encodeTransform);
            m_processor.SendSignalBuffer.LinkTo(m_encodeSignalTransform);
            m_processor.SendCommandBuffer.LinkTo(m_cmdWorker.SendBuffer);

            m_encodeTransform.LinkTo(m_sendAction);
            m_encodeSignalTransform.LinkTo(m_sendAction);
        }

        public void Start()
        {
            var port = MessagingCoreSettings.Default.CoreSocket ?? Port;
            StartWork(port);
            m_cmdWorker.Start();
        }

        public void Close()
        {
            StopWork();
            m_cmdWorker.Close();
        }
        public event EventHandler<NodeRegisteredEventArgs> NodeRegistered
        {
            add => m_processor.NodeRegistered += value;
            remove => m_processor.NodeRegistered -= value;
        }

        public Dictionary<Int32, Node> Nodes => m_processor.Nodes;

        private MessageProcessor m_processor;

        private TransformBlock<(TransportProtocol protocol, ZMessage message), MessageData> m_decodeTransform;
        private TransformBlock<(Node Node, IList<Byte[]> Msg), (TransportProtocol Protocol, ZMessage Msg)> m_encodeTransform;
        private TransformBlock<(Node Node, IList<Byte[]> Msg), (TransportProtocol Protocol, ZMessage Msg)> m_encodeSignalTransform;

        private ActionBlock<MessageData> m_decodedMessageProcessingAction;
        private ActionBlock<MessageData> m_decodedCommandMessageProcessingAction;
        private ActionBlock<(TransportProtocol Protocol, ZMessage Msg)> m_sendAction;

        private CommandMessageWorker m_cmdWorker;
    }
}
