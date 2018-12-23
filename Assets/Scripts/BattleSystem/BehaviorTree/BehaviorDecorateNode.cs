//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 装饰结点基类
    /// </summary>
    public abstract class BehaviorDecorateNode : BehaviorNode
    {
        public BehaviorNode TargetNode;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitDecorateNode (this);
		}

		public override string Name {
			get {
				return "Decorate";
			}
		}

        public override void Abort()
        {
            if (Running)
            {
                if (TargetNode != null)
                {
                    TargetNode.Abort();
                }
                SetAborted();
            }
        }
    }
}
