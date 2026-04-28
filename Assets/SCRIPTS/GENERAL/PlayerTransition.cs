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

        // 1. Disable movement and physics
        if (movement != null) movement.EnableMovement(false);
        if (cc != null) cc.enabled = false;

        // 2. Move the player
        player.transform.position = targetPosition;

        // 3. CRITICAL: Tell Unity to update physics positions immediately
        Physics.SyncTransforms();

        // 4. Wait a tiny bit for the camera or other scripts to catch up
        yield return new WaitForFixedUpdate();

        // 5. Re-enable everything
        if (cc != null) cc.enabled = true;
        if (movement != null) movement.EnableMovement(true);

        isTransitioning = false;
        Debug.Log($"Teleported to {targetPosition}. Current Pos: {player.transform.position}");
    }
}