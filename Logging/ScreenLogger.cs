using System;
using System.Threading.Tasks.Dataflow;
using NLog;

namespace Logging
{
    public static class ScreenLogger
    {
        private const String MainLoggerName = "MainLog";

        static ScreenLogger()
        {
            m_messageBuffer = new BufferBlock<LogEntry>();
            m_logger = LogManager.GetLogger(MainLoggerName);
        }

        public static void Log(LogEntry entry)
        {
            m_messageBuffer.Post(entry);
            m_logger.Info(entry.Message);
        }

        public static void Log(String message)
        {
            m_messageBuffer.Post(new LogEntry
            {
                Message = message
            });
            m_logger.Info(message);
        }

        public static void Log(String message, String sub)
        {
            m_messageBuffer.Post(new LogEntry
            {
                Message = message,
                SubInfo = sub
            });
            m_logger.Info(message);
        }

        public static void Log(String message, LogObject obj)
        {
            m_messageBuffer.Post(new LogEntry
            {
                Message = message,
                Object = obj
            });
            m_logger.Info(message);
        }

        public static void Log(String message, String sub, LogObject obj)
        {
            m_messageBuffer.Post(new LogEntry
            {
                Message = message,
                SubInfo = sub,
                Object = obj
            });
            m_logger.Info(message);
        }

        public static BufferBlock<LogEntry> MessageBuffer => m_messageBuffer;

        private static Logger m_logger;
        private static BufferBlock<LogEntry> m_messageBuffer;
    }
}
