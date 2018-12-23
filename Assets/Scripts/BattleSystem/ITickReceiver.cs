using BattleSystem.ObjectModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem
{
    public interface ITickReceiver
    {
        void OnCreateUnit(int ID, int templateID, int campID, int level);

        void OnUnitPositionChanged(int ID, float x, float y);
        void OnUnitStateChanged(int ID, UnitState state,params Object[] args);
    }
}
