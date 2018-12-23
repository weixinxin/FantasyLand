using System;
using System.Collections.Generic;
using BattleSystem;
using BattleSystem.ObjectModule;
public class TickNotifyPlayer:ITickReceiver
{
    private CampaignAgent campaign;
    public TickNotifyPlayer(CampaignAgent campaign)
    {
        this.campaign = campaign;
        BattleSystem.TickDispatcher.Init(this);
    }

    public void OnCreateUnit(int ID,int templateID, int campID, int level)
    {
        campaign.AddUnit(ID,templateID, campID, level);
    }

    public void OnUnitPositionChanged(int ID, float x, float y)
    {
        campaign.UpdateUnitPosition(ID,x,0,y);
    }
    public void OnUnitStateChanged(int ID, UnitState state, params Object[] args)
    {
        campaign.UpdateUnitState(ID, state, args);
    }
}
