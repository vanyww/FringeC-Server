using MessagingCore.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using SystemSignalsLogic.SignalWorkers;
using Utils;

namespace MessagingCore.Main
{
    public sealed class Node
    {
        public Node(Int32 identity,
                    String name,
                    //NodeSignalsWorker udpSignalsWorker,
                    NodeMainTCPSignalWorker tcpSignalsWorker,
                    NodeCommandSignalWorker cmdSignalsWorker)
        {
            Identity = identity;
            Name = name;

            InMessageBuffer = new BufferBlock<IList<Byte[]>>();
            OutMessageBuffer = new BufferBlock<(Node Node, IList<Byte[]> Msg)>();

            CommandInMessageBuffer = new BufferBlock<IList<Byte[]>>();
            CommandOutMessageBuffer = new BufferBlock<(Int32 Node, IList<Byte[]> Msg)>();

            TCPSocketSignalsWorker = tcpSignalsWorker;
            //UDPSocketSignalsWorker = udpSignalsWorker;
            CMDSocketSignalsWorker = cmdSignalsWorker;
        }

        public Int32 Identity { get; }
        public String Name { get; }
        public NodeStatus Status { get; internal set; }

        //public NodeSignalsWorker UDPSocketSignalsWorker { get; }
        public NodeMainTCPSignalWorker TCPSocketSignalsWorker { get; }
        public NodeCommandSignalWorker CMDSocketSignalsWorker { get; }

        public BufferBlock<IList<Byte[]>> InMessageBuffer { get; }
        public BufferBlock<(Node Node, IList<Byte[]> Msg)> OutMessageBuffer { get; }

        public BufferBlock<IList<Byte[]>> CommandInMessageBuffer { get; }
        public BufferBlock<(Int32 Node, IList<Byte[]> Msg)> CommandOutMessageBuffer { get; }
    }
}
