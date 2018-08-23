using MiddlewareLogic.Router.Base;
using System;

namespace MiddlewareLogic.Events
{
    public sealed class RNodeAddedEventArgs : EventArgs
    {
        public RNodeAddedEventArgs(RNode node) => AddedNode = node;

        public RNode AddedNode { get; }
    }
}
