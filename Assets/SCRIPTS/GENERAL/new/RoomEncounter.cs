using System.Collections.Generic;
using UnityEngine;

public class RoomEncounter : MonoBehaviour
{
    [SerializeField] private DoorBlocker[] doors;

    private int enemyCount;
    private bool roomLocked;
    private bool roomCleared;
    private bool isReady = false;

    private void Awake()
    {
      

        EnemyController[] enemies = GetComponentsInChildren<EnemyController>(true);
        enemyCount = enemies.Length;

    }

    private void Start()
    {
        if (doors == null || doors.Length == 0)
            doors = GetComponentsInChildren<DoorBlocker>(true);

        UnlockAllDoors();

        if (enemyCount <= 0)
        {
            Debug.Log("Room is clear");
            roomCleared = true; // mark cleared without triggering rewards on load
            return;
        }

        isReady = true; // only arm the trigger after setup is done
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReady) return; // ignore any trigger hits during startup

        Debug.Log("Something entered trigger: " + other.name);
        if (roomCleared) return;
        if (!other.CompareTag("Player")) return;

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

        List<UpgradeData> rewards =
        RewardManager.Instance.GenerateRewards();

        RewardUI.Instance.ShowRewards(rewards);
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