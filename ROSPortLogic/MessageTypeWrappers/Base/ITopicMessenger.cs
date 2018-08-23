using System;
using System.Threading.Tasks.Dataflow;
using TypeCreator.Base;

namespace MiddlewareLogic.MessageTypeWrappers.Base
{
    public interface ITopicMessenger<out M> : IRawMessenger
    {
        BufferBlock<Message> MessageBuffer { get; }

        Func<M> TopicMessageCreator { get; }
    }
}
