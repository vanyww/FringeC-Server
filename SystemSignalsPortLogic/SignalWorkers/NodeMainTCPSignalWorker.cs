using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using SystemSignalsLogic.Enums;
using SystemSignalsLogic.Events;

namespace SystemSignalsLogic.SignalWorkers
{
    public sealed class NodeMainTCPSignalWorker : NodeSignalsWorker
    {
        public NodeMainTCPSignalWorker(Int32 nodeIdentity, ITargetBlock<(Int32, OUTSignal, IList<Byte[]>)> sendBuffer) :
            base(nodeIdentity, sendBuffer)
        { }

        #region IN SIGNALS

        public event EventHandler<NodeSignalEventArgs> MessengerInitialization;

        #endregion

        internal override void ProcessSignal(INSignal signal, IList<Byte[]> data)
        {
            base.ProcessSignal(signal, data);

            if (signal == INSignal.MessangerInitialization)
                MessengerInitialization?.Invoke(this, new NodeSignalEventArgs(data));
        }

        internal void RaiseMessangerInitialization(IList<Byte[]> data) => 
            MessengerInitialization?.Invoke(this, new NodeSignalEventArgs(data));
    }
}
