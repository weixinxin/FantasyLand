//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 选择节点
    /// </summary>
    public class BehaviorSelectorNode : BehaviorComposeNode
    {
        public BehaviorPreconditonEvent OnPrecondition;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitSelectorNode (this);
		}

		public override string Name {
			get {
				return "Selector";
			}
		}

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (m_ChildNodes.Count == 0)
            {
              //  Debug.LogError("missing select children node");
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

           // var runingNode = m_ChildNodes[currentIndex];
            while (true)
            {
                if (currentIndex >= m_ChildNodes.Count)
                {                   
                    break;
                }
                var runingNode = m_ChildNodes[currentIndex];
                BehaviorResult result = runingNode.Exec(tree);
                if (result == BehaviorResult.success)
                {
                    SetCompleted();
                    return result;                   
                }
                else if (result == BehaviorResult.failed)
                {
                    currentIndex++;
                }
                else if (result == BehaviorResult.running)
                {
                    return result;                   
                }

            }

            SetFailed();
            return BehaviorResult.failed;

        }
    }
}