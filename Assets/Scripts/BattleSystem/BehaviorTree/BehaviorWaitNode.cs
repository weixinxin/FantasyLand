//using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorWaitNode : BehaviorNode
    {
        public float Interval;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitWaitNode (this);
		}

		public override string Name {
			get {
				return "Wait";
			}
		}
        private float N = 0.0f;

        protected override void Reset()
        {
            base.Reset();
            N = 0.0f;
        }

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            SetStarting();

            if (N == 0.0f)
            {
                N = tree.GameTime;
            }
            while (tree.GameTime - N < Interval)
            {
                return BehaviorResult.running;
            }
             
            SetCompleted();
            N = 0.0f;
            return BehaviorResult.success;
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
