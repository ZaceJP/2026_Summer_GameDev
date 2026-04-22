using UnityEngine;

public class Room : MonoBehaviour
{
    private int enemyCount;
    public Door exitDoor;

    void Start()
    {
        
        EnemyController[] enemies = GetComponentsInChildren<EnemyController>();
        enemyCount = enemies.Length;
        Debug.Log("Enemies in room: " + enemyCount);

        if (exitDoor != null)
        {
            exitDoor.LockDoor();
        }
    }

    public void OnEnemyDied()
    {
        enemyCount--;

        if (enemyCount <= 0)
        {
            RoomCleared();
        }
    }

    void RoomCleared()
    {
        Debug.Log("Room Cleared!");

        if (exitDoor != null)
        {
            exitDoor.UnlockDoor();
        }
    }
}