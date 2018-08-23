using MiddlewareLogic.Enums;
using MiddlewareLogic.MessageTypeWrappers.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using TypeCreator;
using TypeCreator.Base;

namespace MiddlewareLogic.MessageTypeWrappers
{
    public class TopicMessenger<M> : Messenger, ITopicMessenger<M>
        where M : Message, new()
    {
        public TopicMessenger(MessengerInfo info) : base(info)
        {
            m_messageBuffer = new BufferBlock<Message>();
            m_creator = InstanceFactoryCreator.Instance.CreateFactory<M>();
        }

        public void ReceiveMessage(IList<Byte[]> message)
        {
            Message newMessage = m_creator();
            newMessage.SetProperties(message);
            m_messageBuffer.Post(newMessage);
        }

        public BufferBlock<Message> MessageBuffer => m_messageBuffer;
        public Func<M> TopicMessageCreator => m_creator;

        private BufferBlock<Message> m_messageBuffer;
        private Func<M> m_creator;
    }
}
