using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 moveDirection;

    private bool canMove = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update() 
    {
        if (!canMove) return;

        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        if(moveDirection != Vector3.zero) 
        {
            transform.forward = moveDirection;
        }
    }
    public void EnableMovement(bool value)
    {
        canMove = value;
    }

}
