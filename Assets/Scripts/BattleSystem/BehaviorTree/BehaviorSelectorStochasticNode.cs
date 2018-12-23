//using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 随机选择结点
    /// </summary>
    public class BehaviorSelectorStochasticNode : BehaviorComposeNode
    {
        public BehaviorPreconditonEvent OnPrecondition = null;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitSelectorStochasticNode (this);
		}

		public override string Name {
			get {
				return "SelectorStochastic";
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
               // Debug.LogError("missing select children node");
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

            // 随机排序
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
                    currentIndex++;
                }
                
            }
            SetFailed();
            return BehaviorResult.failed;
        }
    }
}
