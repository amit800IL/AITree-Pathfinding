 using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This node makes the enemy search the player last location when condition is true
/// </summary>
public class SearchPlayerLocationNode : RootNode
{
    private EnemyBT EnemyBT;
    private CustomNavMeshAgent enemyNaveMesh;
    private Transform playerPosition;
    private Vector3 lastKnownPosition;
    bool isSearching = false;

    public SearchPlayerLocationNode(EnemyBT enemyBT, CustomNavMeshAgent enemyNaveMesh)
    {
        this.EnemyBT = enemyBT;
        this.enemyNaveMesh = enemyNaveMesh;
        playerPosition = GameManager.Instance.player.transform;
    }

    public override NodeState Evaluate()
    {
        if (!isSearching)
        {
            //If seraching is not already active, define the positoon of the player to the last known position

            lastKnownPosition = playerPosition.position;

            //Run the coroutine of player through the monoBeavior enemyBT class 

            EnemyBT.StartCoroutine(SearchPlayer());

            //Node returns success when done

            return NodeState.SUCCESS;
        }
        
        //If is searching already, return running

        return NodeState.RUNNING;
    }


    private IEnumerator SearchPlayer()
    {
        isSearching = true;

        yield return SearchAlgorithm();

        isSearching = false;
    }

    private IEnumerator SearchAlgorithm()
    {
        //Defining the amount of directions the enemy will serach the player
        int searchSteps = 8;
        //Defining the angle of each serach step, realtive to the amount of searches
        float angleStep = 360f / searchSteps;

        //looping through the steps of searching the player

        for (int i = 0; i < searchSteps; i++)
        {
            //Multiplying the angle step variable by each iteration number of search steps
            //This creates the final angle the enemy will move
            float angle = i * angleStep;

            //Debug.Log(enemyNaveMesh.gameObject.name + "Player is being searched");

            //Defining the position of the enemy at each step, by taking the last known position and adding a new vector3 using cosinos calcultion to define the x axis movement and sine calcultion to define the z axis movement

            Vector3 searchPosition = lastKnownPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            //Setting the enemy speed to 20

            enemyNaveMesh.Speed = 20f;

            //Setting the destination of the enemy to the search position

            //enemyNaveMesh.SetDestination(searchPosition);

            //yield return new WaitUntil(() => !enemyNaveMesh.pathPending && enemyNaveMesh.remainingDistance <= enemyNaveMesh.stoppingDistance);

            yield return new WaitForSeconds(1f);
        }
    }

}
