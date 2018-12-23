using BattleSystem.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
namespace BattleSystem.SkillModule
{
    public static class SkillManager
    {
        private static SkillReader skillReader = new SkillReader();
        public static SkillAction LoadSkillAction(int templateID, Skill skill)
        {
            return skillReader.LoadSkillAction(templateID,skill);
        }

    }
}
