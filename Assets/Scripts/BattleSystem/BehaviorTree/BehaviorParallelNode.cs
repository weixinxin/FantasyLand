//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public enum ParallelPolicyType
    {
        FailedIfOneFaileds,
        SuccessedIfOneSuccesseds
    }
    /// <summary>
    /// 并行结点
    /// </summary>
    public class BehaviorParallelNode : BehaviorComposeNode
    {
        public BehaviorPreconditonEvent OnPrecondition;
        public ParallelPolicyType ParallelPolicy;

		public override void Accept (BehaviorNodeVisitor visitor)
		{
			visitor.VisitParallelNode (this);
		}

		public override string Name {
			get {
				return "Parallel";
			}
		}

        public override BehaviorResult Exec(IBehaviorTree tree)
        {
            if (m_ChildNodes.Count == 0)
            {
              //  Debug.LogError("missing parallel children node");
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
            
            //var taskNodeMap = new Dictionary<BehaviorResult, BehaviorNode>();
            var RunnimgTaskList = new List<BehaviorNode>();
            var taskList = new List<BehaviorResult>();
            foreach (BehaviorNode child in m_ChildNodes)
            {
                BehaviorResult task = child.Exec(tree);
                taskList.Add(task);
                if(task == BehaviorResult.running)
                    RunnimgTaskList.Add(child);
                //taskNodeMap[task] = child;
            }

            var tasksToKill = new List<BehaviorResult>();

            while (true)
            {
                foreach (var task in taskList)
                {
                    if (ParallelPolicy == ParallelPolicyType.FailedIfOneFaileds)
                    {
                        if (task == BehaviorResult.success)
                        {
                            tasksToKill.Add(task);
                        }
                        else if (task == BehaviorResult.running)
                        {
                            // do nothing, wait for all task MoveNext complete
                        }
                        else if (task == BehaviorResult.failed)
                        {
                            foreach (var t in RunnimgTaskList)
                            {
                                t.Abort();
                            }
                            SetFailed();
                            return BehaviorResult.failed;
                            
                        }
                    }
                    else if (ParallelPolicy == ParallelPolicyType.SuccessedIfOneSuccesseds)
                    {
                        if (task == BehaviorResult.success)
                        {
                            foreach (var t in RunnimgTaskList)
                            {
                                t.Abort();
                            }
                            SetCompleted();
                            return BehaviorResult.success;                          
                        }
                        else if (task == BehaviorResult.running)
                        {
                            // do nothing, wait for all task MoveNext complete
                        }
                        else if (task == BehaviorResult.failed)
                        {
                            tasksToKill.Add(task);
                        }
                        
                    }
                }
                if (tasksToKill.Count > 0)
                {
                    foreach (var t in tasksToKill)
                    {
                        taskList.Remove(t);
                    }
                    tasksToKill.Clear();
                }

                if (taskList.Count == 0)
                {
                    if (ParallelPolicy == ParallelPolicyType.FailedIfOneFaileds)
                    {
                        SetCompleted();
                        return BehaviorResult.success;                       
                    }
                    else if (ParallelPolicy == ParallelPolicyType.SuccessedIfOneSuccesseds)
                    {
                        SetFailed();
                        return BehaviorResult.failed;                    
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
