using System;
using UnityEngine;

/// <summary>
/// The conditional Node is repsonsible for making sure that enemy is doing actions only when they are allowed specified conditions
/// </summary>
public class ConditionalNode : RootNode
{
    private Func<bool> condition;
    private RootNode child;

    public ConditionalNode(Func<bool> condition, RootNode child = null)
    {
        this.condition = condition;
        this.child = child;
    }

    public override NodeState Evaluate()
    {
        if (ConditionIsAvailable() && child != null)
        {
            //If the condition is not null and the child node is not null
            //The method returns the evluation of the child node

            return child.Evaluate();
        }
        else if (ConditionIsAvailable() && child == null)
        {
            //If the condition is not null and the child node is null
            //The method returns success to make the system move forward in accordance with the condition
            return NodeState.SUCCESS;
        }

        return NodeState.FALIURE;
    }

    private bool ConditionIsAvailable()
    {
        return condition.Invoke() && condition != null;
    }

}
