using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static event Action<Enemy, float> onEnemyAttacked;
    private Vector3 inputPosition;
    private AIFinalProject inputActions;
    [SerializeField] private Player player;
    [SerializeField] private Rigidbody playerRigibBody;
    [SerializeField] private int playerSpeed;
    [SerializeField] private AudioSource footSteps;
    [SerializeField] private LayerMask enemyMask;

    private void Start()
    {
        inputActions = new AIFinalProject();

        inputActions.Enable();

        inputActions.Player.Move.performed += OnPlayerMove;
        inputActions.Player.Attack.performed += OnPlayerAttack;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnPlayerMove;
        inputActions.Player.Attack.performed -= OnPlayerAttack;
    }

    private void OnPlayerMove(InputAction.CallbackContext inputContext)
    {
        inputPosition = inputContext.ReadValue<Vector2>();

        inputPosition = new Vector3(inputPosition.x, 0, inputPosition.y);

        if (inputContext.performed)
        {
            playerRigibBody.velocity = inputPosition * playerSpeed;
            footSteps.Play();
        }
    }

    private void OnPlayerAttack(InputAction.CallbackContext inputContext)
    {
        if (inputContext.performed)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            bool raycast = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, enemyMask);

            float distance = Vector3.Distance(transform.position, hit.point);

            if (raycast && distance <= 20)
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                onEnemyAttacked?.Invoke(enemy, player.Damage);
            }
        }
    }
}

