using MiddlewareLogic.Enums;
using MiddlewareLogic.MessageTypeWrappers.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using TypeCreator;
using TypeCreator.Base;

namespace MiddlewareLogic.MessageTypeWrappers
{
    public class ActionMessenger<M, G, F> : ServiceMessenger<M, G>, IActionMessenger<M, G, F>
        where M : Message, new()
        where G : Message, new()
        where F : Message, new()
    {
        public ActionMessenger(MessengerInfo info) : base(info)
        {
            m_feedbackBuffer = new BufferBlock<Message>();
            m_creator = InstanceFactoryCreator.Instance.CreateFactory<F>();
        }

        public void ReceiveFeedback(IList<Byte[]> message)
        {
            Message newMessage = m_creator();
            newMessage.SetProperties(message);
            m_feedbackBuffer.Post(newMessage);
        }

        public BufferBlock<Message> FeedbackBuffer => m_feedbackBuffer;
        public Func<F> ActionMessageCreator => m_creator;

        private BufferBlock<Message> m_feedbackBuffer;
        private Func<F> m_creator;
    }
}
