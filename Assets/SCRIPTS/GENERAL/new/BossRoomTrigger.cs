using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Prevent triggering multiple times if the player steps back and forth
        if (hasTriggered) return;

        // Check if the object entering the room is the Player
        // Option A: Check by tag (Make sure your Player prefab has the "Player" tag set in Unity)
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            TriggerGameClear();
        }
        // Option B fallback: Check if the object contains your PlayerStats component
        else if (other.GetComponent<PlayerStats>() != null)
        {
            hasTriggered = true;
            TriggerGameClear();
        }
    }

    private void TriggerGameClear()
    {
        Debug.Log("Player entered the Boss Room! Stage Clear triggered.");

        if (GameEndManager.Instance != null)
        {
            // Call the combined manager with the GameClear state
            GameEndManager.Instance.TriggerEndScreen(GameEndState.GameClear);
        }
        else
        {
            Debug.LogWarning("GameEndManager instance not found in scene!");
        }
    }
}