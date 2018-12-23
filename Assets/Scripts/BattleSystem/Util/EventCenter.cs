using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem
{
    public class EventCenter
    {
        public static readonly EventCenter Instance = new EventCenter();
        public delegate void EventHandle(params object[] args);
        private List<EventHandle>[] _EventMap = new List<EventHandle>[32];
        public void AttachEvent(EventCode eventCode, EventHandle eventHandle)
        {
            List<EventHandle> list = _EventMap[(int)eventCode];
            if (list != null)
            {
                list.Add(eventHandle);
            }
            else
            {
                list = new List<EventHandle>();
                list.Add(eventHandle);
                _EventMap[(int)eventCode] = list;
            }
        }
        public bool DetachEvent(EventCode eventCode, EventHandle eventHandle)
        {
            List<EventHandle> list = _EventMap[(int)eventCode];
            if (list != null)
            {
                return list.Remove(eventHandle);
            }
            return false;
        }
        public void DispatchEvent(EventCode eventCode, params object[] args)
        {
            List<EventHandle> list = _EventMap[(int)eventCode];
            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    var func = list[i];
                    func(args);
                }
            }
        }
        public void Clear()
        {
            for (int i = 0; i < _EventMap.Length; ++i)
            {
                _EventMap[i] = null;
            }
        }
    }
}
