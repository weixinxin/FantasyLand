//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorAssignmentNode : BehaviorNode
    {
		public delegate void AssignmentEvent(); 
		public BehaviorPreconditonEvent OnPrecondition = null;
        public AssignmentEvent OnAction;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitAssignmentNode (this);
		}

		public override string Name {
			get {
				return "Assignment";
			}
		}

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (OnAction == null)
            {
               // Debug.LogError("missing action event method");
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

            OnAction();

            SetCompleted();
            return BehaviorResult.success;
        }
    }
}
