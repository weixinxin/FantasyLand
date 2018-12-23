//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorTrueNode : BehaviorNode
    {
		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitTrueNode (this);
		}

		public override string Name {
			get {
				return "True";
			}
		}

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            return BehaviorResult.success;
        }
    }
}
