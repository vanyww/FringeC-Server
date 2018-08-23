using System;
using TypeCreator.Base;

namespace CommandPortLogic.Wrappers
{
    public interface IMessageCreator
    {
        Message CreateReply(Object[] message);
        Message CreateReply(Byte[][] message);
        Message CreateReply();
        Message CreateRequest(Object[] message);
        Message CreateRequest(Byte[][] message);
        Message CreateRequest();
    }
}
