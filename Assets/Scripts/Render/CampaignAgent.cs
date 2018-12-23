using BattleSystem.Config;
using BattleSystem.ObjectModule;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CampaignAgent
{

    private Dictionary<int, UnitAgent> mUnits;
    private TickNotifyPlayer tickNotifyPlayer;
    public CampaignAgent()
    {
        mUnits = new Dictionary<int, UnitAgent>();
        tickNotifyPlayer = new TickNotifyPlayer(this);
    }

    public void AddUnit(int ID, int templateID, int campID, int level)
    {
        var config = ConfigData.Unit.getRow(templateID);
        var prefab = Resources.Load<GameObject>(config.ModelName);
        var obj = GameObject.Instantiate(prefab);
        UnitAgent unit = obj.GetComponent<UnitAgent>();
        unit.Init(ID, templateID, campID, level);
        mUnits.Add(ID, unit);
    }
    public void UpdateUnitPosition(int ID,float x,float y,float z)
    {
        var unit = mUnits[ID];
        unit.UpdatePosition(x, y, z);
    }
    public void UpdateUnitState(int ID, UnitState state,params System.Object[] args)
    {
        var unit = mUnits[ID];
        switch (state)
        {
            case UnitState.kIdel:
                unit.OnIdle();
                break;
            case UnitState.kMove:
                unit.OnMove();
                break;
            case UnitState.kAttack:
                int target_id = (int)args[0];
                var target = mUnits[target_id];
                unit.OnAttack(target);
                break;
            case UnitState.kDead:
                unit.OnDead();
                break;
        }
    }
}
