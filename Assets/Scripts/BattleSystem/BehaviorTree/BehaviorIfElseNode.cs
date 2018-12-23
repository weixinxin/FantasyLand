//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 条件结点
    /// </summary>
    public class BehaviorIfElseNode : BehaviorNode
    {
        public BehaviorPreconditonEvent OnCondition;
        public BehaviorNode TrueNode;
        public BehaviorNode FalseNode;

        public BehaviorIfElseNode setTrueNode(BehaviorNode node)
        {
            TrueNode = node;
            return this;
        }

        public BehaviorIfElseNode setFalseNode(BehaviorNode node)
        {
            FalseNode = node;
            return this;
        }

        private BehaviorNode m_BranchNode = null;// 当前运行的分支

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitIfElseNode (this);
		}

		public override string Name {
			get {
				return "IfElse";
			}
		}

        protected override void Reset()
        {
            base.Reset();
            m_BranchNode = null;
        }

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (OnCondition == null)
            {
               // Debug.LogError("missing condition event method");
                return BehaviorResult.failed;               
            }
            if (TrueNode == null)
            {
                //Debug.LogError("missing true node");
                return BehaviorResult.failed;               
            }
            if (FalseNode == null)
            {
               // Debug.LogError("missing false node");
                return BehaviorResult.failed;              
            }

            if (! Running)
            {
                if (OnCondition())
                {
                    m_BranchNode = TrueNode;
                }
                else
                {
                    m_BranchNode = FalseNode;
                }

                SetStarting();
            }

            BehaviorResult result = m_BranchNode.Exec(tree);

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
                SetFailed();
                return BehaviorResult.failed;               
            }
            SetAborted();
            return BehaviorResult.abort;          

        }

        public override void Abort()
        {
            if (Running)
            {
                m_BranchNode.Abort();
                SetAborted();
            }
        }
    }
}