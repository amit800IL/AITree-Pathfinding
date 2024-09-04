/// <summary>
/// The sequence node is for nodes that need to succed in order
/// </summary>
public class SequenceNode : RootNode
{
    public override NodeState Evaluate()
    {
        foreach (RootNode child in children)
        {
            NodeState result = child.Evaluate();

            if (result == NodeState.FALIURE)
            {
                return NodeState.FALIURE;
            }
            if (result == NodeState.RUNNING)
            {
                return NodeState.RUNNING;
            }
        }

        return NodeState.SUCCESS;
    }
}

 