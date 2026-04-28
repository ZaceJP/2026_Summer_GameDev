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
        SpawnInitialRoom();
    }

    void SpawnInitialRoom()
    {
        SpawnRoomObject();
        SpawnPlayerAtCurrentRoom();
    }

    void SpawnRoomObject()
    {
        GameObject roomToSpawn = roomCount >= roomsBeforeBoss
            ? bossRoomPrefab
            : roomPrefabs[Random.Range(0, roomPrefabs.Length)];

        currentRoom = Instantiate(roomToSpawn, Vector3.zero, Quaternion.identity);
        roomCount++;
    }

    void SpawnPlayerAtCurrentRoom()
    {
        SpawnPoint spawn = currentRoom.GetComponentInChildren<SpawnPoint>();

        if (spawn == null)
        {
            Debug.LogError("RoomManager: No SpawnPoint found in room!");
            return;
        }

        if (player == null)
        {
            player = Instantiate(GetPlayerPrefab(), spawn.transform.position, Quaternion.identity);
        }
    }

    public void LoadNextRoom()
    {
        StartCoroutine(TransitionToNextRoom());
    }

    IEnumerator TransitionToNextRoom()
    {
        PlayerMovement movement = player?.GetComponent<PlayerMovement>();
        CharacterController cc = player?.GetComponent<CharacterController>();
        if (movement != null) movement.EnableMovement(false);
        if (cc != null) cc.enabled = false;

        yield return null;

        Destroy(currentRoom);
        SpawnRoomObject();

        SpawnPoint spawn = currentRoom.GetComponentInChildren<SpawnPoint>();
        if (spawn == null)
        {
            Debug.LogError("RoomManager: No SpawnPoint in new room!");
            if (cc != null) cc.enabled = true;
            if (movement != null) movement.EnableMovement(true);
            yield break;
        }

        Debug.Log("SpawnPoint world position: " + spawn.transform.position);
        Debug.Log("Player position BEFORE teleport: " + player.transform.position);

        player.transform.position = spawn.transform.position;

        yield return null;

        Debug.Log("Player position AFTER teleport: " + player.transform.position);

        Debug.Log("SpawnPoint found on: " + spawn.gameObject.name +
          " | parent: " + spawn.transform.parent?.name +
          " | local pos: " + spawn.transform.localPosition +
          " | world pos: " + spawn.transform.position);

        if (cc != null) cc.enabled = true;
        if (movement != null) movement.EnableMovement(true);
    }

    GameObject GetPlayerPrefab()
    {
        return Resources.Load<GameObject>("Player_Warrior");
    }
}