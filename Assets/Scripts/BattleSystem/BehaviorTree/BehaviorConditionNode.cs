//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 条件结点
    /// </summary>
    public class BehaviorConditionNode : BehaviorNode
    {
        public BehaviorNodeConditionEvent OnConditionEvent;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitConditioNode (this);
		}

		public override string Name {
			get {
				return "Condition";
			}
		}

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (OnConditionEvent == null)
            {
                return BehaviorResult.failed;              
            }

            SetStarting();

            if (OnConditionEvent())
            {
                SetCompleted();
                return BehaviorResult.success;
            }
            else
            {
                SetFailed();
                return BehaviorResult.failed;
            }
        }
    }
}