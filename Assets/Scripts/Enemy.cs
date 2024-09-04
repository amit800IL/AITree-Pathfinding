using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public float EnemyHP { get; private set; }
    [SerializeField] private Rigidbody enemyRigidBody;

    void Start()
    {
        PlayerController.onEnemyAttacked += TakeDamage;
    }

    void OnDisable()
    {
        PlayerController.onEnemyAttacked -= TakeDamage;
    }

    private void TakeDamage(Enemy enemy, float damage)
    {
        if (enemy == this)
        {
            EnemyHP -= damage;

            enemyRigidBody.AddForce(new Vector3(5, 0, 0));

            Debug.Log("Enemy HP: " + EnemyHP);
        }
    }
}
