using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public class TrackMovement : MovementBase
    {
        IMovable Target;
        float collision = 0;
        public TrackMovement(IMovable owner)
        {
            Owner = owner;
        }
        public void Retarget(IMovable target,float distance = 0)
        {
            Target = target;
            collision = Owner.radius + target.radius + distance;
        }
        public override bool Update(float dt)
        {
            var dis = Target.position - Owner.position;

            if (Owner.acceleration != 0)
                Owner.speed += Owner.acceleration * dt;
            var shift_len = Owner.speed * dt;

            var sqr_length = dis.x * dis.x + dis.y * dis.y;
            if (sqr_length <= shift_len * shift_len)
            {
                Owner.position = Target.position;
                return true;
            }
            else
            {
                var length = (float)Math.Sqrt(sqr_length);
                if(length > collision)
                {
                    var arg = shift_len / length;
                    Owner.position += dis * arg;
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
