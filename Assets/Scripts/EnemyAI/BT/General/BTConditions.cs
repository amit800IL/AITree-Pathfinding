using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BTConditions
{
    private Enemy enemy;
    private Transform playerPosition;
    private CustomNavMeshAgent enemyNavMesh;
    private AudioSource hearinAudioSource;
    private EnemyDataSO enemyDataSO;

    /// <summary>
    /// A class to hold all the conditions for the condition nodes of the BT
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="enemyNavMesh"></param>
    /// <param name="hearinAudioSource"></param>
    /// <param name="enemyDataSO"></param>

    public BTConditions(Enemy enemy, CustomNavMeshAgent enemyNavMesh, AudioSource hearinAudioSource, EnemyDataSO enemyDataSO)
    {
        playerPosition = GameManager.Instance.player.transform;
        this.enemy = enemy;
        this.enemyNavMesh = enemyNavMesh;
        this.hearinAudioSource = hearinAudioSource;
        this.enemyDataSO = enemyDataSO;
    }

    /// <summary>
    /// Checks is player is on attack range
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerInAttackRange()
    {
        float distance = Vector3.Distance(enemyNavMesh.transform.position, playerPosition.position);

        //Allow attacking when range is lower than 15

        return distance <= 15f;
    }

    /// <summary>
    /// Checks is player is on chase range
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerInChaseRange()
    {
        float distance = Vector3.Distance(enemyNavMesh.transform.position, playerPosition.position);

        //Allow chasing when distance is between 15 and 25

        return distance <= 25f && distance > 15f;
    }

    /// <summary>
    /// Checks is player is on follow range
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerInFollowRange()
    {
        float distance = Vector3.Distance(enemyNavMesh.transform.position, playerPosition.position);

        //Allow following when distance is between 25 and 50

        return distance > 25f && distance <= 50f;
    }

    public bool CanSeePlayer()
    {
        //Debug.Log("Trying to see player");

        Vector3 directionToPlayer = playerPosition.position - enemyNavMesh.transform.position;
        float distance = directionToPlayer.magnitude;

        RaycastHit hit;

        bool raycast = Physics.Raycast(enemyNavMesh.transform.position, directionToPlayer.normalized, out hit, enemyDataSO.visionDistance);

        Debug.DrawRay(enemyNavMesh.transform.position, directionToPlayer.normalized * enemyDataSO.visionDistance, Color.red);

        if (raycast)
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public bool CanSmellPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(enemyNavMesh.transform.position, enemyDataSO.smellRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
    public bool CanHearPlayer()
    {
        //Checks is the audio source the enemy needs to listen to is activated

        if (hearinAudioSource.isPlaying && hearinAudioSource != null)
        {
            //If ths audio source is activated, it calcultes the distance between the enemy and the source

            Vector3 direction = enemyNavMesh.transform.position - hearinAudioSource.transform.position;

            //Creating a variable to hold raycast hit data

            RaycastHit hit;

            //Checking if a raycast hit the enemy

            bool raycast = Physics.Raycast(hearinAudioSource.transform.position, direction, out hit, enemyDataSO.hearRadius);

            Debug.DrawRay(hearinAudioSource.transform.position, enemyNavMesh.transform.position, Color.blue);

            //If raycast hit the enemy, the enemy goes to the audio source

            if (raycast)
            {
                //enemyNavMesh.SetDestination(hearinAudioSource.transform.position);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// If health is below or equal to 0, enemy deactivites
    /// </summary>
    /// <returns></returns>

    public bool ShouldEnemyDie()
    {
        if (enemy.EnemyHP <= 0)
        {
            enemy.gameObject.SetActive(false);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Enemy searches enemy location when it is not attacking, chasing, or following the player as result of sensing
    /// </summary>
    /// <returns></returns>

    public bool ShouldSearchPlayerLocation()
    {
        return !IsPlayerInAttackRange() && !IsPlayerInChaseRange() && !IsPlayerInFollowRange();
    }
}
