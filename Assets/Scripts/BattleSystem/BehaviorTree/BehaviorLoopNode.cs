//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorLoopNode : BehaviorDecorateNode
    {
        public int LoopCount = -1;
        private int m_CounterLoop = 0;
        public BehaviorLoopNode setTargetNode(BehaviorNode node)
        {
            TargetNode = node;
            return this;
        }

		public override string Name {
			get {
				return "Loop";
			}
		}

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitLoopNode (this);
		}

        protected override void Reset()
        {
            base.Reset();
            m_CounterLoop = 0;
        }

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (TargetNode == null)
            {
                return BehaviorResult.failed;             
            }

            SetStarting();

            while (LoopCount < 0 || m_CounterLoop < LoopCount)
            {
                BehaviorResult result = TargetNode.Exec(tree);

                if (result == BehaviorResult.success)
                {
                    if (LoopCount > 0)
                    {
                        m_CounterLoop++;
                    }
                    //return BehaviorResult.running;
                }
                else if (result == BehaviorResult.failed)
                {
                    if (LoopCount > 0)
                    {
                        m_CounterLoop++;
                    }
                }
                else if (result == BehaviorResult.running)
                {
                    //return BehaviorResult.running;
                }
                return BehaviorResult.running;
            }

            SetCompleted();
            return BehaviorResult.success;
        }
    }
}
