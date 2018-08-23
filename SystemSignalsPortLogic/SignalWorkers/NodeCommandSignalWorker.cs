using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using SystemSignalsLogic.Enums;
using SystemSignalsLogic.Events;

namespace SystemSignalsLogic.SignalWorkers
{
    public sealed class NodeCommandSignalWorker : NodeSignalsWorker
    {
        public NodeCommandSignalWorker(Int32 nodeIdentity, ITargetBlock<(Int32, OUTSignal, IList<Byte[]>)> sendBuffer) :
            base(nodeIdentity, sendBuffer)
        { }

        #region IN SIGNALS

        public event EventHandler<NodeSignalEventArgs> CommandInitialization;

        #endregion

        internal override void ProcessSignal(INSignal signal, IList<byte[]> data)
        {
            base.ProcessSignal(signal, data);

            if (signal == INSignal.CommandInitialization)
                CommandInitialization?.Invoke(this, new NodeSignalEventArgs(data));
        }

        internal void RaiseCommandInitialization(IList<Byte[]> data) => 
            CommandInitialization?.Invoke(this, new NodeSignalEventArgs(data));
    }
}
