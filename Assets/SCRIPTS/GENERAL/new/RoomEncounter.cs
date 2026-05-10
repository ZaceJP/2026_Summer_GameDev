using UnityEngine;

public class RoomEncounter : MonoBehaviour
{
    [SerializeField] private DoorBlocker[] doors;

    private int enemyCount;
    private bool roomLocked;
    private bool roomCleared;

    private void Awake()
    {
        if (doors == null || doors.Length == 0)
            doors = GetComponentsInChildren<DoorBlocker>(true);

        EnemyController[] enemies = GetComponentsInChildren<EnemyController>(true);
        enemyCount = enemies.Length;

    }

    private void Start()
    {
        UnlockAllDoors();

        if (enemyCount <= 0)
        {
            Debug.Log("Room is clear");
            ClearRoom();
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered trigger: " + other.name);

        if (roomCleared)
        {
            Debug.Log("Room already cleared");
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Not player");
            return;
        }

        Debug.Log("PLAYER ENTERED ROOM");

        BeginEncounter();
    }

    private void BeginEncounter()
    {
        if (roomLocked || roomCleared)
            return;

        if (enemyCount > 0)
        {
            roomLocked = true;
            Debug.Log("Locking room with " + enemyCount + " enemies.");
            LockAllDoors();
        }
    }

    public void OnEnemyDied()
    {
        if (roomCleared)
            return;

        enemyCount--;
        Debug.Log("RoomEncounter - Enemy died, remaining: " + enemyCount);
        if (enemyCount <= 0)
            ClearRoom();
    }

    private void ClearRoom()
    {
        roomCleared = true;
        roomLocked = false;

        Debug.Log("ROOM CLEARED: " + gameObject.name);

        UnlockAllDoors();
    }

    private void LockAllDoors()
    {
        if (doors == null) return;

        foreach (DoorBlocker door in doors)
        {
            if (door != null)
                door.LockDoor();
        }
    }

    private void UnlockAllDoors()
    {
        if (doors == null) return;

        foreach (DoorBlocker door in doors)
        {
            if (door != null)
                door.UnlockDoor();
        }
    }
}