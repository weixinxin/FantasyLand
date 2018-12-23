using BattleSystem.SkillModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    /// <summary>
    /// 跟踪子弹
    /// </summary>
    public  class TrackBullet : BulletBase
    {
        UnitBase Target;
        private vector3 targetPos;

        TrackMovement Movement;
        public TrackBullet(UnitBase shooter, UnitBase target)
        {
            ID = id;
            Shooter = shooter;
            Target = target;
            Movement = new TrackMovement(this);
            Movement.Retarget(target);
        }

        public override void InitDamage(int value, DamageType dt, bool isAttack)
        {
            base.InitDamage(value, dt, isAttack);
            if(isAttack)
            {
                Target.addExpectedDamage(damage);
            }
        }
        public override bool Update(float dt)
        {
            if(Movement.Update(dt))
            {
                //普攻伤害
                if (damage > 0)
                {
                    if (isAttack)
                    {
                        Target.removeExpectedDamage(damage);
                    }
                    Target.LostHP(damage, Shooter, damageType, isAttack);
                }
                //给目标上buff
                if (buffs != null)
                {
                    for(int i = 0;i< buffs.Length;++i)
                    {
                        Target.AddBuff(buffs[i], Shooter);
                    }
                }
                //AOE
                if (aoeRadius > 0)
                {
                    AoeRegion region = new CircleRegion(Campaign.Instance.world, position.x, position.y, aoeRadius);
                    AoeField aoe = new AoeField(Shooter, region, duration, interval, emitters);
                    Campaign.Instance.AddAoeField(aoe);
                }
                return true;
            }
            return false;
        }
    }
}
