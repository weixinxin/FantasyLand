using System;

namespace BattleSystem
{
    public class BuffTrigger
    {
        public bool CheckCondition()
        {
            return true;
        }

        public BuffTrigger()
        {
            EventCenter.Instance.AttachEvent(EventCode.UnitDead, this.OnUnitDead);
        }
        public void OnUnitDead(params object[] args)
        {

        }
    }
}
