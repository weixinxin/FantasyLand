using System;
//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorBlackboard
    {
        private Dictionary<string, object> m_Datas = new Dictionary<string, object>();

        public bool AddVariable<T>(string varName, T value)
        {
            if (m_Datas.ContainsKey(varName))
            {
                return false;
            }
            
            m_Datas.Add(varName, value);

            return true;
        }

        public T GetValue<T>(string varName)
        {
            object value;
            if (m_Datas.TryGetValue(varName, out value))
            {
                if (value is WeakReference)
                {
                    return (T)(value as WeakReference).Target;
                }
                else
                {
                    return (T)value;
                }
            }
           // Debug.LogError(string.Format("undefined behavior blackboard variable of {0}", varName));
            return default(T);
        }
        public T GetValueDefault<T>(string varName, T def)
        {
            object value;
            if (m_Datas.TryGetValue(varName, out value))
            {
                if (value is WeakReference)
                {
                    return (T)(value as WeakReference).Target;
                }
                else
                {
                    return (T)value;
                }
            }
            return def;
        }
        public bool SetValue<T>(string varName, T value)
        {
            if (m_Datas.ContainsKey(varName))
            {
                m_Datas[varName] = value;
                 
                return true;
            }
           // Debug.LogError(string.Format("undefined behavior blackboard variable of {0}", varName));
            return false;
        }
    }
}
