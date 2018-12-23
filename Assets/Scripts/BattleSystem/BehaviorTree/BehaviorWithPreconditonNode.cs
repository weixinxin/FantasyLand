//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorWithPreconditonNode : BehaviorNode
    {
        public BehaviorPreconditonEvent OnPrecondition = null;
        public BehaviorNode ActionNode = null;

        public BehaviorWithPreconditonNode setActionNode(BehaviorNode node)
        {
            ActionNode = node;
            return this;
        }

        public bool IsConditionSatisfied()
        {
            if (OnPrecondition == null)
            {
               // Debug.LogError("with precondition missing precondition event method");
                return false;
            }
            return OnPrecondition();
        }

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitWithPreconditionNode (this);
		}

		public override string Name {
			get {
				return "WithPrecondition";
			}
		}

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (ActionNode == null)
            {
               // Debug.LogError("missing action node");
                return BehaviorResult.failed;               
            }

            SetStarting();

            BehaviorResult result = ActionNode.Exec(tree);

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
            else
            {
                SetAborted();
                return BehaviorResult.abort;           
            }
            
        }

        public override void Abort()
        {
            if (Running)
            {
                if (ActionNode != null)
                {
                    ActionNode.Abort();
                }
                SetAborted();
            }
        }
    }
}
