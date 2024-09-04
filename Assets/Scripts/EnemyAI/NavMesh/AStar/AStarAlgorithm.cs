using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class AStarAlgorithm
{
    /// <summary>
    /// This function calculates the path of the agent from start to goal
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public List<TileNode> FindPath(TileNode start, TileNode goal)
    {
        //Resets all the existing nodes 

        ResetNodes();

        //Creates openList for the nodes

        NodePriorityQueue openList = new NodePriorityQueue();

        //Inserts the start node to the open list

        openList.Enqueue(start);

        //Resets costSoFar value to 0

        start.CostSoFar = 0.0f;

        //Sets final cost from start node the hurestaic cost 

        start.FinalCost = GetHuresaticCost(start, goal);

        //Creates closes list to detect which nodes already checked

        HashSet<TileNode> closedList = new HashSet<TileNode>();

        while (openList.Length != 0)
        {
            //Gets the current node from the openList

            TileNode currentNode = openList.Dequeue();

            if (currentNode.Position == goal.Position)
            {
                //If the current node position is the goal node position, it calcultes the path
                Debug.Log("Path Found!");
                return CalculatePath(currentNode);
            }

            //Sets new nehibor list to get the nehiboers of the current node

            List<TileNode> neighbors = NavMeshGridManager.Instance.GetNeighbors(currentNode);

            foreach (TileNode neighbor in neighbors)
            {
                //Checks if the closes list contains a nehibor, or the current nehibor is obstacle
                if (closedList.Contains(neighbor) || neighbor.IsObstacle)
                {
                    //If yes, the loop exists to the main loop
                    continue;
                }

                //Sets the step cost to be in accordance with direction- diagonal or stright

                float diagonalCost = 1.5f;
                float starightCost = 1.0f;

                float stepCost = IsDiagnoal(currentNode, neighbor) ? diagonalCost : starightCost;

                //Sets the total cost to be the CostSoFar and adding the step cost

                float totalCost = currentNode.CostSoFar + stepCost;

                //Gets the huresatic cost of the nehibor and goal nodes

                float heurasticValue = GetHuresaticCost(neighbor, goal);

                //If the open list does not already contain nehibor, and the total cost is less then cost so far

                if (!openList.Contains(neighbor) || totalCost < neighbor.CostSoFar)
                {
                    //Parent node is set to be current node
                    neighbor.ParentNode = currentNode;

                    //Cost so far is set to be total cost

                    neighbor.CostSoFar = totalCost;

                    //Final cost is set to be total cost plus the heruastic value 

                    neighbor.FinalCost = totalCost + heurasticValue;

                    //If the clost list does not contain the nehibor

                    if (!closedList.Contains(neighbor))
                    {
                        //Add the nehibor to the open list
                        openList.Enqueue(neighbor);
                    }
                }
            }

            //Adding the current node to the closed list to not allow calculation next iteration
            closedList.Add(currentNode);
        }

        //If path is not founds, method returns null
        Debug.LogError("Path not found!");
        return null;
    }

    public void ResetNodes()
    {
        //Gets the nodes array

        TileNode[,] nodes = NavMeshGridManager.Instance.Nodes;

        //Iterates through the nodes

        foreach (TileNode node in nodes)
        {
            //Resets each node

            node.ResetNode();
        }
    }
    private List<TileNode> CalculatePath(TileNode node)
    {
        //Creates new nodesPath list

        List<TileNode> nodesPath = new List<TileNode>();

        while (node != null)
        {
            //While the node is noot null, the node is added to nodesPath list
            nodesPath.Add(node);

            //the parent node of the node is set to be the node
            node = node.ParentNode;
        }

        //Reversing the list to trace the start node up to goal node

        nodesPath.Reverse();

        //Returning the nodesPath list

        return nodesPath;
    }

    /// <summary>
    /// Geths the hureastic cost value by using a modified manthan clacultion 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>

    public float GetHuresaticCost(TileNode start, TileNode end)
    {
        //Sets the movement cost of regular horizontal or vertical movements
        float D = 1.0f;
        //Sets the movement cost of diagonal movement
        float D2 = 1.5f;


        //Gets the absolute number of the x movement direction
        float dx = Mathf.Abs(start.Position.x - end.Position.x);
        //Gets the abosulute number of the z movement direction 
        float dz = Mathf.Abs(start.Position.z - end.Position.z);

        //Calcultes using Manthan calcultion: D * the the addition of the directions, subtracting the Diagonal movement value and substracting 2 multiplying it by the regular movement value, in order to adjust the calcultion to be in favor of the shortest most efficient route, muliplying it by the minimum of the x and z directions to get the amount of the diagonal steps that are possible
        return D * (dx + dz) + (D2 - 2 * D) * Mathf.Min(dx, dz);
    }

    /// <summary>
    /// Checks if diagonal by checking if the x and z of start and goal nodes are different, if yes, the diretion is diagonal as a result
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>

    private bool IsDiagnoal(TileNode start, TileNode end)
    {
        return start.Position.x != end.Position.x && start.Position.z != end.Position.z;
    }
}
