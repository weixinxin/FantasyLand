//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{


    /// <summary>
    /// 动作结点
    /// </summary>
    public class BehaviorActionNode : BehaviorNode
    {
        public BehaviorPreconditonEvent OnPrecondition;
        public BehaviorNodeActionEvent OnAction;
		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitActionNode (this);
		}

		public override string Name {
			get {
				return "Action";
			}
		}


        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (OnAction == null)
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

            BehaviorResult result = OnAction();

            if (result == BehaviorResult.success)
            {
                SetCompleted();                                 
            }
            else if (result == BehaviorResult.failed)
            {
                SetFailed();                  
            }

            return result;
            
        }

        public override void Abort()
        {
            if (Running)
            {
                SetAborted();
            }
        }
    }
}