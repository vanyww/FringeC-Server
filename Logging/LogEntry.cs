using System;

namespace Logging
{
    public sealed class LogEntry
    {
        public LogEntry() { Object = LogObject.None; }
        public LogEntry(String msg, LogEntry[] inners = null) : this()
        {
            Message = msg;
            InnerMessages = inners;
        }

        public LogEntry(String msg, String sub, LogEntry[] inners = null) : this()
        {
            SubInfo = sub;
            Message = msg;
            InnerMessages = inners;
        }

        public LogEntry(String msg, LogObject obj, LogEntry[] inners = null)
        {
            Object = obj;
            Message = msg;
            InnerMessages = inners;
        }

        public LogEntry(String msg, String sub, LogObject obj, LogEntry[] inners = null)
        {
            SubInfo = sub;
            Message = msg;
            Object = obj;
            InnerMessages = inners;
        }

        public LogObject Object { get; internal set; }
        public String Message { get; internal set; }
        public String SubInfo { get; internal set; }

        public LogEntry[] InnerMessages { get; internal set; }
    }
}
