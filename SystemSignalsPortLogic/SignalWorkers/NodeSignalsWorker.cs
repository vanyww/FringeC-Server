using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using SystemSignalsLogic.Enums;

namespace SystemSignalsLogic.SignalWorkers
{
    public class NodeSignalsWorker
    {
        public NodeSignalsWorker(Int32 nodeIdentity,
                                 ITargetBlock<(Int32, OUTSignal, IList<Byte[]>)> sendBuffer)
        {
            m_sendBuffer = sendBuffer;
            m_nodeIdentity = nodeIdentity;
        }

        #region IN SIGNALS

        public event EventHandler Pong;
        public event EventHandler Null;

        #endregion

        #region OUT SIGNALS

        public void SendNullSignal() => m_sendBuffer.Post((m_nodeIdentity, OUTSignal.Null, null));
        public void SendPingSignal() => m_sendBuffer.Post((m_nodeIdentity, OUTSignal.Ping, null));

        #endregion

        internal virtual void ProcessSignal(INSignal signal, IList<Byte[]> data)
        {
            switch (signal)
            {
                case INSignal.Pong:
                    Pong?.Invoke(this, new EventArgs());
                    break;

                case INSignal.Null:
                    Null?.Invoke(this, new EventArgs());
                    break;
            }
        }

        public Int32 NodeIdentity => m_nodeIdentity;

        protected ITargetBlock<(Int32, OUTSignal, IList<Byte[]>)> m_sendBuffer;
        protected Int32 m_nodeIdentity;
    }
}
