using BattleSystem.BehaviorTree;
using BattleSystem.Config;
using BattleSystem.SkillModule;
using BattleSystem.SpaceModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace BattleSystem.ObjectModule
{
    public delegate void FloatDelegate(float f);
    public class UnitBase : IMovable
    {


        private WorldSpace mWorldSpace = null;

        internal GridNode mGridNode = null;

        private SkillModule.Skill[] mSkills = null;



        public AnimatorController Animator { get; private set; }
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public int TemplateID { get; private set; }

        /// <summary>
        /// 坐标
        /// </summary>
        public vector3 position
        {
            get
            {
                return _position;
            }
            set
            {

#if DEBUG
                if (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z) || float.IsInfinity(value.x) || float.IsInfinity(value.y) || float.IsInfinity(value.z))
                {
                    Logger.LogErrorFormat("error: {0},{1},{2}", value.x, value.y, value.z);
                    return;
                }
#endif
                _position = value;
                if (!IsDead)
                    OnPositionChanged();
            }
        }
        private vector3 _position = vector3.zero;
        /// <summary>
        /// 护盾
        /// </summary>
        public Shield Shield = new Shield();

        /// <summary>
        /// 血量
        /// </summary>
        public int HP { get; private set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 最大血量
        /// </summary>
        public int MaxHP { get; set; }

        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead { get; private set; }

        private float mDestroyCountdown = 5.0f;

        /// <summary>
        /// 攻击力
        /// </summary>
        public Attribute ATK { get; set; }

        /// <summary>
        /// 移动速度
        /// </summary>
        public Attribute MoveSpeed { get; set; }

        /// <summary>
        /// 攻击间隔
        /// </summary>
        public AttackDuration AttackDuration { get; set; }

        /// <summary>
        /// 攻击距离
        /// </summary>
        public DistanceAttribute AttackRange { get; set; }
        /// <summary>
        /// 视野
        /// </summary>
        public DistanceAttribute VisualRange { get; set; }

        /// <summary>
        /// 阵营ID
        /// </summary>
        public int CampID { get; set; }

        public UnitBase AttackTarget { get; set; }

        /// <summary>
        /// 子弹ID
        /// </summary>
        public int Bullet { get;private set; }

        private int mAttackMissCount = 0;
        private int mMagicDamageImmunityCount = 0;
        private int mNotargetCount = 0;
        private int mPhysicalDamageImmunityCount = 0;
        private int mUnableAttackCount = 0;
        private int mUnableCastCount = 0;
        private int mUnmovableCount = 0;
        private int mDeathlessCount = 0;
        private int mNegativeEffectImmunityCount = 0;


        /// <summary>
        /// 是否处于攻击失效状态
        /// </summary>
        public bool isAttackMiss { get { return mAttackMissCount > 0; } }

        /// <summary>
        /// 是否处于魔法免疫状态
        /// </summary>
        public bool isMagicDamageImmunity { get { return mMagicDamageImmunityCount > 0; } }

        /// <summary>
        /// 是否处于不被选中状态
        /// </summary>
        public bool isNotarget { get { return mNotargetCount > 0; } }

        /// <summary>
        /// 是否处于物理免疫状态
        /// </summary>
        public bool isPhysicalDamageImmunity { get { return mPhysicalDamageImmunityCount > 0; } }

        /// <summary>
        /// 是否处于无法攻击状态
        /// </summary>
        public bool isUnableAttack { get { return mUnableAttackCount > 0; } }

        /// <summary>
        /// 是否处于无法施法状态
        /// </summary>
        public bool isUnableCast { get { return mUnableCastCount > 0; } }

        /// <summary>
        /// 是否处于无法移动状态
        /// </summary>
        public bool isUnmovable { get { return mUnmovableCount > 0; } }

        /// <summary>
        /// 是否处于不死状态
        /// </summary>
        public bool isDeathless { get { return mDeathlessCount > 0; } }


        /// <summary>
        /// 是否免疫负面效果
        /// </summary>
        public bool isNegativeEffectImmunity { get { return mNegativeEffectImmunityCount > 0; } }


        internal List<SkillModule.Buff> Buffs = new List<SkillModule.Buff>();

        /// <summary>
        /// 尝试往单位身上加buff
        /// </summary>
        /// <param name="templateID">模板id</param>
        /// <param name="caster">施法者</param>
        /// <returns>是否成功</returns>

        internal bool AddBuff(int templateID, UnitBase caster)
        {
            if (IsDead) return false;
            int Groups = 1;//读取配置获取所属组
            for (int i = Buffs.Count - 1; i >= 0; --i)
            {
                if (Buffs[i].MutexCheck(Groups))
                    return false;
            }
            for (int i = Buffs.Count - 1; i >= 0; --i)
            {
                if (Buffs[i].OverlayCheck(templateID))
                    return Buffs[i].OverlayTactics == OverlayTactics.kAddTime;
            }
            var buff = new SkillModule.Buff(templateID, this, caster);
            Buffs.Add(buff);
            return true;
        }

        /// <summary>
        /// 加状态
        /// </summary>
        /// <param name="state">状态</param>
        internal void AddState(BuffEffectType state)
        {
            if (IsDead) return;
            switch (state)
            {
                case BuffEffectType.kAttackMiss:
                    mAttackMissCount++;
                    break;
                case BuffEffectType.kMagicDamageImmunity:
                    mMagicDamageImmunityCount++;
                    break;
                case BuffEffectType.kNotarget:
                    mNotargetCount++;
                    break;
                case BuffEffectType.kPhysicalDamageImmunity:
                    mPhysicalDamageImmunityCount++;
                    break;
                case BuffEffectType.kUnableAttack:
                    mUnableAttackCount++;
                    break;
                case BuffEffectType.kUnableCast:
                    mUnableCastCount++;
                    break;
                case BuffEffectType.kUnmovable:
                    mUnmovableCount++;
                    break;
                case BuffEffectType.kDeathless:
                    mDeathlessCount++;
                    break;
                case BuffEffectType.kNegativeEffectImmunity:
                    mNegativeEffectImmunityCount++;
                    break;
            }
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="state">状态</param>
        internal void RemoveState(BuffEffectType state)
        {
            if (IsDead) return;
            switch (state)
            {
                case BuffEffectType.kAttackMiss:
                    Trace.Assert(mAttackMissCount > 0);
                    mAttackMissCount--;
                    break;
                case BuffEffectType.kMagicDamageImmunity:
                    Trace.Assert(mMagicDamageImmunityCount > 0);
                    mMagicDamageImmunityCount--;
                    break;
                case BuffEffectType.kNotarget:
                    Trace.Assert(mNotargetCount > 0);
                    mNotargetCount--;
                    break;
                case BuffEffectType.kPhysicalDamageImmunity:
                    Trace.Assert(mPhysicalDamageImmunityCount > 0);
                    mPhysicalDamageImmunityCount--;
                    break;
                case BuffEffectType.kUnableAttack:
                    Trace.Assert(mUnableAttackCount > 0);
                    mUnableAttackCount--;
                    break;
                case BuffEffectType.kUnableCast:
                    Trace.Assert(mUnableCastCount > 0);
                    mUnableCastCount--;
                    break;
                case BuffEffectType.kUnmovable:
                    Trace.Assert(mUnmovableCount > 0);
                    mUnmovableCount--;
                    break;
                case BuffEffectType.kDeathless:
                    Trace.Assert(mDeathlessCount > 0);
                    mDeathlessCount--;
                    break;
                case BuffEffectType.kNegativeEffectImmunity:
                    Trace.Assert(mNegativeEffectImmunityCount > 0);
                    mNegativeEffectImmunityCount--;
                    break;
            }
        }
        public UnitBase() { }

        private static int s_id = 1;
        private static int id
        {
            get
            {
                return s_id++;
            }
        }
        public UnitBase(WorldSpace ws, int templateID, int campID, int level)
        {
            this.ID = id;
            this.mWorldSpace = ws;
            this.TemplateID = templateID;
            this.CampID = campID;
            this.Level = level;
            OnSpawned();
            var config = ConfigData.Unit.getRow(templateID);
            this.HP = config.MaxHP;
            this.MaxHP = config.MaxHP;
            this.IsDead = false;

            ATK = new Attribute(config.Attack, null);
            radius = config.Radius;
            MoveSpeed = new Attribute(config.MoveSpeed, null);
            AttackDuration = new AttackDuration(config.AttackDuration, null);
            AttackRange = new DistanceAttribute(config.AttackRange, null);
            VisualRange = new DistanceAttribute(config.VisualRange, null);
            Bullet = config.Bullet;
            if (config.Skills != null)
                mSkills = new Skill[config.Skills.Length];
            else
                mSkills = new Skill[0];
            for (int i = 0; i < mSkills.Length; ++i)
            {
                mSkills[i] = new Skill(this, config.Skills[i], level);
            }
            InitController();
            InitBehaviorTree();
            Animator = new AnimatorController(config.Animator);
        }

        /// <summary>
        /// 单位受到治疗
        /// </summary>
        /// <param name="delta">治疗量</param>
        /// <param name="healer">治疗者</param>
        public void AddHP(int delta, UnitBase healer)
        {
            if (IsDead) return;
            var offset = BuffManager.OnUnitWillHeal(this, healer, delta);
            delta += offset;
            if (delta > 0)
            {
                var origin = HP;
                HP = HP + delta;
                if (HP > MaxHP) HP = MaxHP;
                BuffManager.OnUnitBeHealed(this, healer, HP - origin);
            }
#if DEBUG
            Logger.LogFormat("unit {0} HP = {1}", ID, HP);
#endif
        }
        /// <summary>
        /// 单位受到伤害
        /// </summary>
        /// <param name="delta">伤血值</param>
        /// <param name="assailant">攻击者</param>
        /// <param name="dt">伤害类型</param>
        /// <param name="isAttack">是否普攻</param>
        public void LostHP(int delta, UnitBase assailant, DamageType dt, bool isAttack)
        {
            if (IsDead) return;
            if (dt == DamageType.kMagic && isMagicDamageImmunity || dt == DamageType.kPhysical && isPhysicalDamageImmunity)
            {

#if DEBUG
                if (isMagicDamageImmunity)
                {
                    Logger.LogFormat("unit {0} try hurt  unit {1} but unit{1} isMagicDamageImmunity",assailant.ID, ID);
                }
                else
                {
                    Logger.LogFormat("unit {0} try hurt  unit {1} but unit{1} isPhysicalDamageImmunity", assailant.ID, ID);
                }
                
#endif
                return;
            }

#if DEBUG
            float cur_hp = HP;
#endif
            var offset = BuffManager.OnUnitWillHurt(this, assailant, delta, dt, isAttack);
            delta += offset;
            if (delta > 0)
            {
                var _delta = Shield.Consume(delta);
                HP -= _delta;
                BuffManager.OnUnitBeHurted(this, assailant, delta, dt, isAttack);
                if (HP <= 0)
                {
                    if (isDeathless)
                    {
                        HP = 1;
                    }
                    else if (BuffManager.OnUnitWillDie(this, assailant))
                    {
                        HP = 0;
                        OnDead();
                        BuffManager.OnUnitBeSlayed(this, assailant);
                    }
                    else
                    {
                        if (HP <= 0) HP = 1;
                    }
                }
            }

#if DEBUG
            Logger.LogFormat("unit {0} origHP = {1} HP = {2}", ID, cur_hp,HP);
#endif
        }

        public virtual void OnDead()
        {
            IsDead = true;
            Campaign.Instance.OnUnitDie(this);
            BehaviorTree.Stop();
            mDestroyCountdown = 5.0f;
            Animator.Stop();
            TickDispatcher.OnUnitStateChanged(ID, UnitState.kDead);
            //播放死亡动画
        }

        /// <summary>
        /// 更新单位逻辑
        /// </summary>
        /// <param name="dt">时间</param>
        /// <returns>是否可以销毁单位</returns>
        public virtual bool Update(float dt)
        {
            if (IsDead)
            {
                mDestroyCountdown -= dt;
            }
            else
            {
                for (int i = 0; i < mSkills.Length; ++i)
                {
                    mSkills[i].Update(dt);
                }
                //更新并移除已经结束的buff
                for (int i = Buffs.Count - 1; i >= 0; --i)
                {
                    if (Buffs[i].Update(dt))
                    {
                        Buffs[i].Destroy();
                        Buffs.RemoveAt(i);
                    }
                }

            }
            if (BehaviorTree != null && !IsDead)
                BehaviorTree.Exec();
            if (Animator != null)
                Animator.Update(dt);
            return mDestroyCountdown <= 0;
        }

        private int mExpecteddamage = 0;
        public void addExpectedDamage(int value)
        {
            mExpecteddamage += value;
        }
        public void removeExpectedDamage(int value)
        {
            mExpecteddamage -= value;
            if (mExpecteddamage < 0)
                Logger.LogException(new Exception("Expected damage should not less then zero!"));
        }
        public bool isDying
        {
            get
            {
                return HP - mExpecteddamage <= 0;
            }
        }

        public float speed
        {
            get { return MoveSpeed.value; }
            set { }
        }


        public float acceleration
        {
            get { return 0; }
        }



        public float radius
        {
            get;
            set;
        }

        public bool InAttackRange(UnitBase target)
        {
            var dis = this.AttackRange.value + radius + target.radius;
            var dx = target.position.x - position.x;
            var dy = target.position.y - position.y;
            var sqr_dis = dx * dx + dy * dy;
            return sqr_dis <= dis * dis;
        }
        public bool InVisualRange(UnitBase target)
        {
            var sqr = this.VisualRange.sqrValue;
            var dx = target.position.x - position.x;
            var dy = target.position.y - position.y;
            var sqr_dis = dx * dx + dy * dy;
            return sqr_dis <= sqr;
        }

        public UnitController Controller { get; protected set; }
        public BehaviorTreeBase BehaviorTree { get; protected set; }

        protected virtual void InitController()
        {
            Controller = new UnitController(this);
        }
        protected virtual void InitBehaviorTree()
        {
            BehaviorTree = new BehaviorTreeUnit(this);
        }
        protected virtual void OnSpawned()
        {
            TickDispatcher.OnCreateUnit(ID,TemplateID,CampID,Level);
        }
        protected virtual void OnPositionChanged()
        {
            mWorldSpace.UpdateNode(this);
            TickDispatcher.OnUnitPositionChanged(ID, _position.x, _position.y);
        }
        public virtual void OnEnterAttack()
        {
            TickDispatcher.OnUnitStateChanged(ID, UnitState.kAttack, AttackTarget.ID);
        }
        public virtual void OnEnterIdle()
        {
            TickDispatcher.OnUnitStateChanged(ID, UnitState.kIdel, null);
        }
        public virtual void OnEnterMove()
        {
            TickDispatcher.OnUnitStateChanged(ID, UnitState.kMove, null);
        }
    }
}
