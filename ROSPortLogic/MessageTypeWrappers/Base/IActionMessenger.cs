using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using TypeCreator.Base;

namespace MiddlewareLogic.MessageTypeWrappers.Base
{
    public interface IActionMessenger<out M, out G, out F> : IServiceMessenger<M, G>
    {
        void ReceiveFeedback(IList<Byte[]> message);

        BufferBlock<Message> FeedbackBuffer { get; }

        Func<F> ActionMessageCreator { get; }
    }
}
