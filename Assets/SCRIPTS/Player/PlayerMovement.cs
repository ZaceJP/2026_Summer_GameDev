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

        Camera cam = Camera.main;

        // CAMERA-RELATIVE DIRECTIONS
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        // Ignore vertical tilt
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Movement relative to camera
        Vector3 moveDirection =
            forward * moveInput.y +
            right * moveInput.x;

        // MOVE
        controller.Move(
            moveDirection *
            stats.GetMoveSpeed() *
            Time.deltaTime
        );

        // ROTATE CHARACTER
        if (moveDirection.sqrMagnitude > 0.01f)
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