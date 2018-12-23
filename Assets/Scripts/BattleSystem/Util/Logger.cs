using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace BattleSystem
{
    public static class Logger
    {
        private static ILogger log = null;

        public static void InitLogger(ILogger logger)
        {
            log = logger;
        }
        public static void Log(object message)
        {
            if (log != null)
                log.Log(message);
        }

        public static void LogFormat(string format, params object[] args)
        {
            if (log != null)
                log.LogFormat(format, args);
        }

        public static void LogWarning(object message)
        {
            if (log != null)
                log.LogWarning(message);
        }
        public static void LogWarningFormat(string format, params object[] args)
        {
            if (log != null)
                log.LogWarningFormat(format, args);
        }
        public static void LogError(object message)
        {
            if (log != null)
                log.LogError(message);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            if (log != null)
                log.LogErrorFormat(format, args);
        }
        public static void LogException(Exception exception)
        {
            if (log != null)
                log.LogException(exception);
        }
    }
}
