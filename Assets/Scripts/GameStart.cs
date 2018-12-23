using UnityEngine;
using System.Collections;
using Framework;
using BattleSystem;
using BattleSystem.Config;
public class GameStart : MonoBehaviour {

	// Use this for initialization
    Campaign campaign;

    CampaignAgent campaignAgent;
    void Start()
    {
        BattleSystem.Logger.InitLogger(new UnityLogger());
        campaignAgent = new CampaignAgent();
        LuaManager.Init();
        ConfigData.Init(Application.dataPath + "/Resources/data/");
        campaign = new Campaign();
        campaign.Init(2);


        var unit1 = Campaign.Instance.AddUnit(100005, 0, 1);
        var unit2 = Campaign.Instance.AddUnit(100002, 1, 1);
        var unit3 = Campaign.Instance.AddUnit(100001, 1, 1);
        unit1.position = new vector3(0, 16);
        unit2.position = new vector3(-6, 11);
        unit3.position = new vector3(6, 16);
	}
	
	// Update is called once per frame
	void Update () {
        campaign.Update(Time.deltaTime);
	}
    void FixUpdate()
    {

    }
}
