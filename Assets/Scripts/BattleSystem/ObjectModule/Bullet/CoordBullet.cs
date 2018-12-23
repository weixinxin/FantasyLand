using BattleSystem.SkillModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    /// <summary>
    /// 坐标定位子弹
    /// </summary>
    public  class CoordBullet : BulletBase
    {

        NormalMovement Movement;

        public CoordBullet(UnitBase shooter,vector3 target)
        {
            Movement = new NormalMovement(this);
            Movement.Retarget(target);
        }



        public override bool Update(float dt)
        {
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
            return false;
        }
    }
}
