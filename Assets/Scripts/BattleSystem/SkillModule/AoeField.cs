using BattleSystem.ObjectModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.SkillModule
{
    public class BuffEmitter
    {
        public AoeFilter filter;
        public int[] buffs;
        public UnitBase Caster;
        public Predicate<UnitBase> condition
        {
            get
            {
                switch(filter)
                {
                    case AoeFilter.kAll:
                        return (obj) => true;
                    case AoeFilter.kAlly:
                        return (obj) => obj.CampID == Caster.CampID;
                    case AoeFilter.kEnemy:
                        return (obj) => obj.CampID != Caster.CampID;
                    default:
                        Logger.LogError("undifined filter " + filter.ToString());
                        return null;
                }
            }
        }

        public BuffEmitter Copy()
        {
            BuffEmitter emitter = new BuffEmitter();
            emitter.buffs = new int[buffs.Length];
            for(int i = 0;i< buffs.Length;++i)
            {
                emitter.buffs[i] = buffs[i];
            }
            emitter.filter = filter;
            return emitter;
        }
    }
    public class AoeField
    {

        /// <summary>
        /// 施法者
        /// </summary>
        public UnitBase Caster { get; private set; }

        /// <summary>
        /// 触发间隔
        /// </summary>
        public float Interval { get; private set; }

        private float mCdTime = 0;
        /// <summary>
        /// 持续时间
        /// </summary>
        public float Duration { get; private set; }
        private float mElapseTime = 0;


        private AoeRegion mRegion;

        private List<BuffEmitter> BuffEmitters;

        public AoeField(UnitBase caster, AoeRegion region, float duration, float interval, List<BuffEmitter> emitters)
        {
            Caster = caster;
            mRegion = region;
            Interval = interval;
            Duration = duration;
            mElapseTime = 0;
            mCdTime = 0;
            for(int i = 0;i<emitters.Count;++i)
            {
                emitters[i].Caster = caster;
            }
            BuffEmitters = emitters;
        }

        public bool Update(float dt)
        {
            mElapseTime += dt;
            if (mCdTime > 0)
            {
                mCdTime -= dt;
            }
            else
            {
                Execute();
                mCdTime = Interval;
            }
            return mElapseTime > Duration;
        }

        private void Execute()
        {
            for(var i = 0; i < BuffEmitters.Count;++i)
            {
                var BuffEmitter = BuffEmitters[i];
                List<UnitBase> ls = new List<UnitBase>();
                if (mRegion.Select(ls, BuffEmitter.condition))
                {
                    for (var idx = 0; idx < ls.Count; ++idx)
                    {
                        UnitBase unit = ls[idx] as UnitBase;
                        if(unit != null)
                        {
                            for (var n = 0; n < BuffEmitter.buffs.Length; ++n)
                            {
                                unit.AddBuff(BuffEmitter.buffs[n], Caster);
                            }
                        }
                    }
                }

            }
        }
    }
}
