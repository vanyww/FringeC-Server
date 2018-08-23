using System;
using System.Collections.Generic;
using TypeCreator;
using TypeCreator.Base;

namespace MiddlewareLogic.Commands.Wrappers
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

        public Message CreateReply(IList<Byte[]> message)
        {
            var reply = m_replyInstanceCreator();
            reply.SetProperties(message);
            return reply;
        }

        public Message CreateReply(Object[] objects)
        {
            var reply = m_replyInstanceCreator();
            reply.SetProperties(objects);
            return reply;
        }

        public Message CreateRequest(Object[] objects)
        {
            var request = m_requestInstanceCreator();
            request.SetProperties(objects);
            return request;
        }

        public Message CreateRequest(IList<Byte[]> message)
        {
            var request = m_requestInstanceCreator();
            request.SetProperties(message);
            return request;
        }

        public Message CreateReply() => m_requestInstanceCreator();

        public Message CreateRequest() => m_replyInstanceCreator();
    }
}
