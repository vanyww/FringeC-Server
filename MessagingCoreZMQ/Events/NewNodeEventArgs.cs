using MessagingCore.Main;
using System;

namespace MessagingCore.Events
{
    public class NodeRegisteredEventArgs : EventArgs
    {
        public NodeRegisteredEventArgs(Node newNode) { RegisteredNode = newNode; }

        public Node RegisteredNode { get; }
    }
}
