using MiddlewareLogic.MessageTypeWrappers.Base;
using System;

namespace MiddlewareLogic.Events
{
    public sealed class MessengerAddedEventArgs : EventArgs
    {
        public MessengerAddedEventArgs(Messenger messenger) => Messenger = messenger;

        public Messenger Messenger { get; }
    }
}