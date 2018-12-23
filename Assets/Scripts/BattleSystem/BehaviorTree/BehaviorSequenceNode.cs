//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 顺序结点
    /// </summary>
    public class BehaviorSequenceNode : BehaviorComposeNode
    {
        public BehaviorPreconditonEvent OnPrecondition;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitSequenceNode (this);
		}

		public override string Name {
			get {
				return "Sequence";
			}
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

           // var runingNode = m_ChildNodes[currentIndex];
            while (true)
            {
                if (currentIndex >= m_ChildNodes.Count)
                {
                    break;
                }
                
                BehaviorResult result = m_ChildNodes[currentIndex].Exec(tree);
                if (result == BehaviorResult.success)
                {
                    currentIndex++;
                    if (currentIndex == m_ChildNodes.Count)
                    {
                        SetCompleted();
                        return result;
                    }
                }
                else if (result == BehaviorResult.failed)
                {
                    SetFailed();
                    return result;
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