using UnityEngine;
using System.Collections.Generic;

public class DungeonRoom : MonoBehaviour
{
    [HideInInspector] public Vector2Int gridPosition;
    [HideInInspector] public bool isBossRoom;
    [HideInInspector] public bool isStartRoom;
    [HideInInspector] public bool isCleared = false;

    private Dictionary<Direction, DungeonRoom> neighbours = new Dictionary<Direction, DungeonRoom>();
    private Dictionary<Direction, Door> doors = new Dictionary<Direction, Door>();
    private int enemyCount = 0;

    // Called by DungeonGenerator immediately after instantiation
    // so doors dictionary is ready before RefreshDoors runs
    public void Init()
    {
        Door[] allDoors = GetComponentsInChildren<Door>(true);
        foreach (Door door in allDoors)
            doors[door.direction] = door;
    }

    private void Start()
    {
        enemyCount = GetComponentsInChildren<EnemyController>().Length;
        Debug.Log($"Room {gridPosition} — enemies: {enemyCount}");

        if (isStartRoom || enemyCount == 0)
            ClearRoom();
        else
            LockAllDoors();
    }

    public void SetNeighbour(Direction dir, DungeonRoom room)
    {
        neighbours[dir] = room;
    }

    public DungeonRoom GetNeighbour(Direction dir)
    {
        neighbours.TryGetValue(dir, out DungeonRoom room);
        return room;
    }

    public void RefreshDoors()
    {
        foreach (var kvp in doors)
        {
            Direction dir = kvp.Key;
            Door door = kvp.Value;

            if (neighbours.ContainsKey(dir) && neighbours[dir] != null)
                door.gameObject.SetActive(true);
            else
                door.gameObject.SetActive(false);
        }
    }

    public void OnEnemyDied()
    {
        enemyCount--;
        if (enemyCount <= 0)  // ← fixed braces
        {
            Debug.Log($"Room {gridPosition} cleared!");
            ClearRoom();
        }
    }

    void LockAllDoors()
    {
        foreach (var door in doors.Values)
            door.LockDoor();
    }

    void ClearRoom()
    {
        isCleared = true;
        foreach (var door in doors.Values)
            door.UnlockDoor();
    }
}