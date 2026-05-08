using UnityEngine;
using System.Collections;

public class PlayerTransition : MonoBehaviour
{
    public static PlayerTransition Instance;

    [SerializeField] private GameObject player; // Assign in Inspector or via SetPlayer
    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetPlayer(GameObject p)
    {
        player = p;
    }

    public void TransitionTo(Vector3 targetPosition, DungeonRoom nextRoom)
    {
        if (isTransitioning) return;
        if (player == null)
        {
            Debug.LogError("PlayerTransition: No player object assigned!");
            return;
        }
        StartCoroutine(DoTransition(targetPosition));
    }

    IEnumerator DoTransition(Vector3 targetPosition)
    {
        isTransitioning = true;
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        CharacterController cc = player.GetComponent<CharacterController>();

        if (movement != null) movement.EnableMovement(false);
        if (cc != null) cc.enabled = false;

        player.transform.position = targetPosition;
        Physics.SyncTransforms();

        // ← Snap camera instantly
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null) cam.SnapToTarget();

        yield return new WaitForFixedUpdate();

        if (cc != null) cc.enabled = true;
        if (movement != null) movement.EnableMovement(true);
        isTransitioning = false;
    }
}