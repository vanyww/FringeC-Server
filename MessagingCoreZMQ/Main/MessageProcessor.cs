using MessagingCore.Enums;
using MessagingCore.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using SystemSignalsLogic;
using SystemSignalsLogic.SignalWorkers;

namespace MessagingCore.Main
{
    internal sealed class MessageProcessor
    {
        public MessageProcessor()
        {
            m_nodes = new Dictionary<Int32, Node>();

            SendMessageBuffer = new BufferBlock<(Node Node, IList<Byte[]> Message)>();
            SendSignalBuffer = new BufferBlock<(Node Node, IList<Byte[]> Message)>();
            SendCommandBuffer = new BufferBlock<(Int32 Node, IList<Byte[]> Message)>();

            m_signalsProcessor = new SignalsProcessor();
            var sendSignal = new TransformBlock<(Int32 Id, IList<Byte[]> Data), (Node, IList<Byte[]>)>(signalData => 
                (m_nodes[signalData.Id], signalData.Data));

            m_signalsProcessor.SendSignalBuffer.LinkTo(sendSignal);
            sendSignal.LinkTo(SendSignalBuffer);
        }

        public void ProcessMessage(MessageData message)
        {
            switch (message.Type)
            {
                case MessageType.Common:
                    OnNormalMessage();
                    return;

                case MessageType.NodeSignal:
                    OnNodeSignalMessage();
                    return;

                case MessageType.NodeInitialization:
                    OnNewNode();
                    return;
            }


            void OnNewNode()
            {
                var nodeName = MessageCodec.DecodeNodeInitialization(message.Data);

                //var udpSigWorker = new NodeSignalsWorker(message.Identity, m_signalsProcessor.SendSignalBuffer);
                var tcpSigWorker = new NodeMainTCPSignalWorker(message.Identity, m_signalsProcessor.SendSignalBuffer);
                var cmdSigWorker = new NodeCommandSignalWorker(message.Identity, m_signalsProcessor.SendSignalBuffer);
                
                var node = new Node(message.Identity,
                                    nodeName,
                                    //udpSigWorker,
                                    tcpSigWorker,
                                    cmdSigWorker);

                m_nodes.Add(message.Identity, node);
                node.OutMessageBuffer.LinkTo(SendMessageBuffer);
                node.CommandOutMessageBuffer.LinkTo(SendCommandBuffer);

                NodeRegistered?.Invoke(this, new NodeRegisteredEventArgs(node));

                node.TCPSocketSignalsWorker.SendNullSignal();
            }

            void OnNodeSignalMessage()
            {
                if (m_nodes.TryGetValue(message.Identity, out var node))
                    m_signalsProcessor.RecvSignalBuffer.Post((//(message.Protocol == TransportProtocol.TCP) ?
                                                                   node.TCPSocketSignalsWorker, //:
                                                                   //node.UDPSocketSignalsWorker,
                                                            message.Data));
                else
                    OnUninitializedMessage();
            }

            void OnNormalMessage()
            {
                if (m_nodes.TryGetValue(message.Identity, out var node))
                    node.InMessageBuffer.Post(message.Data);
                else
                    OnUninitializedMessage();
            }
        }

        public void ProcessCommandMessage(MessageData message)
        {
            switch (message.Type)
            {
                case MessageType.NodeSignal:
                    OnNodeSignalMessage();
                    return;

                case MessageType.Command:
                    OnCommandMessage();
                    return;

                default:
                    throw new ArgumentException("Wrong message type.");
            }

            void OnNodeSignalMessage()
            {
                if (m_nodes.TryGetValue(message.Identity, out var node))
                    m_signalsProcessor.RecvSignalBuffer.Post((node.CMDSocketSignalsWorker,
                                                            message.Data));
                else
                    OnUninitializedMessage();
            }

            void OnCommandMessage()
            {
                if (m_nodes.TryGetValue(message.Identity, out var node))
                    node.CommandInMessageBuffer.Post(message.Data);
                else
                    OnUninitializedMessage();
            }
        }

        void OnUninitializedMessage()
        {
            /*
            var emptyDealer = DealersManager.AddNewDealer(identity, DealerStatus.Unknown);
            //set normal value for 2nd param
            if (MessageWorker.CommandsManager.TryGetCommand((CommandsManager.SystemId, ""), out OuterCommand command))
            {
                var msg = MessageWorker.CommandsManager.Coder.EncodeCommand(command);
                SendMessage(msg.dealer, msg.msg);
            }
            */
        }

        public event EventHandler<NodeRegisteredEventArgs> NodeRegistered;

        internal BufferBlock<(Node Node, IList<Byte[]> Message)> SendMessageBuffer { get; }
        internal BufferBlock<(Node Node, IList<Byte[]> Message)> SendSignalBuffer { get; }
        internal BufferBlock<(Int32 Node, IList<Byte[]> Message)> SendCommandBuffer { get; }

        public Dictionary<Int32, Node> Nodes => m_nodes;

        private Dictionary<Int32, Node> m_nodes;

        private SignalsProcessor m_signalsProcessor;
    }
}
