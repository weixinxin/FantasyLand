//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 组合结点基类
    /// </summary>
    public abstract class BehaviorComposeNode : BehaviorNode
    {
		public List<BehaviorNode> m_ChildNodes { get; internal set; }
        public BehaviorNode currentRunningNode;

        protected int currentIndex = 0;//当前执行的子节点

        public BehaviorComposeNode()
        {
            m_ChildNodes = new List<BehaviorNode>();
        }
        public virtual BehaviorComposeNode Add(BehaviorNode node)
        {
            m_ChildNodes.Add(node);
            return this;
        }

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitComposeNode (this);
		}

        protected override void Reset()
        {
            base.Reset();
            currentIndex = 0;
        }

		public override string Name {
			get {
				return "Compose";
			}
		}

        public override void Abort()
        {
            if (Running)
            {
                foreach (var node in m_ChildNodes)
                {
                    node.Abort();
                }
                SetAborted();
            }
        }
    }
}
