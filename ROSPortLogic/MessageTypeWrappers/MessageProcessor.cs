using MiddlewareLogic.Enums;
using MiddlewareLogic.MessageTypeWrappers.Base;
using System;
using System.Collections.Generic;
using TypeCreator.Base;

namespace MiddlewareLogic.MessageTypeWrappers
{
    internal static class MessageProcessor
    {
        public static void ProcessMessage(MessageSubtype messageSubtype, IList<Byte[]> data, Messenger typeWrapper)
        {
            switch (messageSubtype)
            {
                case MessageSubtype.Reply:
                    OnReply();
                    break;

                case MessageSubtype.Feedback:
                    OnFeedback();
                    break;
            }

            void OnReply()
            {
                var casted = typeWrapper as IRawMessenger;
                if (casted is null)
                    throw new InvalidCastException("Wrong message type.");

                casted.ReceiveMessage(data);
            }

            void OnFeedback()
            {
                var casted = typeWrapper as IActionMessenger<Message, Message, Message>;
                if (casted is null)
                    throw new InvalidCastException("Wrong message type.");

                casted.ReceiveFeedback(data);
            }
        }
    }
}
