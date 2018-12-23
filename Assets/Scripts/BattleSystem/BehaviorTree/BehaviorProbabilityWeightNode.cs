//using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    // 与BehaviorSelectorProbabilityNode配合使用，只能添加到BehaviorSelectorProbabilityNode结点下
    public class BehaviorProbabilityWeightNode : BehaviorDecorateNode
    {
        private int m_Weight;
        public int Weight
        {
            get { return m_Weight; }
            set { m_Weight = value; }
        }

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitProbabilityWeightNode (this);
		}

		public override string Name {
			get {
				return "ProbabilityWeight";
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
                SetCompleted();
                return BehaviorResult.success;           
            }
            else if (result == BehaviorResult.failed)
            {
                SetFailed();
                return BehaviorResult.failed;                
            }
            else if (result == BehaviorResult.running)
            {
                return BehaviorResult.running;
            }

            SetAborted();
            return BehaviorResult.abort;
        }
    }
}
