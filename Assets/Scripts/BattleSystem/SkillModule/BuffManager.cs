using BattleSystem.ObjectModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.SkillModule
{
    public static class BuffManager
    {
        public static void Reset()
        {
            mUnitWillDieList.Clear();
            mUnitBeSlayedList.Clear();
            mUnitBeSummonedList.Clear();
            mUnitWillHurtList.Clear();
            mUnitBeHurtedList.Clear();
            mUnitWillHealList.Clear();
            mUnitBeHealedList.Clear();
            mSpellHitList.Clear();
        }

        private static List<Buff> mUnitWillDieList = new List<Buff>();

        public static void RegisterUnitWillDie(Buff buff)
        {
            if (!mUnitWillDieList.Contains(buff))
            {
                mUnitWillDieList.Add(buff);
            }
            else
            {
                throw new Exception("this buff has been registered!");
            }
        }
        public static void UnregisterUnitWillDie(Buff buff)
        {
            if (mUnitWillDieList.Contains(buff))
            {
                mUnitWillDieList.Remove(buff);
            }
            else
            {
                throw new Exception("this buff has not been registered!");
            }
        }
        /// <summary>
        /// 单位濒临死亡
        /// </summary>
        /// <param name="unit">死亡单位</param>
        /// <param name="slayer">攻击者</param>
        /// <returns>是否进入死亡</returns>
        public static bool OnUnitWillDie(UnitBase unit, UnitBase slayer)
        {

#if DEBUG
            Logger.LogFormat("OnUnitWillDie unit = {0} slayer = {1} ", unit.ID, slayer.ID);
#endif
            bool res = true;
            for (int i = mUnitWillDieList.Count - 1; i >= 0; --i)
            {
                if (!mUnitWillDieList[i].OnUnitWillDie(unit, slayer))
                {
                    res = false;
                }
            }
            return res;
        }

        private static List<Buff> mUnitBeSlayedList = new List<Buff>();

        public static void RegisterUnitBeSlayed(Buff buff)
        {
            if (!mUnitBeSlayedList.Contains(buff))
            {
                mUnitBeSlayedList.Add(buff);
            }
            else
            {
                throw new Exception("this buff has been registered!");
            }
        }
        public static void UnregisterUnitBeSlayed(Buff buff)
        {
            if (mUnitBeSlayedList.Contains(buff))
            {
                mUnitBeSlayedList.Remove(buff);
            }
            else
            {
                throw new Exception("this buff has not been registered!");
            }
        }
        /// <summary>
        /// 单位被击杀
        /// </summary>
        /// <param name="unit">死亡单位</param>
        /// <param name="slayer">攻击者</param>
        public static void OnUnitBeSlayed(UnitBase unit, UnitBase slayer)
        {

#if DEBUG
            Logger.LogFormat("OnUnitBeSlayed unit = {0} slayer = {1} ", unit.ID, slayer.ID);
#endif
            for (int i = mUnitBeSlayedList.Count - 1; i >= 0; --i)
            {
                mUnitBeSlayedList[i].OnUnitBeSlayed(unit, slayer);
            }
        }

        private static List<Buff> mUnitBeSummonedList = new List<Buff>();

        public static void RegisterUnitBeSummoned(Buff buff)
        {
            if (!mUnitBeSummonedList.Contains(buff))
            {
                mUnitBeSummonedList.Add(buff);
            }
            else
            {
                throw new Exception("this buff has been registered!");
            }
        }
        public static void UnregisterUnitBeSummoned(Buff buff)
        {
            if (mUnitBeSummonedList.Contains(buff))
            {
                mUnitBeSummonedList.Remove(buff);
            }
            else
            {
                throw new Exception("this buff has not been registered!");
            }
        }
        /// <summary>
        /// 单位被召唤
        /// </summary>
        /// <param name="unit">召唤物</param>
        /// <param name="summoner">召唤者</param>
        public static void OnUnitBeSummoned(UnitBase unit, UnitBase summoner)
        {

#if DEBUG
            Logger.LogFormat("OnUnitBeSummoned unit = {0} summoner = {1} ", unit.ID, summoner.ID);
#endif
            for (int i = mUnitBeSummonedList.Count - 1; i >= 0; --i)
            {
                mUnitBeSummonedList[i].OnUnitBeSummoned(unit, summoner);
            }
        }

        private static List<Buff> mUnitWillHurtList = new List<Buff>();

        public static void RegisterUnitWillHurt(Buff buff)
        {
            if (!mUnitWillHurtList.Contains(buff))
            {
                mUnitWillHurtList.Add(buff);
            }
            else
            {
                throw new Exception("this buff has been registered!");
            }
        }
        public static void UnregisterUnitWillHurt(Buff buff)
        {
            if (mUnitWillHurtList.Contains(buff))
            {
                mUnitWillHurtList.Remove(buff);
            }
            else
            {
                throw new Exception("this buff has not been registered!");
            }
        }
        /// <summary>
        /// 单位即将受到伤害
        /// </summary>
        /// <param name="injured">受伤者</param>
        /// <param name="assailant">攻击者</param>
        /// <param name="value">伤害值</param>
        /// <param name="dt">伤害类型</param>
        /// <param name="isAttack">是否来自普通攻击</param>
        /// <returns>修正值</returns>
        public static int OnUnitWillHurt(UnitBase injured, UnitBase assailant, int value, DamageType dt, bool isAttack)
        {

#if DEBUG
            Logger.LogFormat("OnUnitWillHurt injured = {0} assailant = {1} value = {2} dt ={3} isAttack = {4}", injured.ID, assailant.ID,value,dt,isAttack);
#endif
            int offset = 0;
            for (int i = mUnitWillHurtList.Count - 1; i >= 0; --i)
            {
                offset += mUnitWillHurtList[i].OnUnitWillHurt(injured, assailant, value, dt, isAttack);
            }
            return offset;
        }


        private static List<Buff> mUnitBeHurtedList = new List<Buff>();

        public static void RegisterUnitBeHurted(Buff buff)
        {
            if (!mUnitBeHurtedList.Contains(buff))
            {
                mUnitBeHurtedList.Add(buff);
            }
            else
            {
                throw new Exception("this buff has been registered!");
            }
        }
        public static void UnregisterUnitBeHurted(Buff buff)
        {
            if (mUnitBeHurtedList.Contains(buff))
            {
                mUnitBeHurtedList.Remove(buff);
            }
            else
            {
                throw new Exception("this buff has not been registered!");
            }
        }
        /// <summary>
        /// 单位受到伤害
        /// </summary>
        /// <param name="injured">受伤者</param>
        /// <param name="assailant">攻击者</param>
        /// <param name="value">伤害值</param>
        /// <param name="dt">伤害类型</param>
        /// <param name="isAttack">是否来自普通攻击</param>
        public static void OnUnitBeHurted(UnitBase injured, UnitBase assailant, int value, DamageType dt, bool isAttack)
        {

#if DEBUG
            Logger.LogFormat("OnUnitBeHurted injured = {0} assailant = {1} value = {2} dt ={3} isAttack = {4}", injured.ID, assailant.ID, value, dt, isAttack);
#endif
            for (int i = mUnitBeHurtedList.Count - 1; i >= 0; --i)
            {
                mUnitBeHurtedList[i].OnUnitBeHurted(injured, assailant, value, dt, isAttack);
            }
        }

        private static List<Buff> mUnitWillHealList = new List<Buff>();

        public static void RegisterUnitWillHeal(Buff buff)
        {
            if (!mUnitWillHealList.Contains(buff))
            {
                mUnitWillHealList.Add(buff);
            }
            else
            {
                throw new Exception("this buff has been registered!");
            }
        }
        public static void UnregisterUnitWillHeal(Buff buff)
        {
            if (mUnitWillHealList.Contains(buff))
            {
                mUnitWillHealList.Remove(buff);
            }
            else
            {
                throw new Exception("this buff has not been registered!");
            }
        }
        /// <summary>
        /// 单位即将受到治疗
        /// </summary>
        /// <param name="injured">受伤者</param>
        /// <param name="healer">治疗者</param>
        /// <param name="value">治疗值</param>
        /// <returns>修正值</returns>
        public static int OnUnitWillHeal(UnitBase injured, UnitBase healer, int value)
        {

#if DEBUG
            Logger.LogFormat("OnUnitBeHurted injured = {0} healer = {1} value = {2}", injured.ID, healer.ID, value);
#endif
            int offset = 0;
            for (int i = mUnitWillHealList.Count - 1; i >= 0; --i)
            {
                offset += mUnitWillHealList[i].OnUnitWillHeal(injured, healer, value);
            }
            return offset;
        }

        private static List<Buff> mUnitBeHealedList = new List<Buff>();

        public static void RegisterUnitBeHealed(Buff buff)
        {
            if (!mUnitBeHealedList.Contains(buff))
            {
                mUnitBeHealedList.Add(buff);
            }
            else
            {
                throw new Exception("this buff has been registered!");
            }
        }
        public static void UnregisterUnitBeHealed(Buff buff)
        {
            if (mUnitBeHealedList.Contains(buff))
            {
                mUnitBeHealedList.Remove(buff);
            }
            else
            {
                throw new Exception("this buff has not been registered!");
            }
        }
        /// <summary>
        /// 单位受到治疗
        /// </summary>
        /// <param name="injured">受伤者</param>
        /// <param name="healer">治疗者</param>
        /// <param name="value">治疗值</param>
        public static void OnUnitBeHealed(UnitBase injured, UnitBase healer, int value)
        {
#if DEBUG
            Logger.LogFormat("OnUnitBeHealed injured = {0} healer = {1} value = {2}",injured.ID,healer.ID,value);
#endif
            for (int i = mUnitBeHealedList.Count - 1; i >= 0; --i)
            {
                mUnitBeHealedList[i].OnUnitBeHealed(injured, healer, value);
            }
        }


        private static List<Buff> mUnitCastSpellList = new List<Buff>();

        public static void RegisterUnitCastSpell(Buff buff)
        {
            if (!mUnitCastSpellList.Contains(buff))
            {
                mUnitCastSpellList.Add(buff);
            }
            else
            {
                throw new Exception("this buff has been registered!");
            }
        }
        public static void UnregisterUnitCastSpell(Buff buff)
        {
            if (mUnitCastSpellList.Contains(buff))
            {
                mUnitCastSpellList.Remove(buff);
            }
            else
            {
                throw new Exception("this buff has not been registered!");
            }
        }
        /// <summary>
        /// 单位释放法术
        /// </summary>
        /// <param name="unit">施法单位</param>
        /// <param name="skill">技能</param>
        public static void OnUnitCastSpell(UnitBase unit, Skill skill)
        {

#if DEBUG
            Logger.LogFormat("OnUnitCastSpell unit = {0} skill = {1} ", unit.ID, skill.TemplateID);
#endif
            for (int i = mUnitCastSpellList.Count - 1; i >= 0; --i)
            {
                mUnitCastSpellList[i].OnUnitCastSpell(unit, skill);
            }
        }


        private static List<Buff> mSpellHitList = new List<Buff>();

        public static void RegisterSpellHit(Buff buff)
        {
            if (!mSpellHitList.Contains(buff))
            {
                mSpellHitList.Add(buff);
            }
            else
            {
                throw new Exception("this buff has been registered!");
            }
        }
        public static void UnregisterSpellHit(Buff buff)
        {
            if (mSpellHitList.Contains(buff))
            {
                mSpellHitList.Remove(buff);
            }
            else
            {
                throw new Exception("this buff has not been registered!");
            }
        }
        /// <summary>
        /// 法术命中目标
        /// </summary>
        /// <param name="unit">目标</param>
        /// <param name="caster">施法者</param>
        /// <param name="skill">技能</param>
        public static void OnSpellHit(UnitBase unit, UnitBase caster, Skill skill, bool killed)
        {

#if DEBUG
            Logger.LogFormat("OnSpellHit unit = {0} caster = {1} skill = {2} killed = {3}", unit.ID,caster.ID, skill.TemplateID,killed);
#endif
            for (int i = mSpellHitList.Count - 1; i >= 0; --i)
            {
                mSpellHitList[i].OnSpellHit(unit, caster, skill, killed);
            }
        }


    }
}
