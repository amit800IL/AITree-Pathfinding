using System.Collections.Generic;

/// <summary>
/// The root node of the Beahvior Tree, base class for all nodes
/// </summary>
public abstract class RootNode
{
    protected List<RootNode> children = new List<RootNode>();

    /// <summary>
    /// This method evluate the enemy state each frame
    /// </summary>
    /// <returns></returns>
    public abstract NodeState Evaluate();

    /// <summary>
    /// This method is Adding a child node to a node
    /// </summary>
    /// <param name="childNode"></param>
    /// <returns></returns>
    public RootNode AddChild(RootNode childNode)
    {
        children.Add(childNode);
        return this;
    }
}


