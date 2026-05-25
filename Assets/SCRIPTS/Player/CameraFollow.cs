using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Vector3 offset;
    public float followSpeed = 5f;

    [Header("First Person")]
    public bool firstPersonMode = false;

    public Vector3 firstPersonOffset =
        new Vector3(0f, 1.7f, 0f);

    public float mouseSensitivity = 2f;

    private float yaw;
    private float pitch;


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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            firstPersonMode = !firstPersonMode;

            Cursor.lockState =
                firstPersonMode
                ? CursorLockMode.Locked
                : CursorLockMode.None;

            Cursor.visible = !firstPersonMode;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        // FIRST PERSON
        if (firstPersonMode)
        {
            HandleFirstPerson();
            return;
        }

        // THIRD PERSON

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDir =
            new Vector3(inputX, 0f, inputZ).normalized;

        Vector3 desiredLookAhead =
            inputDir * lookAheadDistance;

        float blendSpeed =
            inputDir.magnitude > 0.1f
            ? lookAheadSpeed
            : lookAheadReturnSpeed;

        currentLookAhead =
            Vector3.Lerp(
                currentLookAhead,
                desiredLookAhead,
                blendSpeed * Time.deltaTime
            );

        Vector3 targetPosition =
            target.position +
            offset +
            currentLookAhead;

        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    void HandleFirstPerson()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.position =
            target.position +
            firstPersonOffset;

        transform.rotation =
            Quaternion.Euler(pitch, yaw, 0f);
    }

    public void SnapToTarget()
    {
        if (target == null) return;
        transform.position = target.position + offset;
        currentLookAhead = Vector3.zero;
    }
}