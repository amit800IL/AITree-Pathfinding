using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Attack node is responsible for enemyAttack on the player
/// </summary>
public class AttackNode : RootNode
{
    public static event Action<float> OnPlayerAttacked;
    private float damage;
    private CustomNavMeshAgent enemyNavMesh;
    private Transform playerTransform;

    public AttackNode(CustomNavMeshAgent enemyNavMesh, float damage)
    {
        this.damage = damage;

        this.enemyNavMesh = enemyNavMesh;

        playerTransform = GameManager.Instance.player.transform;
    }

    public override NodeState Evaluate()
    {
        AttackPlayer(damage);
        return NodeState.SUCCESS;
    }

    public void AttackPlayer(float damage)
    {
        Debug.Log(enemyNavMesh.gameObject.name + "Attacking Player");

        ////Invokes event of attacking, making the player resieciving damage
        OnPlayerAttacked?.Invoke(damage);
    }
}
