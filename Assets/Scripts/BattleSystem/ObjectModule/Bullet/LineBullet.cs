using BattleSystem.SkillModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    /// <summary>
    /// 直线飞行,到终点或者撞到第一个敌人爆炸
    /// </summary>
    public class LineBullet : BulletBase
    {
        
        NormalMovement Movement;

        List<UnitBase> res = new List<UnitBase>();
        public LineBullet(UnitBase shooter, vector3 target)
        {
            this.Shooter = shooter;
            Movement = new NormalMovement(this);
            Movement.Retarget(target);
        }

        public override bool Update(float dt)
        {
            float x = position.x;
            float y = position.y;
            if (Movement.Update(dt))
            {
                //AOE
                if (aoeRadius > 0)
                {
                    AoeRegion region = new CircleRegion(Campaign.Instance.world, position.x, position.y, aoeRadius);
                    AoeField aoe = new AoeField(Shooter, region, duration, interval, emitters);
                    Campaign.Instance.AddAoeField(aoe);
                }
                return true;
            }
            else
            {
                var shift = speed * dt;

                Campaign.Instance.world.SelectRect(x, y, Movement.shift.x, Movement.shift.y, this.radius * 2, shift, res, (obj) => obj.CampID != Shooter.CampID);

                if(res.Count > 0)
                {

                    UnitBase unit = null;
                    float sqr_dis = 0;
                    for (int i = 0; i < res.Count; ++i)
                    {

                        float dx = res[i].position.x - x;
                        float dy = res[i].position.y - y;
                        var _sqr_dis = dx * dx + dy * dy;
                        if (unit == null || _sqr_dis < sqr_dis)
                        {
                            unit = res[i];
                            sqr_dis = _sqr_dis;
                        }
                    }
                    //普攻伤害
                    if (damage > 0)
                        unit.LostHP(damage, Shooter, damageType, isAttack);
                    //给目标上buff
                    if (buffs != null)
                    {
                        for (int i = 0; i < buffs.Length; ++i)
                        {
                            unit.AddBuff(buffs[i], Shooter);
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
            }
            return false;
        }
    }
}
