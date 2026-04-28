using UnityEngine;

public class Door : MonoBehaviour
{
    public Direction direction;   // set this in the Inspector per door object

    private bool isLocked = true;
    private bool used = false;

    public void LockDoor()
    {
        isLocked = true;
    }

    public void UnlockDoor()
    {
        isLocked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLocked || used) return;
        if (!other.CompareTag("Player")) return;

        used = true;

        DungeonRoom currentRoom = GetComponentInParent<DungeonRoom>();
        DungeonRoom nextRoom = currentRoom?.GetNeighbour(direction);

        if (nextRoom == null)
        {
            Debug.LogWarning("Door has no neighbour room!");
            used = false;
            return;
        }

        // Find the entry spawn point on the opposite side of the next room
        Direction entryDir = direction.Opposite();
        SpawnPoint[] spawnPoints = nextRoom.GetComponentsInChildren<SpawnPoint>();
        SpawnPoint target = null;

        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp.direction == entryDir)
            {
                target = sp;
                break;
            }
        }

        // Fallback to any spawn point
        if (target == null)
            target = nextRoom.GetComponentInChildren<SpawnPoint>();

        if (target == null)
        {
            Debug.LogError($"No SpawnPoint found in room {nextRoom.gridPosition}!");
            used = false;
            return;
        }

        if (PlayerTransition.Instance == null)
        {
            Debug.LogError("PlayerTransition.Instance is NULL — is PlayerTransition in the scene?");
            used = false;
            return;
        }

        PlayerTransition.Instance.TransitionTo(target.transform.position, nextRoom);

        // Reset used after a delay so the door works if the player comes back
        Invoke(nameof(ResetUsed), 2f);
    }

    void ResetUsed() => used = false;
}