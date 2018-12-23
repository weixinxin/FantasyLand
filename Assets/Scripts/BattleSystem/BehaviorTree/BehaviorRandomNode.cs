//using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 随机结点
    /// </summary>
    public class BehaviorRandomNode : BehaviorComposeNode
    {
        public BehaviorPreconditonEvent OnPrecondition;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitRandomNode (this);
		}

		public override string Name {
			get {
				return "Random";
			}
		}
        BehaviorNode currentNode = null;

        protected override void Reset()
        {
            base.Reset();
            currentNode = null;
        }

        private List<BehaviorNode> m_RunningList = null;

        protected override void Init()
        {
            m_RunningList = new List<BehaviorNode>(m_ChildNodes);
        }

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (m_ChildNodes.Count == 0)
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

            while (m_RunningList.Count > 0)
            {
                if (currentNode == null)
                {
                    int index = tree.RandomMgr.Next(0, m_RunningList.Count - 1);
                    currentNode = m_RunningList[index];
                    m_RunningList.RemoveAt(index);
                }

                BehaviorResult result = currentNode.Exec(tree);

                if(result == BehaviorResult.running)
                {
                    return BehaviorResult.running;
                }
                else if(result == BehaviorResult.success)
                {
                    SetCompleted();
                    return BehaviorResult.success;                  
                }
                else if(result == BehaviorResult.failed)
                {
                    return BehaviorResult.running;                  
                }
                
            }
            SetFailed();
            return BehaviorResult.failed;
        }
    }
}
