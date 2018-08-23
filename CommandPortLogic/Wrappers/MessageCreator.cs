using System;
using TypeCreator;
using TypeCreator.Base;

namespace CommandPortLogic.Wrappers
{
    public sealed class MessageCreator<R, G> : IMessageCreator
        where R : Message, new()
        where G : Message, new()
    {
        public MessageCreator()
        {
            m_requestInstanceCreator = InstanceFactoryCreator.Instance.CreateFactory<R>();
            m_replyInstanceCreator = InstanceFactoryCreator.Instance.CreateFactory<G>();
        }

        private Func<R> m_requestInstanceCreator;
        private Func<G> m_replyInstanceCreator;

        public Message CreateReply(Byte[][] message)
        {
            var reply = m_replyInstanceCreator();
            reply.SetProperties(message);
            return reply;
        }

        public Message CreateReply(Object[] message)
        {
            var reply = m_replyInstanceCreator();
            reply.SetProperties(message);
            return reply;
        }

        public Message CreateRequest(Object[] message)
        {
            var request = m_requestInstanceCreator();
            request.SetProperties(message);
            return request;
        }

        public Message CreateRequest(Byte[][] message)
        {
            var request = m_requestInstanceCreator();
            request.SetProperties(message);
            return request;
        }

        public Message CreateReply() => m_requestInstanceCreator();

        public Message CreateRequest() => m_replyInstanceCreator();
    }
}
