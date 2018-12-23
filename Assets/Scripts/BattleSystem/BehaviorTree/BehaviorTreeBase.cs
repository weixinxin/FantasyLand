using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BattleSystem.BehaviorTree
{

    public class AICommand
    {
        public static string Moving = "Moving";
        public static string Skill = "Skill";
        public static string Attack = "Attack";
        public static string RearrageFormation = "RearrageFormation";       
    }
    public delegate void AICommandCallbackEvent();



    public class BehaviorTreeBase : IBehaviorTree
    {
        BehaviorTreeExecutor m_BehaviorTreeExecutor;
        public BehaviorTreeBase()
        {
            m_BehaviorTreeExecutor = new BehaviorTreeExecutor(this);
        }

        public virtual float GameTime
        {
            get
            {
                return Campaign.Instance.GameTimeElapsed;
            }
        }

        public float TickDuration
        {
            get { return m_BehaviorTreeExecutor.TickDuration; }
            set { m_BehaviorTreeExecutor.TickDuration = value; }
        }


        public System.Random RandomMgr
        {
            get { return null; }
        }

        public BehaviorRootNode Root { get { return m_BehaviorTreeExecutor.Root; } }
        public BehaviorBlackboard Blackboard { get { return m_BehaviorTreeExecutor.Blackboard; } }
        public bool Running { get { return m_BehaviorTreeExecutor.Running; } }

        public void SetPaused(bool pause)
        {
            m_BehaviorTreeExecutor.Paused = pause;
        }

        public void Exec()
        {
            m_BehaviorTreeExecutor.Exec();
        }

        public bool ExecImmediately()
        {
            return m_BehaviorTreeExecutor.ExecImmediately();
        }

        public void Stop()
        {
            m_BehaviorTreeExecutor.Stop();
        }
    }
}
