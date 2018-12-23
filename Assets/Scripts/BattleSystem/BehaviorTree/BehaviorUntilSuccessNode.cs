//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 直到成功
    /// </summary>
    public class BehaviorUntilSuccessNode : BehaviorDecorateNode
    {
        public BehaviorPreconditonEvent OnPrecondition;

        public BehaviorUntilSuccessNode setTargetNode(BehaviorNode node)
        {
            TargetNode = node;
            return this;
        }

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitUntilSuccessNode (this);
		}

		public override string Name {
			get {
				return "UntilSuccess";
			}
		}

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (TargetNode == null)
            {
               // Debug.LogError("missing target node");
                return BehaviorResult.failed;               
            }

            if (! Running)
            {
                if (OnPrecondition != null)
                {
                    if (!OnPrecondition())
                    {
                        return BehaviorResult.failed;
                    }
                }

                SetStarting();
            }

            while (true)
            {
                BehaviorResult result = TargetNode.Exec(tree);

                if (result == BehaviorResult.success)
                {
                    SetCompleted();
                    return BehaviorResult.success;                   
                }
                else if (result == BehaviorResult.running)
                {
                    return BehaviorResult.running;
                }
                else if (result == BehaviorResult.failed)
                {
                    return BehaviorResult.running;                   
                }         
               
            }
        }
    }
}