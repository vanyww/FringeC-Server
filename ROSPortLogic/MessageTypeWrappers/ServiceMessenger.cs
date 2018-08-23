using MiddlewareLogic.MessageTypeWrappers.Base;
using System;
using System.Threading.Tasks.Dataflow;
using TypeCreator;
using TypeCreator.Base;

namespace MiddlewareLogic.MessageTypeWrappers
{
    public class ServiceMessenger<M, G> : TopicMessenger<M>, IServiceMessenger<M, G>
        where M : Message, new()
        where G : Message, new()
    {
        public ServiceMessenger(MessengerInfo info) : base(info)
        {
            m_goalBuffer = new BufferBlock<Message>();
            m_creator = InstanceFactoryCreator.Instance.CreateFactory<G>();
        }

        public BufferBlock<Message> GoalBuffer => m_goalBuffer;
        public Func<G> ServiceMessageCreator => m_creator;

        private BufferBlock<Message> m_goalBuffer;
        private Func<G> m_creator;
    }
}
