using System;
using TypeCreator.Base;

namespace CommandPortLogic
{
    public sealed class CommandReply
    {
        public Int32 CallId { get; set; }
        public Int32 CommandId { get; set; }
        public Message Result { get; set; }
    }
}
