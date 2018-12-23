//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 直到失败
    /// </summary>
    public class BehaviorUntilFailedNode : BehaviorDecorateNode
    {
        public BehaviorPreconditonEvent OnPrecondition;

        public BehaviorUntilFailedNode setTargetNode(BehaviorNode node)
        {
            TargetNode = node;
            return this;
        }
		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitUntilFailedNode (this);
		}

		public override string Name {
			get {
				return "UntilFailed";
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

                if (result == BehaviorResult.failed)
                {
                    SetFailed();
                    return BehaviorResult.failed;                    
                }
                else if (result == BehaviorResult.running)
                {
                     return BehaviorResult.running;
                }
                else if (result == BehaviorResult.success)
                {
                    return BehaviorResult.running;              
                }
                
            }
        }
    }
}