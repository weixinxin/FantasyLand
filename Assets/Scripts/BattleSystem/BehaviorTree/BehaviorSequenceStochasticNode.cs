//using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorSequenceStochasticNode : BehaviorComposeNode
    {
        public BehaviorPreconditonEvent OnPrecondition = null;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitSequenceStochasticNode (this);
		}

		public override string Name {
			get {
				return "SequenceStochastic";
			}
		}


        private List<BehaviorNode> nodes = null;


        protected override void Reset()
        {
            base.Reset();
            nodes = null;
        }

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (m_ChildNodes.Count == 0)
            {
               // Debug.LogError("missing sequence children node");
                return BehaviorResult.failed;               
            }

            if (! Running)
            {
                if (OnPrecondition != null)
                {
                    if (!OnPrecondition())
                    {
                        SetFailed();
                        return BehaviorResult.failed;
                    }
                }

                SetStarting();
            }

            if (nodes == null)
            {
                nodes = new List<BehaviorNode>(m_ChildNodes);
                nodes.Sort(delegate(BehaviorNode x, BehaviorNode y) { return tree.RandomMgr.Next(-1, 1); });
            }

            while(true)           
            {

                if (currentIndex >= nodes.Count)
                {
                    break;
                }

                BehaviorResult result = nodes[currentIndex].Exec(tree);

                if (result == BehaviorResult.running)
                {
                     return BehaviorResult.running;
                }
                else if (result == BehaviorResult.failed)
                {
                    SetFailed();
                    return BehaviorResult.failed;                 
                }
                else if (result == BehaviorResult.success)
                {
                    currentIndex++;
                    if (currentIndex == m_ChildNodes.Count)
                    {
                        SetCompleted();
                        return result;
                    }
                }
                
            }
            SetCompleted();
            return BehaviorResult.success;
        }
    }
}
