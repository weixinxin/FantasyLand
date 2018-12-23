//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 取反结点
    /// </summary>
    public class BehaviorInvertNode : BehaviorDecorateNode
    {
		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitInvertNode (this);
		}

		public override string Name {
			get {
				return "Invert";
			}
		}

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (TargetNode == null)
            {
                return BehaviorResult.failed;               
            }

            SetStarting();

            BehaviorResult result = TargetNode.Exec(tree);

            if (result == BehaviorResult.success)
            {
                SetFailed();
                return BehaviorResult.failed;               
            }
            else if (result == BehaviorResult.failed)
            {
                SetCompleted();
                return BehaviorResult.success;               
            }
            else if (result == BehaviorResult.running)
            {
                return BehaviorResult.running;
            }

            return BehaviorResult.abort;
        }
    }
}