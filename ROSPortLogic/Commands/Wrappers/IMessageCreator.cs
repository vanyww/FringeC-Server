using System;
using System.Collections.Generic;
using TypeCreator.Base;

namespace MiddlewareLogic.Commands.Wrappers
{
    public interface IMessageCreator
    {
        Message CreateReply(Object[] objects);
        Message CreateReply(IList<Byte[]> message);
        Message CreateReply();
        Message CreateRequest(Object[] objects);
        Message CreateRequest(IList<Byte[]> message);
        Message CreateRequest();
    }
}
