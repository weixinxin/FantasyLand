using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleSystem.Util;
using BattleSystem.Config;

namespace BattleSystem.SkillModule
{
    public class SkillReader
    {
        public Dictionary<int, SkillAction> LoadedList = new Dictionary<int, SkillAction>();
        public SkillAction LoadSkillAction(int templateID,Skill skill)
        {
            if(!LoadedList.ContainsKey(templateID))
            {
                string xmlPath = string.Format(ConfigData.configDir + "config/skillaction/{0}.xml",templateID);
                //创建xml文档
                XmlDocument xml = Utils.LoadXMLDocument(xmlPath);
                if (xml == null)
                {
                    Logger.LogErrorFormat("Load skill action failed path = {0}", xmlPath);
                    return null;
                }
                XmlNode skillXml = xml.SelectSingleNode("Skill");
                var act = LoadAction(skillXml.FirstChild as XmlElement);
                LoadedList.Add(templateID, act);
            }
            return LoadedList[templateID].Copy(skill);
        }

        private SkillAction LoadAction(XmlElement node)
        {
            switch(node.Name)
            {
                case "SequenceAction":
                    return LoadSequenceAction(node);
                case "ParallelAction":
                    return LoadParallelAction(node);
                case "WaitSecondsAction":
                    return LoadWaitSecondsAction(node);
                case "SelectTargetAction":
                    return LoadSelectTargetAction(node);
                case "AddBuffAction":
                    return LoadAddBuffAction(node);
                case "AoeFieldAction":
                    return LoadAoeFieldAction(node);
                case "PlayAnimationAction":
                    return LoadPlayAnimationAction(node);
                case "PlayEffectAction":
                    return LoadPlayEffectAction(node);
                case "PlaySoundAction":
                    return LoadPlaySoundAction(node);
            }
            return null;
        }
        private SequenceAction LoadSequenceAction(XmlElement node)
        {
            SkillAction[] acts = new SkillAction[node.ChildNodes.Count];
            for (int i = 0; i < node.ChildNodes.Count;++i)
            {
                acts[i] = LoadAction(node.ChildNodes[i] as XmlElement);
            }
            return new SequenceAction(acts);
        }
        private ParallelAction LoadParallelAction(XmlElement node)
        {
            SkillAction[] acts = new SkillAction[node.ChildNodes.Count];
            for (int i = 0; i < node.ChildNodes.Count;++i)
            {
                acts[i] = LoadAction(node.ChildNodes[i] as XmlElement);
            }
            return new ParallelAction(acts);
        }
        private WaitSecondsAction LoadWaitSecondsAction(XmlElement node)
        {
            float time = float.Parse(node.GetAttribute("time"));
            return new WaitSecondsAction(time);
        }
        private SelectTargetAction LoadSelectTargetAction(XmlElement node)
        {
            var action =  new SelectTargetAction();
            action.filter = (TargetFilter)Enum.Parse(typeof(TargetFilter),node.GetAttribute("filter"));
            if (action.filter != TargetFilter.kSelf)
            {
                action.range = (TargetRange)Enum.Parse(typeof(TargetRange), node.GetAttribute("range"));
                if(action.range == TargetRange.kCirclefield)
                {
                    action.radius = float.Parse(node.GetAttribute("radius"));
                }
            }
            return action;
        }
        private AddBuffAction LoadAddBuffAction(XmlElement node)
        {
            int buff = int.Parse(node.GetAttribute("buff"));
            return new AddBuffAction(buff);
        }

        private AoeFieldAction LoadAoeFieldAction(XmlElement node)
        {
            AoeFieldAction action = new AoeFieldAction();

            action.duration = float.Parse(node.GetAttribute("duration"));
            action.interval = float.Parse(node.GetAttribute("interval"));
            action.emitters = new List<BuffEmitter>();
            bool initRegion = false;
            for (int i = 0; i < node.ChildNodes.Count; ++i)
            {
                XmlElement child = (XmlElement)node.ChildNodes[i];
                switch (node.ChildNodes[i].Name)
                {
                    case "CircleRegion":
                        action.type = RegionType.kCircle;
                        action.radius = float.Parse(child.GetAttribute("radius"));
                        initRegion = true;
                        break;
                    case "RectRegion":
                        action.type = RegionType.kRect;
                        action.width = float.Parse(child.GetAttribute("width"));
                        action.height = float.Parse(child.GetAttribute("height"));
                        initRegion = true;
                        break;
                    case "SectorRegion":
                        action.type = RegionType.kSector;
                        action.radius = float.Parse(child.GetAttribute("radius"));
                        action.theta = float.Parse(child.GetAttribute("theta"));
                        initRegion = true;
                        break;
                    case "BuffEmitter":
                        BuffEmitter emitter = new BuffEmitter();
                        emitter.filter = (AoeFilter)Enum.Parse(typeof(AoeFilter), child.GetAttribute("filter"));
                        string[] buffs = child.GetAttribute("buffs").Split('|');
                        emitter.buffs = new int[buffs.Length];
                        for (int n = 0; n < buffs.Length;++n)
                        {
                            emitter.buffs[n] = int.Parse(buffs[n]);
                        }
                        action.emitters.Add(emitter);
                        break;
                }
            }
            if (action.emitters.Count == 0)
                throw new Exception("AoeFieldAction miss emitter!");
            if (!initRegion)
                throw new Exception("AoeFieldAction miss region!");
            return action;
        }
        private PlayAnimationAction LoadPlayAnimationAction(XmlElement node)
        {
            string name = node.GetAttribute("name");
            float duration = float.Parse(node.GetAttribute("duration"));
            return new PlayAnimationAction(name, duration);
        }

        private PlayEffectAction LoadPlayEffectAction(XmlElement node)
        {
            string name = node.GetAttribute("name");
            return new PlayEffectAction(name);
        }
        private PlaySoundAction LoadPlaySoundAction(XmlElement node)
        {
            string name = node.GetAttribute("name");
            return new PlaySoundAction(name);
        }
        
    }
    
    
}
