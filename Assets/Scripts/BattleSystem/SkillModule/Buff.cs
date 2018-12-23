
using BattleSystem.ObjectModule;
using System.Collections.Generic;
using System;
using BattleSystem.Config;
using LuaInterface;
namespace BattleSystem.SkillModule
{

    /// <summary>
    /// 附加状态
    /// </summary>
    public class Buff
    {
        private static int s_id = 0;
        private static int id
        {
            get
            {
                return s_id++;
            }
        }
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public int ID { get;private set; }
        private int mTemplateID;
        /// <summary>
        /// 所属类别组
        /// </summary>
        public int Groups { get; private set; }

        /// <summary>
        /// 排斥的组
        /// </summary>
        private int mRejectionGroups = 0;


        /// <summary>
        /// 叠加策略
        /// </summary>
        public OverlayTactics OverlayTactics { get; private set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; private set; }

        /// <summary>
        /// 首次延迟
        /// </summary>
        public float Delay { get; private set; }
        private float mDelay = 0;

        /// <summary>
        /// 循环标记
        /// </summary>
        public bool isLoop { get; private set; }

        /// <summary>
        /// 间隔
        /// </summary>
        public float Duration { get; private set; }
        private float mCfgDuration;
        private float mElapseTime = 0;

        /// <summary>
        /// 生命周期不受父节点影响
        /// </summary>
        public bool isIndividual { get; private set; }
        
        /// <summary>
        /// 是否可移除
        /// </summary>
        public bool isClearable { get; private set; }

        /// <summary>
        /// 是否是负面影响
        /// </summary>
        public bool isNegative { get; private set; }

        /// <summary>
        /// buff持有者
        /// </summary>
        public UnitBase Owner { get; private set; }

        /// <summary>
        /// buff施法者
        /// </summary>
        public UnitBase Caster { get; private set; }
        private LuaTable mLuaBuff = null;
        private LuaFunction mOnUnitWillDieHandle = null;
        private LuaFunction mOnUnitBeSlayedHandle = null;
        private LuaFunction mOnUnitBeSummonedHandle = null;
        private LuaFunction mOnUnitWillHurtHandle = null;
        private LuaFunction mOnUnitBeHurtedHandle = null;
        private LuaFunction mOnUnitWillHealHandle = null;
        private LuaFunction mOnUnitBeHealedHandle = null;
        private LuaFunction mOnUnitCastSpellHandle = null;
        private LuaFunction mOnSpellHitHandle = null;


        protected BuffEffect[] mEffects;

        protected List<BuffEffect> mRunningEffects = new List<BuffEffect>();
        protected List<BuffEffect> mRunningScriptEffects = new List<BuffEffect>(); 
        protected int mPauseCount = 0;
        public Buff(int templateID, UnitBase owner, UnitBase caster)
        {
            this.mTemplateID = templateID;
            this.Owner = owner;
            this.Caster = caster;
            this.ID = id;
            mPauseCount = 0;
            var config = ConfigData.Buff.getRow(templateID);
            bool isScriptBuff = config.isScriptBuff; //是否有脚本
            this.Groups = config.Groups;
            this.mRejectionGroups = config.RejectionGroups;
            this.OverlayTactics = config.OverlayTactics;
            this.Desc = config.Desc;
            this.Delay = config.Delay;
            this.isLoop = config.isLoop;
            this.mCfgDuration = config.Duration;
            this.isIndividual = config.isIndividual;
            this.isClearable = config.isClearable;
            this.isNegative = config.isNegative;
            if (config.BuffEffect != null)
                this.mEffects = new BuffEffect[config.BuffEffect.Length];
            else
                this.mEffects = new BuffEffect[0];
            for (int i = 0; i < this.mEffects.Length; ++i)
            {
                this.mEffects[i] = new BuffEffect(config.BuffEffect[i]);
            }
            if (isScriptBuff)
            {
                var lua = LuaClient.GetMainState();
                LuaTable buff = lua.Require<LuaTable>(string.Format("buff/buff{0}", templateID));
                if (buff == null)
                {
                    Logger.LogErrorFormat("load buff/buff{0}.lua failed!", templateID);
                }
                else
                {

                    LuaFunction newFunc = buff["new"] as LuaFunction;
                    newFunc.BeginPCall();
                    newFunc.PCall();
                    mLuaBuff = (LuaTable)newFunc.CheckLuaTable();
                    newFunc.EndPCall();

                    LuaFunction init = mLuaBuff["init"] as LuaFunction;
                    if (init != null)
                    {
                        init.BeginPCall();
                        init.Push(mLuaBuff);
                        init.Push(id);
                        init.Push(templateID);
                        init.Push(owner);
                        init.Push(caster);
                        init.PCall();
                        init.EndPCall();
                    }
                    else
                    {
                        Logger.LogErrorFormat("buff/buff{0}.lua dont find init function !", templateID);
                    }
                    mOnUnitWillDieHandle = mLuaBuff["OnUnitWillDie"] as LuaFunction;
                    if (mOnUnitWillDieHandle != null)
                        BuffManager.RegisterUnitWillDie(this);
                    mOnUnitBeSlayedHandle = mLuaBuff["OnUnitBeSlayed"] as LuaFunction;
                    if (mOnUnitBeSlayedHandle != null)
                        BuffManager.RegisterUnitBeSlayed(this);
                    mOnUnitBeSummonedHandle = mLuaBuff["OnUnitBeSummoned"] as LuaFunction;
                    if (mOnUnitBeSummonedHandle != null)
                        BuffManager.RegisterUnitBeSummoned(this);
                    mOnUnitWillHurtHandle = mLuaBuff["OnUnitWillHurt"] as LuaFunction;
                    if (mOnUnitWillHurtHandle != null)
                        BuffManager.RegisterUnitWillHurt(this);
                    mOnUnitBeHurtedHandle = mLuaBuff["OnUnitBeHurted"] as LuaFunction;
                    if (mOnUnitBeHurtedHandle != null)
                        BuffManager.RegisterUnitBeHurted(this);
                    mOnUnitWillHealHandle = mLuaBuff["OnUnitWillHeal"] as LuaFunction;
                    if (mOnUnitWillHealHandle != null)
                        BuffManager.RegisterUnitWillHeal(this);
                    mOnUnitBeHealedHandle = mLuaBuff["OnUnitBeHealed"] as LuaFunction;
                    if (mOnUnitBeHealedHandle != null)
                        BuffManager.RegisterUnitBeHealed(this);
                    mOnUnitCastSpellHandle = mLuaBuff["OnUnitCastSpell"] as LuaFunction;
                    if (mOnUnitCastSpellHandle != null)
                        BuffManager.RegisterUnitCastSpell(this);
                    mOnSpellHitHandle = mLuaBuff["OnSpellHit"] as LuaFunction;
                    if (mOnSpellHitHandle != null)
                        BuffManager.RegisterSpellHit(this);
                }
            }


            this.Duration = this.mCfgDuration;
            this.mDelay = this.Delay;
            if (mDelay == 0)
                Apply();
            mElapseTime = 0;
        }

        /// <summary>
        /// buff互斥检查
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="Groups"></param>
        /// <returns></returns>
        public bool MutexCheck(int Groups)
        {
            //优先检查排斥组
            return (mRejectionGroups & Groups) > 0;
        }

        private bool mRecycle = false;
        /// <summary>
        /// 叠加
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        public bool OverlayCheck(int templateID)
        {

            if (templateID == mTemplateID)
            {
                switch (OverlayTactics)
                {
                    case OverlayTactics.kAddTime:
                        if (!isLoop)
                        {
                            Duration += mCfgDuration;
                        }
                        return true;
                    case OverlayTactics.kSingleton:
                        return true;
                    case OverlayTactics.kCoexist:
                        return false;
                    case OverlayTactics.kReplace:
                        mRecycle = true;
                        return false;

                }
            }
            return false;
        }


        public virtual void Destroy()
        {
            Clear();
            for (int i = 0; i < mRunningScriptEffects.Count; ++i)
            {
                mRunningScriptEffects[i].Clear(Owner);
            }
            mRunningScriptEffects.Clear();

            if (mOnUnitWillDieHandle != null)
            {
                mOnUnitWillDieHandle.Dispose();
                mOnUnitWillDieHandle = null;
                BuffManager.UnregisterUnitWillDie(this);
            }
            if (mOnUnitBeSlayedHandle != null)
            {
                mOnUnitBeSlayedHandle.Dispose();
                mOnUnitBeSlayedHandle = null;
                BuffManager.UnregisterUnitBeSlayed(this);
            }
            if (mOnUnitBeSummonedHandle != null)
            {
                mOnUnitBeSummonedHandle.Dispose();
                mOnUnitBeSummonedHandle = null;
                BuffManager.UnregisterUnitBeSummoned(this);
            }
            if (mOnUnitWillHurtHandle != null)
            {
                mOnUnitWillHurtHandle.Dispose();
                mOnUnitWillHurtHandle = null;
                BuffManager.UnregisterUnitWillHurt(this);
            }
            if (mOnUnitBeHurtedHandle != null)
            {
                mOnUnitBeHurtedHandle.Dispose();
                mOnUnitBeHurtedHandle = null;
                BuffManager.UnregisterUnitBeHurted(this);
            }
            if (mOnUnitWillHealHandle != null)
            {
                mOnUnitWillHealHandle.Dispose();
                mOnUnitWillHealHandle = null;
                BuffManager.UnregisterUnitWillHeal(this);
            }
            if (mOnUnitBeHealedHandle != null)
            {
                mOnUnitBeHealedHandle.Dispose();
                mOnUnitBeHealedHandle = null;
                BuffManager.UnregisterUnitBeHealed(this);
            }
            if (mOnUnitCastSpellHandle != null)
            {
                mOnUnitCastSpellHandle.Dispose();
                mOnUnitCastSpellHandle = null;
                BuffManager.UnregisterUnitCastSpell(this);
            }
            if (mOnSpellHitHandle != null)
            {
                mOnSpellHitHandle.Dispose();
                mOnSpellHitHandle = null;
                BuffManager.UnregisterSpellHit(this);
            }
        }  
        /// <summary>
        /// buf默认给宿主施加
        /// </summary>
        protected virtual void Apply()
        {
            if (Owner.IsDead) return;
            for (int i = 0; i < mEffects.Length; ++i)
            {
                if(mEffects[i].Apply(Owner,Caster))
                {
                    //需要结束后移除的效果才添加进队列 
                    mRunningEffects.Add(mEffects[i]);
                }
            }
        }
        
        /// <summary>
        /// 更新buff
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>是否结束</returns>
        public virtual bool Update(float dt)
        {
            if (mRecycle) return true;
            //检查buff效果是否结束
            if(mDelay > 0)
            {
                mDelay -= dt;
                if(mDelay <= 0)
                {
                    Apply();
                }
            }
            else if (isLoop)
            {
                mElapseTime += dt;
                if(mElapseTime >= Duration)
                {
                    Clear();
                    //循环buff
                    mElapseTime = 0;
                    Apply();
                }
            }
            else if(mElapseTime < Duration)
            {
                mElapseTime += dt;
                if (mElapseTime >= Duration)
                {
                    Clear();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 清除buff
        /// </summary>
        public virtual void Clear()
        {
            for (int i = 0; i < mRunningEffects.Count; ++i)
            {
                mRunningEffects [i].Clear(Owner);
            }
            mRunningEffects.Clear();
        }
        

        /// <summary>
        /// 暂停buff效果(例如沉默，计数器是针对多个沉默先后施加的情况)
        /// </summary>
        public virtual void Pause()
        {
            if (mPauseCount == 0)
            {
                Clear();
            }
            mPauseCount++;
        }

        /// <summary>
        /// 恢复buff效果(例如沉默效果结束)
        /// </summary>
        public virtual void Resume()
        {
            if (mPauseCount > 0)
            {
                mPauseCount--;
                if (mPauseCount == 0)
                {
                    Apply();
                }
            }
            else
            {
                Logger.LogError("buff is not paused!");
            }
        }
        #region 消息响应

        /// <summary>
        /// 单位即将受到伤害
        /// </summary>
        /// <param name="injured">受伤者</param>
        /// <param name="assailant">攻击者</param>
        /// <param name="value">伤害值</param>
        /// <param name="dt">伤害类型</param>
        /// <param name="isAttack">是否来自普通攻击</param>
        /// <returns>修正值</returns>
        public virtual int OnUnitWillHurt(UnitBase injured, UnitBase assailant, int value, DamageType dt, bool isAttack)
        {
            int offset = 0;
            if (mOnUnitWillHurtHandle != null)
            {
                mOnUnitWillHurtHandle.BeginPCall();
                mOnUnitWillHurtHandle.Push(mLuaBuff);
                mOnUnitWillHurtHandle.Push(injured);
                mOnUnitWillHurtHandle.Push(assailant);
                mOnUnitWillHurtHandle.Push(value);
                mOnUnitWillHurtHandle.Push(dt);
                mOnUnitWillHurtHandle.Push(isAttack);
                mOnUnitWillHurtHandle.PCall();
                offset = (int)mOnUnitWillHurtHandle.CheckNumber();
                mOnUnitWillHurtHandle.EndPCall();
            }
            else
                throw new Exception("undefined OnUnitWillDieHandle");
            return offset;
        }

        /// <summary>
        /// 单位受到伤害
        /// </summary>
        /// <param name="injured">受伤者</param>
        /// <param name="assailant">攻击者</param>
        /// <param name="value">伤害值</param>
        /// <param name="dt">伤害类型</param>
        /// <param name="isAttack">是否来自普通攻击</param>
        public virtual void OnUnitBeHurted(UnitBase injured, UnitBase assailant, int value, DamageType dt, bool isAttack)
        {
            if (mOnUnitBeHurtedHandle != null)
            {
                mOnUnitBeHurtedHandle.BeginPCall();
                mOnUnitBeHurtedHandle.Push(mLuaBuff);
                mOnUnitBeHurtedHandle.Push(injured);
                mOnUnitBeHurtedHandle.Push(assailant);
                mOnUnitBeHurtedHandle.Push(value);
                mOnUnitBeHurtedHandle.Push(dt);
                mOnUnitBeHurtedHandle.Push(isAttack);
                mOnUnitBeHurtedHandle.PCall();
                mOnUnitBeHurtedHandle.EndPCall();
            }
            else
                throw new Exception("undefined OnUnitBeHurtedHandle");
        }
        
        /// <summary>
        /// 单位即将受到治疗
        /// </summary>
        /// <param name="injured">受伤者</param>
        /// <param name="healer">治疗者</param>
        /// <param name="value">治疗值</param>
        /// <returns>修正值</returns>
        public virtual int OnUnitWillHeal(UnitBase injured, UnitBase healer, int value)
        {
            if (mOnUnitWillHealHandle != null)
            {
                mOnUnitWillHealHandle.BeginPCall();
                mOnUnitWillHealHandle.Push(mLuaBuff);
                mOnUnitWillHealHandle.Push(injured);
                mOnUnitWillHealHandle.Push(healer);
                mOnUnitWillHealHandle.Push(value);
                mOnUnitWillHealHandle.PCall();
                mOnUnitWillHealHandle.EndPCall();
            }
            else
                throw new Exception("undefined OnUnitWillHeal");
            return 0;
        }
        /// <summary>
        /// 单位受到治疗
        /// </summary>
        /// <param name="injured">受伤者</param>
        /// <param name="healer">治疗者</param>
        /// <param name="value">治疗值</param>
        public virtual void OnUnitBeHealed(UnitBase injured, UnitBase healer, int value)
        {

            if (mOnUnitBeHealedHandle != null)
            {
                mOnUnitBeHealedHandle.BeginPCall();
                mOnUnitBeHealedHandle.Push(mLuaBuff);
                mOnUnitBeHealedHandle.Push(injured);
                mOnUnitBeHealedHandle.Push(healer);
                mOnUnitBeHealedHandle.Push(value);
                mOnUnitBeHealedHandle.PCall();
                mOnUnitBeHealedHandle.EndPCall();
            }
            else
                throw new Exception("undefined OnUnitBeHealed");
        }

        /// <summary>
        /// 单位被召唤
        /// </summary>
        /// <param name="unit">召唤物</param>
        /// <param name="summoner">召唤者</param>
        public virtual void OnUnitBeSummoned(UnitBase unit, UnitBase summoner)
        {

            if (mOnUnitBeSummonedHandle != null)
            {
                mOnUnitBeSummonedHandle.BeginPCall();
                mOnUnitBeSummonedHandle.Push(mLuaBuff);
                mOnUnitBeSummonedHandle.Push(unit);
                mOnUnitBeSummonedHandle.Push(summoner);
                mOnUnitBeSummonedHandle.PCall();
                mOnUnitBeSummonedHandle.EndPCall();
            }
            else
                throw new Exception("undefined OnUnitBeSummoned");
        }

        /// <summary>
        /// 单位被击杀
        /// </summary>
        /// <param name="unit">死亡单位</param>
        /// <param name="slayer">攻击者</param>
        public virtual void OnUnitBeSlayed(UnitBase unit, UnitBase slayer)
        {

            if (mOnUnitBeSlayedHandle != null)
            {
                mOnUnitBeSlayedHandle.BeginPCall();
                mOnUnitBeSlayedHandle.Push(mLuaBuff);
                mOnUnitBeSlayedHandle.Push(unit);
                mOnUnitBeSlayedHandle.Push(slayer);
                mOnUnitBeSlayedHandle.PCall();
                mOnUnitBeSlayedHandle.EndPCall();
            }
            else
                throw new Exception("undefined OnUnitBeSlayed");
        }

        /// <summary>
        /// 单位濒临死亡
        /// </summary>
        /// <param name="unit">死亡单位</param>
        /// <param name="slayer">攻击者</param>
        /// <returns>是否进入死亡</returns>
        public virtual bool OnUnitWillDie(UnitBase unit, UnitBase slayer)
        {
            bool shouldDie = true;
            if (mOnUnitWillDieHandle != null)
            {
                mOnUnitWillDieHandle.BeginPCall();
                mOnUnitWillDieHandle.Push(mLuaBuff);
                mOnUnitWillDieHandle.Push(unit);
                mOnUnitWillDieHandle.Push(slayer);
                mOnUnitWillDieHandle.PCall();
                shouldDie = mOnUnitWillDieHandle.CheckBoolean();
                mOnUnitWillDieHandle.EndPCall();
            }
            else
                throw new Exception("undefined OnUnitWillDie");
            return shouldDie;
        }

        /// <summary>
        /// 单位释放法术
        /// </summary>
        /// <param name="unit">施法单位</param>
        /// <param name="skill">技能</param>
        public virtual void OnUnitCastSpell(UnitBase unit, Skill skill)
        {

            if (mOnUnitCastSpellHandle != null)
            {
                mOnUnitCastSpellHandle.BeginPCall();
                mOnUnitCastSpellHandle.Push(mLuaBuff);
                mOnUnitCastSpellHandle.Push(unit);
                mOnUnitCastSpellHandle.Push(skill);
                mOnUnitCastSpellHandle.PCall();
                mOnUnitCastSpellHandle.EndPCall();
            }
            else
                throw new Exception("undefined OnUnitCastSpell");
        }

        /// <summary>
        /// 法术命中目标
        /// </summary>
        /// <param name="unit">目标</param>
        /// <param name="caster">施法者</param>
        /// <param name="skill">技能</param>
        /// <param name="killed">是否击杀</param>
        public virtual void OnSpellHit(UnitBase unit, UnitBase caster, Skill skill, bool killed)
        {

            if (mOnSpellHitHandle != null)
            {
                mOnSpellHitHandle.BeginPCall();
                mOnSpellHitHandle.Push(mLuaBuff);
                mOnSpellHitHandle.Push(unit);
                mOnSpellHitHandle.Push(caster);
                mOnSpellHitHandle.Push(skill);
                mOnSpellHitHandle.Push(killed);
                mOnSpellHitHandle.PCall();
                mOnSpellHitHandle.EndPCall();
            }
            else
                throw new Exception("undefined OnSpellHit");
        }
        #endregion
    }
}