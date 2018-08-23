using System;
using System.Threading.Tasks.Dataflow;
using TypeCreator.Base;

namespace MiddlewareLogic.MessageTypeWrappers.Base
{
    public interface IServiceMessenger<out M, out G> : ITopicMessenger<M>
    {
        BufferBlock<Message> GoalBuffer { get; }

        Func<G> ServiceMessageCreator { get; }
    }
}
