using NLog;
using System;

namespace Logging
{
    public class LoggingBase
    {
        private const String MainLoggerName = "MainLog";

        public LoggingBase()
        {
            m_logger = LogManager.GetLogger(MainLoggerName);
        }

        protected Logger Logger => m_logger;

        private static Logger m_logger;
    }
}
