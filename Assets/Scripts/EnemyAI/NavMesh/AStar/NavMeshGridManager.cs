using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The gridManager defines the navigation area of the agents
/// </summary>

public class NavMeshGridManager : MonoBehaviour
{
    public static NavMeshGridManager Instance { get; private set; }
    public Vector3 GridOriginsPos { get { return transform.position; } }
    [field: SerializeField] public int RowNum { get; private set; }
    [field: SerializeField] public int ColNum { get; private set; }
    [field: SerializeField] public int GridCellSize { get; private set; }
    [field: SerializeField] public CustomNavMeshObstacle[] customNaveMeshObstacles { get; private set; }
    public TileNode[,] Nodes { get; private set; }

    private int[,] offsets = new int[,]
   {
        {-1,  0},
        { 1,  0}, 
        { 0, -1}, 
        { 0,  1},
        {-1, -1},
        { 1, -1},
        {-1,  1}, 
        { 1,  1}  
   };

    public float StepCost
    {
        get { return GridCellSize; }
    }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        CreateGrid();
        FindObstacle();
    }

    /// <summary>
    /// Setting grid on x and y coordinates
    /// </summary>

    private void CreateGrid()
    {
        Vector3 gridPos = new Vector3(ColNum, 0, RowNum);
        Nodes = new TileNode[ColNum, RowNum];

        for (int x = 0; x < ColNum; x++)
        {
            for (int y = 0; y < RowNum; y++)
            {
                //Creating all tiles on grid using their position

                Vector3 worldPoint = GetGridCellCenter(x, y);

                Nodes[x, y] = new TileNode(worldPoint);
            }
        }
    }

    /// <summary>
    /// Gets the tile of the grid based on columm and row
    /// </summary>
    /// <param name="coluumm"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public Vector3 GetGridCellCenter(int coluumm, int row)
    {
        Vector3 cellPosition = GetGridCellPosition(coluumm, row);

        cellPosition.x += GridCellSize / 2.0f;
        cellPosition.z += GridCellSize / 2.0f;

        return cellPosition;
    }

    /// <summary>
    /// Gets the position of the tile on the grid based on columm and row
    /// </summary>
    /// <param name="coluumm"></param>
    /// <param name="row"></param>
    /// <returns></returns>

    public Vector3 GetGridCellPosition(int coluumm, int row)
    {
        float xPosInGrid = coluumm * GridCellSize;
        float zPosInGrid = row * GridCellSize;

        return GridOriginsPos + new Vector3(xPosInGrid, 0, zPosInGrid);
    }

    /// <summary>
    /// Initiliazes all obstacles on the grid
    /// </summary>

    public void FindObstacle()
    {
        foreach (CustomNavMeshObstacle customNaveMeshObstacle in customNaveMeshObstacles)
        {
            customNaveMeshObstacle.InitiliazeObstacle();
        }
    }

    /// <summary>
    /// Checkes if area is traversable for agents by checking the area is inside bounds and the tile is not an obstacle
    /// </summary>
    /// <param name="columm"></param>
    /// <param name="row"></param>
    /// <returns></returns>

    private bool IsTraversable(int columm, int row)
    {
        return columm >= 0 && columm < ColNum && row >= 0 && row < RowNum && !Nodes[columm, row].IsObstacle;
    }

    /// <summary>
    /// Gets the grid tile's columm and row by processing vector3 location
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public (int, int) GetGridCoordinates(Vector3 pos)
    {
        if (!IsInBounds(pos))
        {
            return (-1, -1);
        }

        int col = (int)Mathf.Floor((pos.x - GridOriginsPos.x) / GridCellSize);
        int row = (int)Mathf.Floor((pos.z - GridOriginsPos.z) / GridCellSize);

        return (col, row);
    }

    /// <summary>
    /// Check whether the current position is inside the grid or not
    /// </summary>
    public bool IsInBounds(Vector3 pos)
    {
        float width = ColNum * GridCellSize;
        float height = RowNum * GridCellSize;
        return (pos.x >= GridOriginsPos.x && pos.x <= GridOriginsPos.x + width && pos.z <= GridOriginsPos.z + height && pos.z >= GridOriginsPos.z);
    }

    /// <summary>
    /// Gets all the nehibors of the specified node
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<TileNode> GetNeighbors(TileNode node)
    {
        //Setting the list of the node's nehibors

        List<TileNode> neighbors = new List<TileNode>();

        //Gets the node cooardinates 

        (int column, int row) = GetGridCoordinates(node.Position);
        
        if (column == -1 || row == -1)
        {
            Debug.LogWarning("Node is out of bounds.");
            return neighbors;
        }

        //If columun and row are not -1, meaning invalid, the method iterates through the grid
        //The loop goes through the offsets list intiliazed above, taking in condideration straight and diagonal movements

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            //Setting X check variable, by setting the columm and adding the number of i and 0 to it
            int checkX = column + offsets[i, 0];
            //Setting the Y check variable, by getting the row and adding the number i and 1 to it
            int checkY = row + offsets[i, 1];

            //CheckX and CheckY variables are inserted into the signature of IsTraversable() method

            if (IsTraversable(checkX, checkY))
            {
                //Each node that passes the check inserted into the nehiboers list
                neighbors.Add(Nodes[checkX, checkY]);
            }
        }

        //Returns the nehibors list

        return neighbors;
    }

    private void OnDrawGizmos()
    {
        float width = (ColNum * GridCellSize);
        float height = (RowNum * GridCellSize);

        for (int i = 0; i < ColNum + 1; i++)
        {
            Vector3 startPos = GridOriginsPos + i * GridCellSize * new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 endPos = startPos + width * new Vector3(1.0f, 0.0f, 0.0f);

            Debug.DrawLine(startPos, endPos, Color.blue);
        }

        for (int i = 0; i < RowNum + 1; i++)
        {
            Vector3 startPos = GridOriginsPos + i * GridCellSize * new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 endPos = startPos + height * new Vector3(0.0f, 0.0f, 1.0f);
            Debug.DrawLine(startPos, endPos, Color.blue);
        }
    }
}
