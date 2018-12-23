//using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace BattleSystem.BehaviorTree
{
    public interface IBehaviorTree
    {
        float TickDuration { get; set; }

        float GameTime { get; }

        System.Random RandomMgr { get; }

        BehaviorRootNode Root { get; }
        BehaviorBlackboard Blackboard { get; }
        bool Running { get; }

        void Exec();
        bool ExecImmediately();
        void Stop();

        void SetPaused(bool pause);

       // MonoBehaviour MonoBehaviour { get; }
       // NetworkBehaviour NetworkBehaviour { get; }
    }

    internal class BehaviorTreeExecutor
    {
        private IBehaviorTree m_Owner;

        private float m_TickDuration = 0.1f;
        public float TickDuration
        {
            get { return m_TickDuration; }
            set { m_TickDuration = mathf.Clamp(value, 0, float.MaxValue); }
        }

        private BehaviorRootNode m_Root;
        private BehaviorBlackboard m_Blackboard;
        private bool m_Running;

       // private Coroutine m_ExecCoroutine;

        private BehaviorNode currentNode;
        public BehaviorRootNode Root { get { return m_Root; } }
        public BehaviorBlackboard Blackboard { get { return m_Blackboard; } }
        public bool Running { get { return m_Running; } }

        public bool Paused { get; set; }

      //  private IEnumerator<BehaviorResult> m_CurrentTask;

        public BehaviorTreeExecutor(IBehaviorTree owner)
        {
            m_Owner = owner;
            m_Root = new BehaviorRootNode();
            m_Blackboard = new BehaviorBlackboard();
        }

        public void Exec()
        {

            if (!Paused)
            {
                m_Running = true;
                m_Root.Exec(m_Owner);
            }                
        }

        public bool ExecImmediately()
        {
            if (m_Running && currentNode != null)
            {
                currentNode.Exec(m_Owner);
                return true;
            }
            return false;
        }

        public void Stop()
        {
            if (m_Running)
            {
                Root.Abort();
                m_Running = false;
            }
        }
    }
}
