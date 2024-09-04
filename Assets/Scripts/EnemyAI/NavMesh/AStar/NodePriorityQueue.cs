using System.Collections.Generic;
using System.Linq;

public class NodePriorityQueue
{
    private List<TileNode> nodesList = new List<TileNode>();
    public int Length { get { return nodesList.Count; } }


    public void Enqueue(TileNode node)
    {
        if (nodesList.Contains(node))
        {
            TileNode oldNode = nodesList.First(n => n.Equals(node));

            if (oldNode.FinalCost <= node.FinalCost)
            {
                return;
            }
            else
            {
                nodesList.Remove(oldNode);
            }
        }

        nodesList.Add(node);

        nodesList.Sort((a, b) => a.FinalCost.CompareTo(b.FinalCost));
    }

    public TileNode Dequeue()
    {
        if (nodesList.Count > 0)
        {
            TileNode dequeueNode = nodesList[0];
            nodesList.RemoveAt(0);
            return dequeueNode;
        }

        return null;
    }
    public bool Contains(TileNode node)
    {
        return nodesList.Contains(node);
    }
}
