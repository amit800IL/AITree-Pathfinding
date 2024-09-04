using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The selector node is for nodes that only one of them needs to succed
/// </summary>
public class SelectorNode : RootNode
{
    public override NodeState Evaluate()
    {
        foreach (RootNode child in children)
        {
            NodeState result = child.Evaluate();

            if (result == NodeState.SUCCESS)
            {
                return NodeState.SUCCESS;
            }

            if (result == NodeState.RUNNING)
            {
                return NodeState.RUNNING;
            }
        }

        return NodeState.FALIURE;
    }
}
