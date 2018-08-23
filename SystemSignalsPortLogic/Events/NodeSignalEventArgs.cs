using System;
using System.Collections.Generic;

namespace SystemSignalsLogic.Events
{
    public sealed class NodeSignalEventArgs : EventArgs
    {
        public NodeSignalEventArgs(IList<Byte[]> data) => Data = data;

        public IList<Byte[]> Data { get; }
    }
}
