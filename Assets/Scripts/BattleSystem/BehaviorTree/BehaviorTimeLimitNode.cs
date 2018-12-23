//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 限制执行时间，超时取消
    /// </summary>
    public class BehaviorTimeLimitNode : BehaviorDecorateNode
    {
        public BehaviorPreconditonEvent OnPrecondition;
        public float Interval;

        public BehaviorTimeLimitNode setTargetNode(BehaviorNode node)
        {
            TargetNode = node;
            return this;
        }

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitTimeLimitNode (this);
		}

		public override string Name {
			get {
				return "TimeLimit";
			}
		}
        private float N = 0.0f;

        protected override void Reset()
        {
            base.Reset();
            N = 0.0f;
        }

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (TargetNode == null)
            {
                //Debug.LogError("missing target node");
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

            if (N == 0.0f)
            {
                N = tree.GameTime;
            }
            BehaviorResult result = TargetNode.Exec(tree);

            if (result == BehaviorResult.success)
            {
                SetCompleted();
                N = 0.0f;
                return BehaviorResult.success;                
            }
            else if (result == BehaviorResult.failed)
            {
                SetFailed();
                return BehaviorResult.failed;
            }
            else if (result == BehaviorResult.running)
            {
                if (tree.GameTime - N < Interval)
                {
                    return BehaviorResult.running;
                }
                else
                {
                    TargetNode.Abort();
                    N = 0.0f;
                    return BehaviorResult.failed;
                }
            }
            else {
                return BehaviorResult.abort;
            }
            
        }
    }
}