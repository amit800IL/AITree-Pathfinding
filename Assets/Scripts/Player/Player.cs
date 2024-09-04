using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float HP;

    [SerializeField] private Rigidbody playerRigidBody;
    [field: SerializeField] public float Damage { get; private set; }

    private void Start()
    {
        AttackNode.OnPlayerAttacked += TakeDamage;
    }

    private void OnDisable()
    {
        AttackNode.OnPlayerAttacked -= TakeDamage;
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;

        playerRigidBody.AddForce(new Vector3(5, 0, 0));

        if (HP <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        Debug.Log(HP);
    }
}
