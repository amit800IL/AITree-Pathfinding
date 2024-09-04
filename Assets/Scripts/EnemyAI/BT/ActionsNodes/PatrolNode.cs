using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This node makes the enemy patrol the area while not detecting the player
/// </summary>
public class PatrolNode : RootNode
{
    private CustomNavMeshAgent enemyNavMesh;
    private List<Transform> patrolPoints;
    private int patrolPointIndex = 0;

    /// <summary>
    /// This node makes the enemy patrol the area while not detecting the player
    /// </summary>
    /// <param name="enemyNavMesh"></param>
    public PatrolNode(CustomNavMeshAgent enemyNavMesh)
    {
        this.enemyNavMesh = enemyNavMesh;

        patrolPoints = GameManager.Instance.patrolPoints;

        patrolPointIndex = Random.Range(1, patrolPoints.Count);
    }

    public override NodeState Evaluate()
    {
        if (patrolPoints.Count == 0)
        {
            return NodeState.FALIURE;
        }

        if (enemyNavMesh.IsPathPending)
        {
            SetNextPatrolPoint();
        }

        return NodeState.SUCCESS;
    }

    private void SetNextPatrolPoint()
    {
        patrolPointIndex = Random.Range(1, patrolPoints.Count);

        Vector3 destination = patrolPoints[patrolPointIndex % patrolPoints.Count].position;

        if (destination != null && patrolPoints != null)
        {
            enemyNavMesh.SetDestination(enemyNavMesh, destination);
        }
    }

    public bool EnemyAtPatrolPoint(Vector3 destination)
    {
        //Setting the condition for checking if enemy is at patorl point

        float enemyPointDistance = Vector3.Distance(enemyNavMesh.transform.position, destination);

        //Setting the enemy speed to 20

        enemyNavMesh.Speed = 5f;

        //Setting the enemy readius to 3 to avoid obstacled

        return enemyPointDistance <= 5f;
    }

}
