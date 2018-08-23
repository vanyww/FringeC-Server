using System.Collections.Generic;

namespace WPFTerminal.Model
{
    public sealed class ExpandableConsoleMessage : ConsoleMessage
    {
        public ExpandableConsoleMessage() =>
            InnerMessages = new List<ConsoleMessage>();

        public List<ConsoleMessage> InnerMessages { get; set; }
    }
}
