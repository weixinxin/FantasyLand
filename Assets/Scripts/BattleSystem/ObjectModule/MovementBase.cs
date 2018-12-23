using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public abstract class MovementBase
    {
        public MovementBase() { }

        protected IMovable Owner;

        public MovementBase(IMovable obj)
        {
            Owner = obj;
        }
        public abstract bool Update(float dt);
    }
}
