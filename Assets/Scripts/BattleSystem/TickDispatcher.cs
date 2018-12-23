using BattleSystem.ObjectModule;
using System;

namespace BattleSystem
{
    public static class TickDispatcher
    {
        private static ITickReceiver mReceiver = null;
        public static void Init(ITickReceiver receiver)
        {
            mReceiver = receiver;
        }

        public static void OnCreateUnit(int ID, int templateID, int campID, int level)
        {
            if(mReceiver != null)
            {
                mReceiver.OnCreateUnit(ID,templateID, campID, level);
            }
        }
        public static void OnUnitPositionChanged(int ID,float x,float y)
        {
            if (mReceiver != null)
            {
                mReceiver.OnUnitPositionChanged(ID, x, y);
            }
        }
        public static void OnUnitStateChanged(int ID, UnitState state,params Object[] args)
        {

            if (mReceiver != null)
            {
                mReceiver.OnUnitStateChanged(ID, state, args);
            }
        }

    }
}
