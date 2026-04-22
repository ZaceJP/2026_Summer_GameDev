using UnityEngine;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    public GameObject[] roomPrefabs;
    public GameObject bossRoomPrefab;

    private GameObject currentRoom;
    private int roomCount = 0;
    public int roomsBeforeBoss = 5;

    private GameObject player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // For the very first room, we spawn everything
        SpawnInitialRoom();
    }

    void SpawnInitialRoom()
    {
        SpawnRoomObject();
        SpawnPlayerAtCurrentRoom();
    }

    // This handles just the Room Instantiation logic
    void SpawnRoomObject()
    {
        GameObject roomToSpawn;

        if (roomCount >= roomsBeforeBoss)
        {
            roomToSpawn = bossRoomPrefab;
        }
        else
        {
            int index = Random.Range(0, roomPrefabs.Length);
            roomToSpawn = roomPrefabs[index];
        }

        currentRoom = Instantiate(roomToSpawn, Vector3.zero, Quaternion.identity);
        roomCount++;
    }

    // Logic to either create the player or just move them
    void SpawnPlayerAtCurrentRoom()
    {
        SpawnPoint spawn = currentRoom.GetComponentInChildren<SpawnPoint>();

        if (player == null)
        {
            GameObject prefab = GetPlayerPrefab();
            player = Instantiate(prefab, spawn.transform.position, Quaternion.identity);
        }
        else
        {
            // If the player already exists, we use the coroutine to move them safely
            StartCoroutine(MovePlayerToSpawn(spawn.transform.position));
        }
    }

    public void LoadNextRoom()
    {
        Destroy(currentRoom);
        SpawnRoomObject();
        SpawnPlayerAtCurrentRoom();
    }

    IEnumerator MovePlayerToSpawn(Vector3 targetPosition)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        CharacterController cc = player.GetComponent<CharacterController>();

        // 1. Disable movement logic and the CharacterController physics
        if (movement != null) movement.EnableMovement(false);
        if (cc != null) cc.enabled = false;

        // 2. Wait for the end of the frame to ensure the controller is truly disabled
        yield return new WaitForEndOfFrame();

        // 3. Move player
        player.transform.position = targetPosition;
        Debug.Log("Player moved to: " + targetPosition);

        // 4. Wait one more frame for the Transform to sync
        yield return null;

        // 5. Re-enable
        if (cc != null) cc.enabled = true;
        if (movement != null) movement.EnableMovement(true);
    }

    GameObject GetPlayerPrefab()
    {
        return Resources.Load<GameObject>("Player_Warrior");
    }
}