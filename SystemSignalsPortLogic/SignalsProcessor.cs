using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using SystemSignalsLogic.Enums;
using SystemSignalsLogic.SignalWorkers;

[assembly: InternalsVisibleTo("MessagingCore")]

namespace SystemSignalsLogic
{
    internal class SignalsProcessor
    {
        public SignalsProcessor()
        {
            m_processSignalAction = new ActionBlock<(NodeSignalsWorker worker, INSignal signal, IList<Byte[]> data)>(signalData =>
                signalData.worker.ProcessSignal(signalData.signal, signalData.data));
            m_decodeSignalTransform = new TransformBlock<(NodeSignalsWorker Worker, IList<Byte[]> Message),
                                                         (NodeSignalsWorker Worker, INSignal Signal, IList<Byte[]> Data)>(msg =>
                                                         {
                                                             var (Signal, Data) = NodeSignalsCodec.DecodeSignal(msg.Message);
                                                             return (msg.Worker, Signal, Data);
                                                         });

            m_encodeSignalTransform = new TransformBlock<(Int32 Node, OUTSignal Signal, IList<Byte[]> Data), (Int32 Node, IList<Byte[]> Data)>(signalData =>
                (signalData.Node, NodeSignalsCodec.EncodeSignal(signalData.Signal, signalData.Data)));
            m_decodeSignalTransform.LinkTo(m_processSignalAction);
        }

        public TransformBlock<(NodeSignalsWorker Worker, IList<Byte[]> Message),
                                      (NodeSignalsWorker Worker, INSignal Signal, IList<Byte[]> Data)> RecvSignalBuffer => m_decodeSignalTransform;
        public TransformBlock<(Int32 Node, OUTSignal Signal, IList<Byte[]> Data), (Int32 Node, IList<Byte[]> Data)> SendSignalBuffer => m_encodeSignalTransform;

        private TransformBlock<(NodeSignalsWorker Worker, IList<Byte[]> Message),
                                      (NodeSignalsWorker Worker, INSignal Signal, IList<Byte[]> Data)> m_decodeSignalTransform;
        private TransformBlock<(Int32 Node, OUTSignal Signal, IList<Byte[]> Data), (Int32 Node, IList<Byte[]> Data)> m_encodeSignalTransform;
        private ActionBlock<(NodeSignalsWorker, INSignal, IList<Byte[]>)> m_processSignalAction;
    }
}
