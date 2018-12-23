using BattleSystem;
using System;
using UnityEngine;

public class UnityLogger : BattleSystem.ILogger
{
    public void Log(object message)
    {
        UnityEngine.Debug.Log(message.ToString());
    }

    public void LogFormat(string format, params object[] args)
    {

        UnityEngine.Debug.LogFormat(string.Format(format, args));
    }

    public void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message.ToString());
    }

    public void LogWarningFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(string.Format(format, args));
    }

    public void LogError(object message)
    {
        UnityEngine.Debug.LogError(message.ToString());
    }

    public void LogErrorFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(string.Format(format, args));
    }

    public void LogException(Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }
}