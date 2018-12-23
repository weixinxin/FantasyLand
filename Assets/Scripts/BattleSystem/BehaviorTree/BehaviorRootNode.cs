//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 根结点
    /// </summary>
    public class BehaviorRootNode : BehaviorDecorateNode
    {
		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitRootNode (this);
		}

		public override string Name {
			get {
				return "Root";
			}
		}


        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (TargetNode == null)
            {
               // Debug.LogError("missing target node");
                return BehaviorResult.failed;               
            }

            SetStarting();

            while (true)
            {
                BehaviorResult result = TargetNode.Exec(tree);

                if(result == BehaviorResult.success)
                {
                    return BehaviorResult.success;   
                
                }
                else if(result == BehaviorResult.failed)
                {
                    return BehaviorResult.failed;                    
                }
                else if(result == BehaviorResult.running)
                {
                    return BehaviorResult.running;
                }
                
            }
        }
    }
}