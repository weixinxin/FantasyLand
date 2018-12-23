using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem
{
    public interface ILogger
    {
        void Log(object message);
        void LogFormat(string format, params object[] args);
        void LogWarning(object message);
        void LogWarningFormat(string format, params object[] args);
        void LogError(object message);
        void LogErrorFormat(string format, params object[] args);

        void LogException(Exception exception);
    }
}
