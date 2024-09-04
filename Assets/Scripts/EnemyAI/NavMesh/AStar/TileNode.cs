using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// The basic node for the navigation system
/// </summary>
public class TileNode
{
    public Vector3 Position { get; set; }
    public float CostSoFar { get; set; }
    public float HeurasticCost { get; } 
    public float FinalCost { get; set; }
    public TileNode ParentNode { get; set; }
    public bool IsObstacle { get; private set; }

    public TileNode(Vector3 position)
    {
        Position = position;
        IsObstacle = false;
        ResetNode();
    }

    public void ResetNode()
    {
        ParentNode = null;
        FinalCost = 0.0f;
        CostSoFar = 0.0f;
    }

    public void SetObstacleNode()
    {
       IsObstacle = true;
    }

    public void ResetObstacleNode()
    {
        IsObstacle = false;
    }

    public override bool Equals(object obj)
    {
        return obj is TileNode node && Position.Equals(node.Position);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
