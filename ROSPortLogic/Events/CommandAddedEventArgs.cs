using MiddlewareLogic.Commands;
using System;

namespace MiddlewareLogic.Events
{
    public sealed class CommandAddedEventArgs : EventArgs
    {
        public CommandAddedEventArgs(OuterCommand command) => Command = command;

        public OuterCommand Command { get; }
    }
}
