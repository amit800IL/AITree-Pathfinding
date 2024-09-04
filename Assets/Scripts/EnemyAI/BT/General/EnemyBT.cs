using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class EnemyBT : MonoBehaviour
{
    private RootNode BTRootNode;
    private BTConditions conditions;
    [SerializeField] private Enemy enemy;
    [SerializeField] private CustomNavMeshAgent enemyNavMesh;
    [SerializeField] private AudioSource hearinAudioSource;
    [SerializeField] private EnemyDataSO enemyDataSO;

    private void Start()
    {
        conditions = new BTConditions(enemy, enemyNavMesh, hearinAudioSource, enemyDataSO);
        CreateBeahviorTree();
    }

    private void FixedUpdate()
    {
        BTRootNode.Evaluate();
    }
    private void CreateBeahviorTree()
    {
        //Setting Action Nodes

        PatrolNode patrolNode = new PatrolNode(enemyNavMesh);
        FollowNode followNode = new FollowNode(enemyNavMesh);
        ChaseNode chaseNode = new ChaseNode(enemyNavMesh);
        AttackNode attackNode = new AttackNode(enemyNavMesh, enemyDataSO.damage);
        SearchPlayerLocationNode searchPlayerLocationNode = new SearchPlayerLocationNode(this, enemyNavMesh);

        //Setting Condition Nodes

        ConditionalNode followCondition = new ConditionalNode(conditions.IsPlayerInFollowRange, followNode);
        ConditionalNode chaseCondition = new ConditionalNode(conditions.IsPlayerInChaseRange, chaseNode);
        ConditionalNode attackCodition = new ConditionalNode(conditions.IsPlayerInAttackRange, attackNode);
        ConditionalNode canSeePlayer = new ConditionalNode(conditions.CanSeePlayer);
        ConditionalNode canSmellPlayer = new ConditionalNode(conditions.CanSmellPlayer);
        ConditionalNode canHearPlayer = new ConditionalNode(conditions.CanHearPlayer);
        ConditionalNode shouldEnemyDie = new ConditionalNode(conditions.ShouldEnemyDie);
        ConditionalNode serachPlayerCondition = new ConditionalNode(conditions.ShouldSearchPlayerLocation, searchPlayerLocationNode);

        //Setting selecot node for follow chase and attack conditions to activate

        SelectorNode followChaseAttackSelector = CreateSelectorNode(followCondition, chaseCondition, attackCodition);

        //Setting sequence nodes for each sense

        SequenceNode visionSequence = CreateSequenceNode(patrolNode, canSeePlayer, followChaseAttackSelector);
        SequenceNode smellSequence = CreateSequenceNode(patrolNode, canSmellPlayer, followChaseAttackSelector);
        SequenceNode hearSequence = CreateSequenceNode(patrolNode, canHearPlayer, followChaseAttackSelector);

        //Setting sense selector node

        SelectorNode senseSelectorNode = CreateSelectorNode(visionSequence, smellSequence, hearSequence);

        //Setting root selector node

        SelectorNode rootSelector = CreateSelectorNode(shouldEnemyDie, senseSelectorNode, serachPlayerCondition);

        BTRootNode = rootSelector;
    }

    private void OnDrawGizmos()
    {
        if (enemyNavMesh != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(enemyNavMesh.transform.position, enemyDataSO.smellRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(enemyNavMesh.transform.position, enemyDataSO.hearRadius);

            //Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(enemyNavMesh.transform.position, enemyDataSO.searchRadius);
        }
    }
    private SequenceNode CreateSequenceNode(params RootNode[] nodes)
    {
        SequenceNode sequenceNode = new SequenceNode();

        foreach (RootNode node in nodes)
        { 
            sequenceNode.AddChild(node);
        }

        return sequenceNode;
    }

    private SelectorNode CreateSelectorNode(params RootNode[] nodes)
    {
        SelectorNode selectorNode = new SelectorNode();

        foreach (RootNode node in nodes)
        {
            selectorNode.AddChild(node);
        }

        return selectorNode;
    }
}
public enum NodeState
{
    FALIURE,
    SUCCESS,
    RUNNING
}
