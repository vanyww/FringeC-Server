using MiddlewareLogic.Commands.Wrappers;
using MiddlewareLogic.Enums;
using System;
using System.Collections.Generic;
using TypeCreator.Base;

namespace MiddlewareLogic.Commands
{
    public class OuterCommand
    {
        public Int32 Node { get; internal set; }
        public Byte Messenger { get; internal set; }
        public Int32 CommandId { get; internal set; }
        public String Name { get; internal set; }
        public String Description { get; internal set; }
        public CommandUsage Usage { get; internal set; }
        public IList<MessagePartDescription> RequestDescription { get; internal set; }
        public IList<MessagePartDescription> ReplyDescription { get; internal set; }
        public IMessageCreator MessageWrapper { get; internal set; }

        internal Action<OuterCommandCall> CallCommandAction { get; set; }
        internal Action<OuterCommandCall> CallCommandBufferAction { get; set; }

        public OuterCommandCall ToCommandCall(Message msg) =>
            new OuterCommandCall
            (
                this,
                msg,
                CallCommandAction,
                CallCommandBufferAction
            );


    }
}

