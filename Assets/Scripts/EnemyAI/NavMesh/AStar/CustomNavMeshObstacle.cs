using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class of all obstacles on the navigation system
/// </summary>
public class CustomNavMeshObstacle : MonoBehaviour
{
    private List<(int, int)> previousObstacleCoordinates = new List<(int, int)>();
    [SerializeField] private int obstacleRadiusX = 6;
    [SerializeField] private int obstacleRadiusY = 6;

    private void Start()
    {
        //Subscribing to OnEnviormentChangesEvent in order to allow daynamic real time obstalce pathfinding
        CustomNavMeshAgent.OnEvniormentChanged += SetObstacle;
    }

    private void OnDisable()
    {
        CustomNavMeshAgent.OnEvniormentChanged -= SetObstacle;
    }

    /// <summary>
    /// Intiliazes obstacle on the grid
    /// </summary>
    public void InitiliazeObstacle()
    {
        if (NavMeshGridManager.Instance != null)
        {
            (int column, int row) = NavMeshGridManager.Instance.GetGridCoordinates(transform.position);

            if (previousObstacleCoordinates.Count > 0 && previousObstacleCoordinates != null)
                ClearObstacleArea();

            if (column != -1 && row != -1)
            {
                MarkObstacleArea(column, row, obstacleRadiusX, obstacleRadiusY);
            }
        }

    }

    /// <summary>
    /// Sets obstacles on the grid by each agents checkup daynamicly based on OnEnviormentChanges event call
    /// </summary>
    /// <param name="navMeshAgent"></param>
    /// <param name="position"></param>

    public void SetObstacle(CustomNavMeshAgent navMeshAgent, Vector3 position)
    {
        if (NavMeshGridManager.Instance != null && navMeshAgent != null)
        {
            //Getting the tile coordiantes

            (int column, int row) = NavMeshGridManager.Instance.GetGridCoordinates(transform.position);

            //Setting position to the columm and row of the tile

            position = new Vector3(column, 0, row);

            //In case that the specific obstacle position is the same as the new updated position, the update is canceled

            if (transform.position == position) return;

            //Clears all known obstacles to allow replacment for new ones

            if (previousObstacleCoordinates.Count > 0 && previousObstacleCoordinates != null)
                ClearObstacleArea();


            if (column != -1 && row != -1)
            {
                //Marks new obstacle area
                MarkObstacleArea(column, row, obstacleRadiusX, obstacleRadiusY);
            }
        }

    }

    /// <summary>
    /// Clear all obstacles for the obstacle list
    /// </summary>
    public void ClearObstacleArea()
    {
        NavMeshGridManager gridManager = NavMeshGridManager.Instance;

        //First it iterates through all the current obstacles on the list

        foreach ((int, int) item in previousObstacleCoordinates)
        {
            //Sets item1 and item2 to be X and Y
            int x = item.Item1;
            int y = item.Item2;

            //Checks if X and Y inside bounds

            if (x >= 0 && x < gridManager.ColNum && y >= 0 && y < gridManager.RowNum)
            {

                //If yes, it sets the specified node 

                TileNode node = gridManager.Nodes[x, y];

                if (node != null)
                {
                    //If the node is not null, it clears the node
                    node.ResetObstacleNode();
                    node.ResetNode();
                }
            }
        }

        //Clears the list to allow replacment of new obstacles later on
        previousObstacleCoordinates.Clear();
    }

    /// <summary>
    /// Marks the obstacle area, based on X and Y cooardinates, and the radius of X and Y
    /// </summary>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    /// <param name="radiusX"></param>
    /// <param name="radiusY"></param>
    private void MarkObstacleArea(int centerX, int centerY, int radiusX, int radiusY)
    {
        NavMeshGridManager gridManager = NavMeshGridManager.Instance;

        //Calcultes the cooardintes of the tile minus the radius to create obstacle area for both X and Y

        for (int x = centerX - radiusX; x <= centerX + radiusX; x++)
        {
            for (int y = centerY - radiusY; y <= centerY + radiusY; y++)
            {
                //Checks if x and y are inside the bounds of the grid

                if (x >= 0 && x < gridManager.ColNum && y >= 0 && y < gridManager.RowNum)
                {
                    //Sets node

                    TileNode node = gridManager.Nodes[x, y];

                    if (node != null)
                    {
                        //Sets node as obstacle node

                        node.SetObstacleNode();

                        //Node added to obstacle list to be traced

                        previousObstacleCoordinates.Add((x, y));
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        NavMeshGridManager gridManager = NavMeshGridManager.Instance;
        Gizmos.color = Color.red;

        if (gridManager != null)
        {
            Vector3 cellSize = new Vector3(gridManager.GridCellSize, 1.0f, gridManager.GridCellSize);

            if (gridManager.Nodes != null)
            {
                for (int i = 0; i < gridManager.ColNum; i++)
                {
                    for (int j = 0; j < gridManager.RowNum; j++)
                    {
                        if (gridManager.Nodes[i, j] != null && gridManager.Nodes[i, j].IsObstacle)
                        {
                            Gizmos.DrawCube(gridManager.GetGridCellCenter(i, j), cellSize);
                        }
                    }
                }
            }
        }
    }
}
