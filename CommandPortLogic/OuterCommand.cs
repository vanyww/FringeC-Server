using CommandPortLogic.Enums;
using CommandPortLogic.Wrappers;
using System;
using System.Collections.Generic;
using TypeCreator.Base;

namespace CommandPortLogic
{
    public class OuterCommand
    {
        public Int32 Node { get; set; }
        public Int32 CommandId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public CommandUsage Usage { get; set; }
        public IList<MessagePartDescription> RequestDescription { get; set; }
        public IList<MessagePartDescription> ReplyDescription { get; set; }
        public IMessageCreator MessageWrapper { get; set; }

        public OuterCommandCall ToCommandCall(Message msg) =>
            new OuterCommandCall
            {
                Command = this,
                CallMessage = msg
            };
    }
}

