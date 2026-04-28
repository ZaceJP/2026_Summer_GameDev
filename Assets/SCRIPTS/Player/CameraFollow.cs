using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset;
    public float followSpeed = 5f;

    [Header("Look-Ahead")]
    public float lookAheadDistance = 3f;   // how far ahead the camera peeks
    public float lookAheadSpeed = 4f;       // how fast it shifts ahead
    public float lookAheadReturnSpeed = 2f; // how fast it returns when idle

    private Transform target;
    private Vector3 currentLookAhead;

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            target = player.transform;
        else
            Debug.LogWarning("CameraFollow: No GameObject with tag 'Player' found.");
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        // Get the player's movement direction from input (works regardless of PlayerMovement internals)
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(inputX, 0f, inputZ).normalized;

        // Calculate the desired look-ahead offset (X and Z only, ignore Y)
        Vector3 desiredLookAhead = inputDir * lookAheadDistance;

        // If moving, shift ahead — if idle, drift back to center
        float blendSpeed = inputDir.magnitude > 0.1f ? lookAheadSpeed : lookAheadReturnSpeed;
        currentLookAhead = Vector3.Lerp(currentLookAhead, desiredLookAhead, blendSpeed * Time.deltaTime);

        Vector3 targetPosition = target.position + offset + currentLookAhead;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}