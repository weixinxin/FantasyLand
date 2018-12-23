//using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 概率选择结点
    /// </summary>
    public class BehaviorSelectorProbabilityNode : BehaviorComposeNode
    {
        public BehaviorPreconditonEvent OnPrecondition = null;
        public override BehaviorComposeNode Add(BehaviorNode node)
        {
           // Debug.Assert(node is BehaviorProbabilityWeightNode, "node must be probability weight node");
            return base.Add(node);
        }

        public override void Accept(BehaviorNodeVisitor visitor)
        {
            visitor.VisitSelectorProbabilityNode(this);
        }

        public override string Name
        {
            get
            {
                return "SelectorProbability";
            }
        }

        private List<BehaviorNode> m_RunningNodes = null;
        private int Probability = -1;

        protected override void Init()
        {
            base.Init();
            // 按权重从大到小排序
            m_RunningNodes = new List<BehaviorNode>(m_ChildNodes);
            m_RunningNodes.Sort(
                delegate(BehaviorNode x, BehaviorNode y) 
                { 
                    return (x as BehaviorProbabilityWeightNode).Weight.CompareTo((y as BehaviorProbabilityWeightNode).Weight); 
                }
            );
        }

        protected override void Reset()
        {
            base.Reset();
            Probability = -1;
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

            // 概率
            if (Probability == -1)
            {
                Probability = tree.RandomMgr.Next(0, 100);
            }

            while(true)
            {
                if (currentIndex >= m_RunningNodes.Count)
                {
                    break;
                }
                if (Probability <= (m_RunningNodes[currentIndex] as BehaviorProbabilityWeightNode).Weight)
                {
                    BehaviorResult result = m_RunningNodes[currentIndex].Exec(tree);

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
            }
            SetFailed();
            return BehaviorResult.failed;
        }
    }
}
