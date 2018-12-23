using BattleSystem.ObjectModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace BattleSystem.SkillModule
{
    public abstract class SkillAction
    {
        protected Skill mSkill;
        /// <summary>
        /// 执行动作
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>是否结束动作</returns>
        public abstract bool Execute(float dt);

        public virtual void Reset() { }

        public abstract SkillAction Copy(Skill skill);
    }

    /// <summary>
    /// 等待动作
    /// </summary>
    public class WaitSecondsAction : SkillAction
    {
        private float mElapseTime = 0;
        private float mDuration;

        public WaitSecondsAction(float t)
        {
            mDuration = t;
            mElapseTime = 0;
        }
        public override bool Execute(float dt)
        {
            mElapseTime += dt;
            //Debug.Log("Execute WaitSecondsAction");
            return mElapseTime >= mDuration;
        }
        public override void Reset()
        {
            mElapseTime = 0;
        }

        public override SkillAction Copy(Skill skill)
        {
            WaitSecondsAction action = new WaitSecondsAction(mDuration);
            action.mSkill = skill;
            return action;
        }
    }

    /// <summary>
    /// 序列动作
    /// </summary>
    public class SequenceAction : SkillAction
    {
        private SkillAction[] mActions;
        private int mIndex = 0;
        public SequenceAction(SkillAction[] actions)
        {
            mActions = actions;
            mIndex = 0;
        }
        public override bool Execute(float dt)
        {
            while (mIndex < mActions.Length)
            {
                //Debug.Log("Execute SequenceAction " + mIndex);
                if (mActions[mIndex].Execute(dt))
                {
                    mIndex++;
                }
                else
                {
                    break;
                }
            }
            return mIndex >= mActions.Length;
        }
        public override void Reset()
        {
            mIndex = 0;
            for (int i = 0; i < mActions.Length; ++i)
            {
                mActions[i].Reset();
            }
        }

        public override SkillAction Copy(Skill skill)
        {
            SkillAction[] acts = new SkillAction[mActions.Length];
            for(int i = 0;i < acts.Length;++i)
            {
                acts[i] = mActions[i].Copy(skill);
            }
            var seq = new SequenceAction(acts);
            seq.mSkill = skill;
            return seq;
        }
    }

    /// <summary>
    /// 平行动作
    /// </summary>
    public class ParallelAction : SkillAction
    {
        private SkillAction[] mActions;
        private List<SkillAction> mList = null;
        public ParallelAction(SkillAction[] actions)
        {
            mActions = actions;
            mList = new List<SkillAction>(actions.Length);

            for (int i = actions.Length - 1; i >= 0; --i)
            {
                mList.Add(actions[actions.Length - 1 - i]);
            }
        }
        public override bool Execute(float dt)
        {
            //Debug.Log("Execute ParallelAction");
            for (int i = mList.Count - 1; i >= 0; --i)
            {
                if (mList[i].Execute(dt))
                {
                    mList.RemoveAt(i);
                }
            }
            return mList.Count == 0;
        }
        public override void Reset()
        {
            mList.Clear();
            for (int i = mActions.Length - 1; i >= 0; --i)
            {
                var action = mActions[mActions.Length - 1 - i];
                action.Reset();
                mList.Add(action);
            }
        }

        public override SkillAction Copy(Skill skill)
        {
            SkillAction[] acts = new SkillAction[mActions.Length];
            for (int i = 0; i < acts.Length; ++i)
            {
                acts[i] = mActions[i].Copy(skill);
            }
            var par = new ParallelAction(acts);
            par.mSkill = skill;
            return par;
        }
    }

    /// <summary>
    /// 选择技能目标
    /// </summary>
    public class SelectTargetAction : SkillAction
    {
        public TargetFilter filter;
        public TargetRange range;
        public float radius;

        public override bool Execute(float dt)
        {
            SelectTarget();
            return true;
        }
        public void SelectTarget()
        {
            if (filter == TargetFilter.kSelf)
            {
                mSkill.target = mSkill.Owner;
                return;
            }
            switch (range)
            {
                case TargetRange.kBattlefield:
                    {

                        switch (filter)
                        {
                            case TargetFilter.kLowestHPAlly:
                                mSkill.target = Campaign.Instance.GetLowestHPAlly(mSkill.Owner.ID, mSkill.Owner.CampID);
                                break;
                            case TargetFilter.kLowestHPEnemy:
                                mSkill.target = Campaign.Instance.GetLowestHPEnemy(mSkill.Owner.CampID);
                                break;
                            case TargetFilter.kNearestAlly:
                                mSkill.target = Campaign.Instance.GetNearestAlly(mSkill.Owner.position.x, mSkill.Owner.position.y, mSkill.Owner.ID, mSkill.Owner.CampID);
                                break;
                            case TargetFilter.kNearestEnemy:
                                mSkill.target = Campaign.Instance.GetNearestEnemy(mSkill.Owner.position.x, mSkill.Owner.position.y, mSkill.Owner.ID, mSkill.Owner.CampID);
                                break;
                        }
                    }
                    break;
                case TargetRange.kCirclefield:
                    {
                        List<UnitBase> set = new List<UnitBase>();
                        switch (filter)
                        {
                            case TargetFilter.kNearestEnemy:
                            case TargetFilter.kLowestHPEnemy:
                                Campaign.Instance.world.SelectCircle(mSkill.Owner.position.x, mSkill.Owner.position.y, radius, set, (obj) => obj.CampID != mSkill.Owner.CampID);
                                break;
                            case TargetFilter.kLowestHPAlly:
                            case TargetFilter.kNearestAlly:
                                Campaign.Instance.world.SelectCircle(mSkill.Owner.position.x, mSkill.Owner.position.y, radius, set, (obj) => obj.CampID == mSkill.Owner.CampID);
                                break;
                        }
                        UnitBase unit = null;
                        switch (filter)
                        {
                            case TargetFilter.kNearestAlly:
                            case TargetFilter.kNearestEnemy:
                                float sqr_dis = 0;
                                for (int i = 0; i < set.Count; ++i)
                                {

                                    float dx = set[i].position.x - mSkill.Owner.position.x;
                                    float dy = set[i].position.y - mSkill.Owner.position.y;
                                    var _sqr_dis = dx * dx + dy * dy;
                                    if (unit == null || _sqr_dis < sqr_dis)
                                    {
                                        unit = set[i];
                                        sqr_dis = _sqr_dis;
                                    }
                                }
                                break;
                            case TargetFilter.kLowestHPAlly:
                            case TargetFilter.kLowestHPEnemy:
                                for (int i = 0; i < set.Count; ++i)
                                {
                                    if (unit == null || unit.HP > set[i].HP)
                                    {
                                        unit = set[i];
                                    }
                                }
                                break;
                        }
                        mSkill.target = unit;

                    }
                    break;
                default:
                    break;
            }

#if DEBUG
            Logger.LogFormat("unit {0} skill {1} SelectTarget target = {2}", mSkill.Owner.ID, mSkill.TemplateID, (mSkill.target == null)?"null":mSkill.target.ID.ToString());
#endif
        }

        public override SkillAction Copy(Skill skill)
        {
            SelectTargetAction action = new SelectTargetAction();
            action.filter = this.filter;
            action.radius = this.radius;
            action.range = this.range;
            action.mSkill = skill;
            return action;
        }
    }
   
    /// <summary>
    /// 为单位加buff
    /// </summary>
    public class AddBuffAction : SkillAction
    {
        int templateID;
        public AddBuffAction(int id)
        {
            templateID = id;
        }
        public override bool Execute(float dt)
        {
            if (mSkill.target != null)
            {
#if DEBUG
                Logger.LogFormat("unit {0} skill {1} AddBuffAction to unit {2}", mSkill.Owner.ID, mSkill.TemplateID, mSkill.target.ID);
#endif
                mSkill.target.AddBuff(templateID, mSkill.Owner);
            }
            return true;
        }

        public override SkillAction Copy(Skill skill)
        {
            AddBuffAction act = new AddBuffAction(templateID);
            act.mSkill = skill;
            return act;
        }
    }

    /// <summary>
    /// 释放AOE场
    /// </summary>
    public class AoeFieldAction : SkillAction
    {

        public bool userInput;
        public float duration;
        public float interval;
        public RegionType type;
        public float radius;
        public float width;
        public float height;
        public float theta;

        public List<BuffEmitter> emitters;

        public override bool Execute(float dt)
        {

            float x, y;
            if (userInput)
            {
                if (mSkill.inputX > 0 || mSkill.inputY > 0)
                {
                    x = mSkill.inputX;
                    y = mSkill.inputY;
                }
                else
                {
                    if (mSkill.target == null)
                        return true;
                    x = mSkill.target.position.x;
                    y = mSkill.target.position.y;
                }
            }
            else
            {
                    if (mSkill.target == null)
                        return true;
                x = mSkill.target.position.x;
                y = mSkill.target.position.y;
            }
            AoeRegion region = null;
            switch (type)
            {
                case RegionType.kCircle:
                    {
                        region = new CircleRegion(Campaign.Instance.world, x, y, radius);
                    }
                    break;
                case RegionType.kRect:
                    {
                        var dx = x - mSkill.Owner.position.x;
                        var dy = y - mSkill.Owner.position.y;
                        region = new RectRegion(Campaign.Instance.world, mSkill.Owner.position.x, mSkill.Owner.position.y, dx, dy, width, height);
                    }
                    break;
                case RegionType.kSector:
                    {
                        var dx = x - mSkill.Owner.position.x;
                        var dy = y - mSkill.Owner.position.y;
                        region = new SectorRegion(Campaign.Instance.world, mSkill.Owner.position.x, mSkill.Owner.position.y, dx, dy, radius, theta);
                    }
                    break;
            }
            AoeField aoe = new AoeField(mSkill.Owner, region, duration, interval, emitters);

#if DEBUG
            Logger.LogFormat("unit {0} skill {1} AddAoeField at {2},{3}", mSkill.Owner.ID, mSkill.TemplateID, x,y);
#endif
            Campaign.Instance.AddAoeField(aoe);
            return true;
        }

        public override SkillAction Copy(Skill skill)
        {
            AoeFieldAction act = new AoeFieldAction();
            act.userInput = userInput;
            act.duration = duration;
            act.interval = interval;
            act.type = type;
            act.radius = radius;
            act.width = width;
            act.height = height;
            act.theta = theta;
            act.mSkill =skill;
            act.emitters = new List<BuffEmitter>();
            for (int i = 0; i < emitters.Count;++i)
            {
                act.emitters.Add(emitters[i].Copy());
            }
            return act;
        }
    }

    /// <summary>
    /// 播放角色动画
    /// </summary>
    public class PlayAnimationAction : SkillAction
    {
        private string name;
        private float mElapseTime = 0;
        private float mDuration;

        private bool mExecuted = false;
        public PlayAnimationAction(string name, float duration)
        {
            this.name = name;
            mDuration = duration;
            mElapseTime = 0;
        }
        public override bool Execute(float dt)
        {
            if(!mExecuted)
            {
                mExecuted = true;
                //播放动画

#if DEBUG
                Logger.LogFormat("unit {0} skill {1} PlayAnimation {2}", mSkill.Owner.ID, mSkill.TemplateID, this.name);
#endif
            }
            mElapseTime += dt;
            return mElapseTime >= mDuration;
        }
        public override void Reset()
        {
            mExecuted = false;
            mElapseTime = 0;
        }

        public override SkillAction Copy(Skill skill)
        {
            PlayAnimationAction act = new PlayAnimationAction(this.name, this.mDuration);
            act.mSkill = skill;
            return act;
        }
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    public class PlayEffectAction :SkillAction
    {

        private string name;
        public PlayEffectAction (string name )
        {
            this.name = name;
        }
        public override bool Execute(float dt)
        {
#if DEBUG
            Logger.LogFormat("unit {0} skill {1} PlayEffectAction {2}", mSkill.Owner.ID, mSkill.TemplateID, this.name);
#endif
            //播放特效
            return true;
        }

        public override SkillAction Copy(Skill skill)
        {
            PlayEffectAction act = new PlayEffectAction(name);
            act.mSkill = skill;
            return act;
        }
    }
    
    /// <summary>
    /// 播放音效
    /// </summary>
    public class PlaySoundAction :SkillAction
    {

        private string name;
        public PlaySoundAction(string name)
        {
            this.name = name;
        }
        public override bool Execute(float dt)
        {
#if DEBUG
            Logger.LogFormat("unit {0} skill {1} PlaySoundAction {2}", mSkill.Owner.ID, mSkill.TemplateID, this.name);
#endif
            //播放音效
            return true;
        }

        public override SkillAction Copy(Skill skill)
        {
            PlaySoundAction act = new PlaySoundAction(name);
            act.mSkill = skill;
            return act;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ShootBulletAction :SkillAction
    {
        int templateID;
        public ShootBulletAction(int id)
        {
            this.templateID = id;
        }


        public override bool Execute(float dt)
        {
#if DEBUG
            Logger.LogFormat("unit {0} skill {1} ShootBulletAction {2}", mSkill.Owner.ID, mSkill.TemplateID, this.templateID);
#endif
            Campaign.Instance.ShootBullet(templateID, mSkill.Owner, mSkill.target, false);
            return true;
        }

        public override SkillAction Copy(Skill skill)
        {
            ShootBulletAction act = new ShootBulletAction(this.templateID);
            act.mSkill = skill;
            return act;
        }
    }
    
}