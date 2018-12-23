//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorCDTimeNode : BehaviorDecorateNode
    {
        public BehaviorPreconditonEvent OnPrecondition = null;
        public float CDTime = 1f;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitCDTimeNode (this);
		}

		public override string Name {
			get {
				return "CDTime";
			}
		}
        public BehaviorCDTimeNode setTargetNode(BehaviorNode node)
        {
            TargetNode = node;
            return this;
        }

        protected override void Reset()
        {
            base.Reset();
            N = 0.0f;
        }

        private float N = 0.0f;
        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (TargetNode == null)
            {
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
                if (N == 0.0f)
                {
                    N = tree.GameTime;
                }
                while (tree.GameTime - N < CDTime)
                {
                    return BehaviorResult.running;
                }
                BehaviorResult result = TargetNode.Exec(tree);

                if (result == BehaviorResult.success)
                {
                    Reset();
                }
                else if (result == BehaviorResult.failed)
                {
                    Reset();
                }
                else if (result == BehaviorResult.running)
                {
                    return BehaviorResult.running;
                }
                
            }
        }
    }
}
