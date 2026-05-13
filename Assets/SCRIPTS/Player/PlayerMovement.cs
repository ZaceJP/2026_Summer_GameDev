using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector2 moveInput;

    private Vector3 velocity;

    public float gravity = -25f;

    private bool canMove = true;

    PlayerStats stats;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        if (!canMove) return;

        // MOVE
        Vector3 moveDirection =
            new Vector3(moveInput.x, 0f, moveInput.y);

        controller.Move(
            moveDirection * stats.GetMoveSpeed() * Time.deltaTime);

        // ROTATE
        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }

        // GRAVITY
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void EnableMovement(bool value)
    {
        canMove = value;
    }
}