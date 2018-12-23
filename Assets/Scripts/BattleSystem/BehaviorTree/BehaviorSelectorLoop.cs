//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorSelectorLoop : BehaviorComposeNode
    {
        public override BehaviorComposeNode Add(BehaviorNode node)
        {
          //  Debug.Assert(node is BehaviorWithPreconditonNode, "node must be with precondition node");
            return base.Add(node);
        }

		public override string Name {
			get {
				return "SelectorLoop";
			}
		}

        protected BehaviorWithPreconditonNode CheckPreconditionChild()
        {
            for (int i = 0; i < m_ChildNodes.Count; ++i)
            {
                BehaviorWithPreconditonNode child = m_ChildNodes[i] as BehaviorWithPreconditonNode;
                if (child.IsConditionSatisfied())
                {
                    return child;
                }
            }
            return null;
        }

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitSelectorLoop (this);
		}

        private BehaviorWithPreconditonNode curNode = null;

        protected override void Reset()
        {
            base.Reset();
            curNode = null;
        }


        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (m_ChildNodes.Count == 0)
            {
                return BehaviorResult.failed;              
            }

            SetStarting();
           
            while (true)
            {
                if (curNode == null)
                {
                    curNode = CheckPreconditionChild();
                }
                if (curNode != null)
                {
                    //每次都重新检查子节点条件    
                    BehaviorWithPreconditonNode newNode = CheckPreconditionChild();
                    if (newNode != curNode)
                    {
                        curNode.Abort();

                        curNode = newNode;

                        if (curNode == null)
                        {
                            // 等待到下一帧再跳到循环开头继续检测
                            return BehaviorResult.running;
                        }
                                             
                     }

                    BehaviorResult result = curNode.Exec(tree);

                    if (result == BehaviorResult.success)
                    {
                        SetCompleted();
                        return BehaviorResult.success;                     
                    }
                    else if (result == BehaviorResult.failed)
                    {
                        // 子节点执行失败时不退出，继续选择
                        curNode = null;
                        return BehaviorResult.running;                      
                    }
                    else if (result == BehaviorResult.running)
                    {
                        return BehaviorResult.running;// 等待一帧                                         
                    }
                }
                else
                {                    
                    return BehaviorResult.running;
                }
            }
        }
    }
}
