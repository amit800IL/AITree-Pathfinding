using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This node makes the enemy to follow player when range condition is true
/// </summary>
public class FollowNode : RootNode
{
    private CustomNavMeshAgent enemyNavMesh;
    private Transform playerTransform;

    public FollowNode(CustomNavMeshAgent enemyNavMesh)
    {
        this.enemyNavMesh = enemyNavMesh;
        playerTransform = GameManager.Instance.player.transform;
    }

    public override NodeState Evaluate()
    {
        Debug.Log(enemyNavMesh.gameObject.name + " ,Running follow node");

        FollowPlayer();
        return NodeState.RUNNING;
    }

    public void FollowPlayer()
    {
        //Setting the enemy spee to 20, to make sure speed is normal
        enemyNavMesh.Speed = 10f;
     
        enemyNavMesh.SetDestination(enemyNavMesh, playerTransform.position);
    }
}
