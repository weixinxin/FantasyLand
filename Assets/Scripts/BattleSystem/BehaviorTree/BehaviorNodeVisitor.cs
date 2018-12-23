//using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Networking;

namespace BattleSystem.BehaviorTree
{
	public abstract class BehaviorNodeVisitor
	{
		abstract public void VisitActionNode (BehaviorActionNode node);
		abstract public void VisitAssignmentNode(BehaviorAssignmentNode node);
		abstract public void VisitCDTimeNode(BehaviorCDTimeNode node);
		abstract public void VisitComposeNode(BehaviorComposeNode node);
		abstract public void VisitConditioNode(BehaviorConditionNode node);
		abstract public void VisitDecorateNode(BehaviorDecorateNode node);
		abstract public void VisitFalseNode(BehaviorFalseNode node);
		abstract public void VisitIfElseNode(BehaviorIfElseNode node);
		abstract public void VisitInvertNode(BehaviorInvertNode node);
		abstract public void VisitLoopNode(BehaviorLoopNode node);
		abstract public void VisitParallelNode(BehaviorParallelNode node);
		abstract public void VisitProbabilityWeightNode(BehaviorProbabilityWeightNode node);
		abstract public void VisitRandomNode(BehaviorRandomNode node);
		abstract public void VisitRootNode(BehaviorRootNode node);
		abstract public void VisitSelectorNode(BehaviorSelectorNode node);
		abstract public void VisitSelectorLoop(BehaviorSelectorLoop node);
		abstract public void VisitSelectorProbabilityNode(BehaviorSelectorProbabilityNode node);
		abstract public void VisitSelectorStochasticNode(BehaviorSelectorStochasticNode node);
		abstract public void VisitSequenceNode(BehaviorSequenceNode node);
		abstract public void VisitSequenceStochasticNode(BehaviorSequenceStochasticNode node);
		abstract public void VisitTimeDelayNode(BehaviorTimeDelayNode node);
		abstract public void VisitTimeLimitNode(BehaviorTimeLimitNode node);
		abstract public void VisitTrueNode(BehaviorTrueNode node);
		abstract public void VisitUntilFailedNode(BehaviorUntilFailedNode node);
		abstract public void VisitUntilSuccessNode(BehaviorUntilSuccessNode node);
		abstract public void VisitWaitNode(BehaviorWaitNode node);
		abstract public void VisitWithPreconditionNode(BehaviorWithPreconditonNode node);
	}
}

