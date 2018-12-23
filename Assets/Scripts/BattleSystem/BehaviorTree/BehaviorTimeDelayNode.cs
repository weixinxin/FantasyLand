//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 延时执行
    /// </summary>
    public class BehaviorTimeDelayNode : BehaviorDecorateNode
    {
        public BehaviorPreconditonEvent OnPrecondition;
        public float Interval;

        public BehaviorTimeDelayNode setTargetNode(BehaviorNode node)
        {
            TargetNode = node;
            return this;
        }

		public override string Name {
			get {
				return "TimeDelay";
			}
		}
        private float N = 0.0f;
		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitTimeDelayNode (this);
		}

        protected override void Reset()
        {
            base.Reset();
            N = 0.0f;
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

            if (N == 0.0f)
            {
                N = tree.GameTime;
            }

            while (tree.GameTime - N < Interval)
            {
                return BehaviorResult.running;
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
                N = 0.0f;
                return BehaviorResult.failed;              
            }
            else if (result == BehaviorResult.running)
            {
                return BehaviorResult.running;
            }
            else
            {
                return BehaviorResult.abort;
            }
            
        }
    }
}