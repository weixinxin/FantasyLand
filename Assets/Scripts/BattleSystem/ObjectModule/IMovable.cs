using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public interface IMovable
    {
        vector3 position
        {
            get;
            set;
        }
        float speed { get; set; }

        float acceleration { get; }


        float radius { get; set; }
    }
}
