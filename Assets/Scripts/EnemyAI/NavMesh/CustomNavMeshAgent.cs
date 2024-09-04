using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomNavMeshAgent : MonoBehaviour
{
    public static event Action<CustomNavMeshAgent, Vector3> OnEvniormentChanged;
    public bool IsPathPending { get; private set; } = false;

    [Header("General")]
    [SerializeField] private Rigidbody agentRigidBody;

    [Header("Pathfinding")]
    private AStarAlgorithm aStarAlgorithm = new AStarAlgorithm();
    private List<TileNode> pathNodes = new List<TileNode>();
    private int currentNodeIndex = 0;
    private List<Transform> patrolPoints;
    private int patrolPointIndex = 0;
    private Vector3 agentDestination;


    [field: Header("Restrictions & Defnitions")]
    [field: SerializeField] public float Speed { get; set; }
    [field: SerializeField] public float ObstacleAvoidanceRadius { get; set; }
    [SerializeField] private SphereCollider obstacleAvoidacneCollider;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private bool canAttackPlayerOnObstacle = false;

    private void Start()
    {
        obstacleAvoidacneCollider.radius = ObstacleAvoidanceRadius;
        OnEvniormentChanged += SetDestination;
    }

    private void OnDisable()
    {
        OnEvniormentChanged -= SetDestination;
    }

    private void Update()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        //If pathnodes is not yet set, allow start of pathfinding and return to create the list

        if (pathNodes == null || pathNodes.Count == 0)
        {
            IsPathPending = true;
            return;
        }

        //In case list is initiliazed, the system uses current node index to trace the steps of the agent
        //By doing so, the system goes from, index 0- start node, to the max indes of the list, and moving the agent along the tiles while increamenting tne number each time it reaches its stoppoing distance

        if (currentNodeIndex < pathNodes.Count)
        {
            //Setting the current node

            TileNode currentNode = pathNodes[currentNodeIndex];

            //Setting the direction to the current node

            Vector3 direction = (currentNode.Position - transform.position).normalized;

            //Making the agent to move to the node

            agentRigidBody.velocity = direction * Speed;

            //Calculating the distance to the node

            float distanceToNode = Vector3.Distance(transform.position, currentNode.Position);

            //checks If distance is smaller than agent's defined stopping distance

            if (distanceToNode <= stoppingDistance)
            {
                //if yes, Curent node index is incremented by 1 to allow movement to next node 
                currentNodeIndex++;

                if (currentNodeIndex >= pathNodes.Count)
                {
                    //In case the agents finished moving alongside the nodes, the agent stops, to recalculate the path
                    agentRigidBody.velocity = Vector3.zero;
                    IsPathPending = true;
                }
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            //If enemy obstacle avoidance collider detects obstacle, it becomes a regular collider, to allow safe distance for movement
            obstacleAvoidacneCollider.isTrigger = false;

            //In such case, OnEnviormentChanged event is invoked, making the agent recauclting its path and obstacle locations in case its neccery

            OnEvniormentChanged?.Invoke(this, agentDestination);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            //If enemy obstacle avoidance collider detects Enemy, it becomes a regular collider, to allow safe distance for movement

            obstacleAvoidacneCollider.isTrigger = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            //If enemy obstacle avoidance collider finished detecting obstacle, it becomes a trigger collider, to allow contiunation of smooth movement

            obstacleAvoidacneCollider.isTrigger = true;

            //In such case, OnEnviormentChanged event is invoked, making the agent recauclting its path and obstacle locations in case its neccery, its called here once more to make sure there are no misses of the obstacles recaucltion 

            OnEvniormentChanged?.Invoke(this, agentDestination);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            //If enemy obstacle avoidance collider finished detecting obstacle, it becomes a trigger collider, to allow contiunation of smooth movement

            obstacleAvoidacneCollider.isTrigger = true;
        }
    }

    public void SetDestination(CustomNavMeshAgent navMeshAgent, Vector3 destination)
    {
        //Checks if the reaclualting agent, is the specified agent that is addresed

        if (navMeshAgent == this)
        {
            //Setting the columms and rows of both starting and goal nodes

            (int startColumn, int startRow) = NavMeshGridManager.Instance.GetGridCoordinates(transform.position);
            (int goalColumn, int goalRow) = NavMeshGridManager.Instance.GetGridCoordinates(destination);

            //Setting agent destination to the inserted destination value

            agentDestination = destination;

            //In case either of the columms and rows are out of bounds, the system gets out of the function

            if (startColumn == -1 || startRow == -1 || goalColumn == -1 || goalRow == -1) return;

            //Setting the start and goal nodes based on their columm and row

            TileNode startNode = NavMeshGridManager.Instance.Nodes[startColumn, startRow];
            TileNode goalNode = NavMeshGridManager.Instance.Nodes[goalColumn, goalRow];


            if (goalNode.IsObstacle && destination == GameManager.Instance.player.transform.position && canAttackPlayerOnObstacle)
            {
                //In case the destination of the agent is the player, and the agent is allowed to attack player that is hiding inside a obstacle area, the enemy can attack the player

                Vector3 directionToPlayer = destination - transform.position;
                agentRigidBody.velocity = directionToPlayer * Speed;
                return;
            }

            else if (goalNode.IsObstacle)
            {
                //In case the agent is not allowed to move to the player, the agent recalcultes his path

                return;
            }

            pathNodes = aStarAlgorithm.FindPath(startNode, goalNode);

            currentNodeIndex = 0;
            IsPathPending = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (pathNodes == null)
            return;

        if (pathNodes.Count > 0)
        {
            int index = 1;
            foreach (TileNode node in pathNodes)
            {
                if (index < pathNodes.Count)
                {
                    TileNode nextNode = pathNodes[index];
                    Debug.DrawLine(node.Position, nextNode.Position, Color.green);
                    index++;
                }
            }
        }
    }
}