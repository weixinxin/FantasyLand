//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public delegate BehaviorResult BehaviorNodeActionEvent();
    public enum BehaviorResult
    {
        failed,
        success,
        running,
        abort
    }

    public delegate bool BehaviorNodeConditionEvent();
    public delegate void BehaviorNodeAbortEvent();
    public delegate bool BehaviorPreconditonEvent();
    public delegate void BehaviorNodeEnterEvent();
    public delegate void BehaviorNodeExitEvent(BehaviorResult result);

    /// <summary>
    /// 行为结点基类
    /// </summary>
    public abstract class BehaviorNode 
    {
        public BehaviorNodeEnterEvent OnEnter;
        public BehaviorNodeExitEvent OnExit;
        public string Desc;
        public abstract BehaviorResult Exec(IBehaviorTree tree);


        /// <summary>
        /// 取消结点执行处理
        /// </summary>
        public virtual void Abort()
        {

        }

		public abstract string Name { get; }

		public abstract void Accept (BehaviorNodeVisitor visitor);

        public bool Running { get; private set; }

        protected virtual void Init()
        {

        }

        protected virtual void Reset()
        {

        }

        protected void SetStarting()
        {
            if (! Running)
            {                
                Running = true;
                Init();
                if (OnEnter != null)
                {
                    OnEnter();
                }
            }
        }


        protected virtual void SetCompleted()
        {
            if (Running)
            {
                Reset();
                Running = false;

                if (OnExit != null)
                {
                    OnExit(BehaviorResult.success);
                }
            }
        }

        protected virtual void SetFailed()
        {
            if (Running)
            {
                Reset();
                Running = false;
                if (OnExit != null)
                {
                    OnExit(BehaviorResult.failed);
                }
            }
        }

        protected void SetAborted()
        {
            if (Running)
            {             
                Running = false;
                Reset();
                if (OnExit != null)
                {
                    OnExit(BehaviorResult.abort);
                }
            }
        }

    }
}
