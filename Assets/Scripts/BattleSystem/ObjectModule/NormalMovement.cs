using System;

namespace BattleSystem.ObjectModule
{
    public class NormalMovement : MovementBase
    {
        vector3 Target;

        public vector3 shift{get;private set;}
        public NormalMovement(IMovable owner)
        {
            Owner = owner;
        }
        public void Retarget(vector3 pos)
        {
            Target = pos;
            shift = (Target - Owner.position).normalized;

        }
        public override bool Update(float dt)
        {
            var dis = Target - Owner.position;
            if (Owner.acceleration != 0)
                Owner.speed += Owner.acceleration * dt;
            var shift_len = Owner.speed * dt;

            var sqr_length = dis.x * dis.x + dis.y * dis.y;
            if (sqr_length <= shift_len * shift_len)
            {
                Owner.position = Target;
                return true;
            }
            else
            {
                Owner.position += shift * shift_len;
                return false;
            }
        }
    }
}
