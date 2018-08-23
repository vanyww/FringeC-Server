using System;
using TypeCreator.Enums;

namespace TypeCreator.Base
{
    public sealed class MessagePartDescription
    {
        public String Name { get; set; }
        public MessageValueType Type { get; set; }

    public override String ToString() =>
            $"{Name} <{Type}>";
    }
}
