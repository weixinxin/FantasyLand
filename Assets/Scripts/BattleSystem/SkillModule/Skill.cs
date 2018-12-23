
using BattleSystem.Config;
using BattleSystem.ObjectModule;
using System.Collections.Generic;
namespace BattleSystem.SkillModule
{
    public enum SkillStatus
    {
        kReady,
        kRunning,
        kCoolDown,
    }
    public class Skill
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
        /// 技能持有者
        /// </summary>
        public UnitBase Owner { get; private set; }

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public int TemplateID { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; private set; }

        /// <summary>
        /// 技能消耗值
        /// </summary>
        public int Cost { get; private set; }

        /// <summary>
        /// 技能冷却时间
        /// </summary>
        public float CD { get; private set; }
        protected float mCDTime = 0; 

        /// <summary>
        /// 技能持续时间
        /// </summary>
        public float Duration { get; private set; }
        protected float mSkillElapseTime = 0;
        protected bool mSkillActionsOver = false;

        /// <summary>
        /// 技能等级
        /// </summary>
        public int Level { get;private set; }


        /// <summary>
        /// 是否自动释放(被动技能,无需目标)
        /// </summary>
        public bool AutoCast { get; private set; }

        /// <summary>
        /// 技能状态
        /// </summary>
        public SkillStatus Status { get; private set; }

        /// <summary>
        /// 构造技能
        /// </summary>
        /// <param name="templateID">技能模板ID</param>
        /// <param name="id">技能ID</param>
        /// <param name="level">技能等级</param>
        public Skill(UnitBase owner,int templateID,int level = 1)
        {
            this.Owner = owner;
            this.TemplateID = templateID;
            ID = id;
            var config = ConfigData.Skill.getRow(templateID);
            this.AutoCast = config.AutoCast;
            this.CD = config.CD;
            this.Cost = config.Cost;
            this.Desc = config.Desc;
            this.Duration = config.Duration;
            this.Level = level;
            //通过模板ID获取配置数据
            mAction = SkillManager.LoadSkillAction(templateID,this);

            var cdScale = 0;
            //初始CD
            mCDTime = CD * cdScale;
            Status = mCDTime == 0 ? SkillStatus.kReady : SkillStatus.kCoolDown;
        }

        /// <summary>
        /// 输入的技能目标参数
        /// </summary>
        internal UnitBase target;

        /// <summary>
        /// 输入的技能坐标参数X
        /// </summary>
        internal float inputX = 0;
        /// <summary>
        /// 输入的技能坐标参数Y
        /// </summary>
        internal float inputY = 0;


        
        //internal List<>

        /// <summary>
        /// 技能行为
        /// </summary>
        private SkillAction mAction = null;

        /// <summary>
        /// 释放技能
        /// </summary>
        public void Cast()
        {
            if(Status == SkillStatus.kReady)
            {
#if DEBUG
                Logger.LogFormat("unit {0} cast skill {1}",Owner.ID,TemplateID);
#endif
                mAction.Reset();
                Status = SkillStatus.kRunning;
                mSkillElapseTime = 0;
                mSkillActionsOver = false;
            }
        }

        /// <summary>
        /// 对单位释放技能
        /// </summary>
        /// <param name="target">目标单位</param>
        public void Cast(UnitBase target)
        {
            this.target = target;
            Cast();
        }

        /// <summary>
        /// 对坐标区域释放技能
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Cast(float x,float y)
        {
            this.inputX = x;
            this.inputY = y;
            Cast();
        }

        /// <summary>
        /// 打断技能
        /// </summary>
        public void Abort()
        {
            if (Status == SkillStatus.kRunning)
            {
                OnSkillOver();
            }
        }
        public void OnSkillOver()
        {

            mCDTime = CD;
            Status = SkillStatus.kCoolDown;
        }
        public void Update(float dt)
        {
            switch(Status)
            {
                case SkillStatus.kCoolDown:
                    {
                        mCDTime -= dt;
                        if(mCDTime <= 0)
                        {
                            Status = SkillStatus.kReady;
                        }
                    }
                    break;
                case SkillStatus.kRunning:
                    {
                        mSkillElapseTime += dt;
                        if (!mSkillActionsOver)
                        {

                            if (mAction.Execute(dt))
                            {
                                mSkillActionsOver = true;
                            }
                        }
                        else if (mSkillElapseTime >= Duration)
                        {
#if DEBUG
                            if(mSkillElapseTime - Duration > dt && Duration > 0)
                            {
                                Logger.LogWarningFormat("Skill duration {0} is less than the action time", Duration);
                            }
#endif
                            OnSkillOver();
                        }
                    }
                    break;
                case SkillStatus.kReady:
                    {
                        if(AutoCast)
                        {
                            Cast();
                        }
                    }
                    break;
            }
        }

    }
}