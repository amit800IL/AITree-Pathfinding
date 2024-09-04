using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This node makes the enemy to chase player when range condition is true
/// </summary>
public class ChaseNode : RootNode
{
    private CustomNavMeshAgent enemyNavMesh;
    private Transform playerTransform;

    public ChaseNode(CustomNavMeshAgent enemyNavMesh)
    {
        this.enemyNavMesh = enemyNavMesh;
        playerTransform = GameManager.Instance.player.transform;
    }

    public override NodeState Evaluate()
    {
        Debug.Log(enemyNavMesh.gameObject.name + " ,Running chase node");

        ChasePlayer();
        return NodeState.RUNNING;
    }

    public void ChasePlayer()
    {
        enemyNavMesh.Speed = 15f;
        //enemyNavMesh.radius = 0f;
        enemyNavMesh.SetDestination(enemyNavMesh, playerTransform.position);
    }
}
