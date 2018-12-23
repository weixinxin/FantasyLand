using BattleSystem.ObjectModule;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    public class BehaviorTreeUnit : BehaviorTreeBase
    {
        public UnitController m_Controller { get; private set; }

        public UnitBase m_Agent { get; private set; }
        public BehaviorTreeUnit(UnitBase unit): base()
        {
            m_Controller = unit.Controller;
            m_Agent = unit;

            //Blackboard.AddVariable<string>("UnitCommand", "");
            //Blackboard.AddVariable<Vector3>("MoveTargetLocation", Vector3.zero);
            //Blackboard.AddVariable<bool>("MoveByFindPath", true);//是否寻路
            //Blackboard.AddVariable<AICommandCallbackEvent>("MoveCompleteEvent", null);
            //Blackboard.AddVariable<bool>("isPlayingAttackAnimation", false);
            Start();
        }

        //public override void NotifyMoveTo(Vector3 targetLocation,AICommandCallbackEvent callback)
        //{
        //    Blackboard.SetValue<string>("UnitCommand", AICommand.Moving);
        //    Blackboard.SetValue<Vector3>("MoveTargetLocation", targetLocation);
        //    Blackboard.SetValue<AICommandCallbackEvent>("MoveCompleteEvent", callback);
        //}

        //protected bool IsMatchedCommand(string cmd)
        //{
        //    string cur = Blackboard.GetValue<string>("UnitCommand");
        //    return cur == cmd;
        //}
        //protected void ResetMatchedCommand(string cmd)
        //{
        //    if (Blackboard.GetValue<string>("UnitCommand") == cmd)
        //        Blackboard.SetValue<string>("UnitCommand", "");
        //}

        //public override void NotifyFireSkill(Skill skill, AICommandCallbackEvent callback = null)
        //{

        //    Blackboard.SetValue<string>("UnitCommand", AICommand.Skill);
        //    Blackboard.SetValue<Skill>("PlayerFiredSkill", skill);

        //}

        protected virtual void Start()
        {
            Root.TargetNode = new BehaviorSelectorLoop()
                .Add(
                    new BehaviorWithPreconditonNode()
                    {
                        OnPrecondition = delegate()
                        {
                            return true;
                        }
                    }
                    .setActionNode(
                    new BehaviorLoopNode()
                        .setTargetNode(new BehaviorSelectorNode()
                            .Add(new BehaviorSequenceNode()
                                {
                                    Desc = "攻击",
                                    OnPrecondition = delegate()
                                    {
                                        return m_Agent.AttackTarget != null && !m_Agent.AttackTarget.IsDead && m_Agent.InAttackRange(m_Agent.AttackTarget);
                                    }
                                }
                                .Add(new BehaviorActionNode() { OnEnter = m_Controller.EnterWaitForAttackCD, OnAction = m_Controller.WaitForAttackCD })
                                .Add(new BehaviorActionNode() { OnEnter = m_Controller.EnterAttack, OnAction = m_Controller.Attack, OnExit = m_Controller.ExitAttack })
                            )
                            .Add(new BehaviorSequenceNode()
                                {
                                    Desc = "接近攻击目标",
                                    OnPrecondition = delegate()
                                    {
                                        return m_Agent.AttackTarget != null && !m_Agent.AttackTarget.IsDead && m_Agent.InVisualRange(m_Agent.AttackTarget);
                                    }
                                }
                                .Add(new BehaviorActionNode() { OnEnter = m_Controller.EnterApproachToAttackTarget, OnAction = m_Controller.ApproachToAttackTarget, OnExit = m_Controller.ExitApproachToAttackTarget })
                            ).Add(new BehaviorSequenceNode() 
                                {
                                    Desc = "待机并搜索",
                                }
                                .Add(new BehaviorActionNode() { Desc = "待机", OnEnter = m_Controller.EnterIdle, OnAction = m_Controller.Idle })
                                .Add(new BehaviorActionNode() { Desc = "搜索敌人", OnAction = m_Controller.SearchEnemy })
                            )
                        )
                    )
                );
            //Exec();
        }

    }
}
