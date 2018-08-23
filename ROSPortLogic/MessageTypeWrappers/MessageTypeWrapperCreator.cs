using MiddlewareLogic.Enums;
using System;
using System.Diagnostics.Contracts;

namespace MiddlewareLogic.MessageTypeWrappers
{
    public static class MessengerTypeCreator
    {
        public static Type CreateMessengerType(MessengerType messenger, Type[] types)
        {
            if ((types?.Length ?? 0) != (Int32)messenger)
                throw new ArgumentException("Wrong types number.");
            Contract.EndContractBlock();

            switch (messenger)
            {
                case MessengerType.Raw:
                    return typeof(RawMessenger);

                case MessengerType.Topic:
                    return typeof(TopicMessenger<>).MakeGenericType(types);

                case MessengerType.Service:
                    return typeof(ServiceMessenger<,>).MakeGenericType(types);

                case MessengerType.Action:
                    return typeof(ActionMessenger<,,>).MakeGenericType(types);

                default:
                    return null;
            }
        }
    }
}
