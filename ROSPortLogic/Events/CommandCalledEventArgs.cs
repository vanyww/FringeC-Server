using MiddlewareLogic.Commands;
using System;

namespace MiddlewareLogic.Events
{
    internal sealed class CommandCalledEventArgs : EventArgs
    {
        public CommandCalledEventArgs(OuterCommandCall call) => Call = call;

        public OuterCommandCall Call { get; }
    }
}
