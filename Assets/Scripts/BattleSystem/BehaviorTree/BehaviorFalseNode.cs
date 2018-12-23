//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorFalseNode : BehaviorNode
    {
		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitFalseNode (this);
		}

		public override string Name {
			get {
				return "False";
			}
		}

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            return BehaviorResult.failed;
        }
    }
}
