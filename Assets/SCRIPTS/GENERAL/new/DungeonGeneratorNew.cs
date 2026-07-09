using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnedRoom
{
    public Vector2Int gridPos;
    public GameObject roomObject;

    public SpawnedRoom(Vector2Int pos, GameObject obj)
    {
        gridPos = pos;
        roomObject = obj;
    }
}


public class DungeonGeneratorNew : MonoBehaviour
{
    [Header("Special Rooms")]
    public GameObject startRoomPrefab;
    public GameObject bossRoomPrefab;

    [Header("Special Dead End Rooms")]
    public GameObject[] treasureRoomPrefabs;

    [Range(0f, 1f)]
    public float treasureRoomChance = 0.8f;

    [Header("Normal Rooms")]
    public GameObject[] roomPrefabs;

    [Header("Hallways")]
    public GameObject hallwayPrefab;

    [Header("Hero Selection")]
    public HeroSelection heroSelection;

    [Header("Settings")]
    public int roomCount = 10;
    public float spacing = 20f;

    private List<Vector2Int> roomPositions = new List<Vector2Int>();
    private List<SpawnedRoom> spawnedRooms = new List<SpawnedRoom>();

    void Start()
    {
        GenerateDungeon();
    }


    
    //##########################################################
    //###############      DUNGEON GENERATION   ################
    //##########################################################
 

    void GenerateDungeon()
    {
        if (startRoomPrefab == null)
        {
            Debug.LogError("Missing Start Room Prefab");
            return;
        }

        if (bossRoomPrefab == null)
        {
            Debug.LogError("Missing Boss Room Prefab");
            return;
        }

        if (roomPrefabs.Length == 0)
        {
            Debug.LogError("No normal room prefabs assigned");
            return;
        }

        if (hallwayPrefab == null)
        {
            Debug.LogError("Missing Hallway Prefab");
            return;
        }

        roomPositions.Clear();
        spawnedRooms.Clear();

        Vector2Int startPos = Vector2Int.zero;

        roomPositions.Add(startPos);

        // Spawn START room
        GameObject startRoom =
            SpawnSpecificRoom(startRoomPrefab, startPos);

        spawnedRooms.Add(
            new SpawnedRoom(startPos, startRoom));

        // Spawn selected hero
        SpawnSelectedHero(startRoom);

        // Generate dungeon
        for (int i = 1; i < roomCount; i++)
        {
            Queue<SpawnedRoom> roomsToProcess =
            new Queue<SpawnedRoom>();

            roomsToProcess.Enqueue(
                new SpawnedRoom(startPos, startRoom));

            while (roomsToProcess.Count > 0 &&
                   spawnedRooms.Count < roomCount)
            {
                SpawnedRoom[] roomArray =
                 roomsToProcess.ToArray();

                SpawnedRoom currentNode =
                    roomArray[Random.Range(0, roomArray.Length)];

                roomsToProcess =
                    new Queue<SpawnedRoom>(
                        System.Array.FindAll(
                            roomArray,
                            r => r != currentNode));

                GameObject currentRoom =
                    currentNode.roomObject;

                Vector2Int currentPos =
                    currentNode.gridPos;

                RoomDoors currentDoors =
                    currentRoom.GetComponent<RoomDoors>();

                if (currentDoors == null)
                    continue;

                List<DoorDirection> availableDoors =
                    currentDoors.GetAvailableDoors();

                bool spawnedAtLeastOneRoom = false;

                foreach (DoorDirection chosenDoor in availableDoors)
                {
                    if (spawnedAtLeastOneRoom &&
                          Random.value > 0.4f)
                    {
                        continue;
                    }

                    Vector2Int direction =
                        DirectionToVector(chosenDoor);

                    Vector2Int newPos =
                        currentPos + direction;

                    if (roomPositions.Contains(newPos))
                        continue;

                    GameObject nextRoom =
                        SpawnCompatibleRoom(newPos, chosenDoor);

                    if (nextRoom == null)
                        continue;

                    roomPositions.Add(newPos);

                    SpawnedRoom spawned =
                        new SpawnedRoom(newPos, nextRoom);

                    spawnedRooms.Add(spawned);

                    SpawnHallway(
                        currentRoom,
                        nextRoom,
                        direction);

                    roomsToProcess.Enqueue(
                        new SpawnedRoom(newPos, nextRoom));

                    spawnedAtLeastOneRoom = true;

                    if (spawnedRooms.Count >= roomCount)
                        break;
                }
            }
        }

        ReplaceFurthestRoomWithBossRoom();
        ReplaceDeadEnds();
        UpdateRoomConnections();

    }

    void SpawnSelectedHero(GameObject startRoom)
    {
        if (heroSelection == null)
        {
            Debug.LogError("HeroSelection reference missing.");
            return;
        }

        if (heroSelection.selectedHero == null)
        {
            Debug.LogError("No hero selected.");
            return;
        }

        Transform spawnPoint =
            startRoom.transform.Find("PlayerSpawnPoint");

        if (spawnPoint == null)
        {
            Debug.LogError("PlayerSpawnPoint not found in Start Room.");
            return;
        }

        GameObject player = Instantiate(
    heroSelection.selectedHero.prefab,
    spawnPoint.position,
    spawnPoint.rotation);

        PlayerInitializer initializer =
            player.GetComponent<PlayerInitializer>();

        if (initializer != null)
        {
            initializer.heroDefinition = heroSelection.selectedHero;
            initializer.Initialize();
            Debug.Log("User selected hero spawned and initialized.");
        }
        else
        {
            Debug.LogError("Player prefab missing PlayerInitializer!");
        }
    }

    void ReplaceFurthestRoomWithBossRoom()
    {
        SpawnedRoom furthestRoom = null;

        float furthestDistance = -1f;

        foreach (SpawnedRoom room in spawnedRooms)
        {
            float dist =
                Vector2Int.Distance(Vector2Int.zero, room.gridPos);

            if (dist > furthestDistance)
            {
                furthestDistance = dist;
                furthestRoom = room;
            }
        }

        if (furthestRoom == null)
            return;

        Vector3 pos = furthestRoom.roomObject.transform.position;
        Quaternion rot = furthestRoom.roomObject.transform.rotation;

        Destroy(furthestRoom.roomObject);

        GameObject bossRoom =
            Instantiate(bossRoomPrefab, pos, rot);

        furthestRoom.roomObject = bossRoom;
    }


    //##########################################################
    //##########################################################
    //###############     DEAD END REPLACEMENT    ##############
    //##########################################################
    //##########################################################
    void ReplaceDeadEnds()
    {
        foreach (SpawnedRoom room in spawnedRooms)
        {
            // Skip start & boss room
            if (room.roomObject == null)
                continue;

            if (room.roomObject.name.Contains(startRoomPrefab.name))
                continue;

            if (room.roomObject.name.Contains(bossRoomPrefab.name))
                continue;

            int connections = CountRoomConnections(room.gridPos);

            // Dead end = only 1 connection
            if (connections == 1)
            {
                if (Random.value <= treasureRoomChance)
                {
                    ReplaceWithTreasureRoom(room);
                }
            }
        }
    }

    //###############    Check for Connections for the Dead End  ##############

    void UpdateRoomConnections()
    {
        foreach (SpawnedRoom room in spawnedRooms)
        {
            RoomDoors doors =
                room.roomObject.GetComponent<RoomDoors>();

            if (doors == null)
                continue;

            Vector2Int pos = room.gridPos;

            doors.SetConnection(
                DoorDirection.North,
                roomPositions.Contains(pos + Vector2Int.up));

            doors.SetConnection(
                DoorDirection.South,
                roomPositions.Contains(pos + Vector2Int.down));

            doors.SetConnection(
                DoorDirection.East,
                roomPositions.Contains(pos + Vector2Int.right));

            doors.SetConnection(
                DoorDirection.West,
                roomPositions.Contains(pos + Vector2Int.left));
        }
    }
    int CountRoomConnections(Vector2Int pos)
    {
        int count = 0;

        Vector2Int[] dirs =
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

        foreach (Vector2Int dir in dirs)
        {
            if (roomPositions.Contains(pos + dir))
            {
                count++;
            }
        }

        return count;
    }

    DoorDirection GetConnectionDirection(Vector2Int pos)
    {
        if (roomPositions.Contains(pos + Vector2Int.up))
            return DoorDirection.North;

        if (roomPositions.Contains(pos + Vector2Int.down))
            return DoorDirection.South;

        if (roomPositions.Contains(pos + Vector2Int.right))
            return DoorDirection.East;

        if (roomPositions.Contains(pos + Vector2Int.left))
            return DoorDirection.West;

        return DoorDirection.North;
    }

    //###############   Actual Replace last rooms with treasure rooms  ##############
    void ReplaceWithTreasureRoom(SpawnedRoom room)
    {
        if (treasureRoomPrefabs.Length == 0)
            return;

        // Which side connects to another room?
        DoorDirection requiredDoor =
            GetConnectionDirection(room.gridPos);

        // Find treasure rooms with matching door
        List<GameObject> validRooms =
            new List<GameObject>();

        foreach (GameObject prefab in treasureRoomPrefabs)
        {
            RoomDoors doors =
                prefab.GetComponent<RoomDoors>();

            if (doors != null &&
                doors.HasDoor(requiredDoor))
            {
                validRooms.Add(prefab);
            }
        }

        if (validRooms.Count == 0)
        {
            Debug.LogWarning(
                $"No treasure room found with {requiredDoor} door.");

            return;
        }

        Vector3 pos = room.roomObject.transform.position;
        Quaternion rot = room.roomObject.transform.rotation;

        Destroy(room.roomObject);

        GameObject selectedPrefab =
            validRooms[
                Random.Range(0, validRooms.Count)];

        GameObject newRoom =
            Instantiate(selectedPrefab, pos, rot);

        room.roomObject = newRoom;
    }

    GameObject SpawnSpecificRoom(GameObject prefab, Vector2Int gridPos)
    {
        Vector3 worldPos =
            new Vector3(gridPos.x * spacing, 0, gridPos.y * spacing);

        return Instantiate(prefab, worldPos, Quaternion.identity);
    }
    GameObject SpawnCompatibleRoom(
        Vector2Int gridPos,
        DoorDirection requiredDoor)
    {
        DoorDirection neededDoor =
            GetOppositeDoor(requiredDoor);

        List<GameObject> validRooms =
            new List<GameObject>();

        foreach (GameObject roomPrefab in roomPrefabs)
        {
            RoomDoors doors =
                roomPrefab.GetComponent<RoomDoors>();

            if (doors != null &&
                doors.HasDoor(neededDoor))
            {
                validRooms.Add(roomPrefab);
            }
        }

        if (validRooms.Count == 0)
        {
            Debug.LogError(
                $"No compatible rooms found with {neededDoor} door.");

            return null;
        }

        GameObject selected =
            validRooms[Random.Range(0, validRooms.Count)];

        return SpawnSpecificRoom(selected, gridPos);
    }

    //##########################################################
    //##################   SPAWN HALLWAYS   ####################
    //##########################################################
    void SpawnHallway(GameObject roomA, GameObject roomB, Vector2Int direction)
    {
        RoomDoors doorsA = roomA.GetComponent<RoomDoors>();
        RoomDoors doorsB = roomB.GetComponent<RoomDoors>();

        if (doorsA == null || doorsB == null)
        {
            Debug.LogError("Missing RoomDoors component.");
            return;
        }

        DoorDirection doorAType =
            GetDoorDirection(direction);

        DoorDirection doorBType =
            GetDoorDirection(-direction);

        Transform doorA = doorsA.GetDoor(doorAType);
        Transform doorB = doorsB.GetDoor(doorBType);

        if (doorA == null || doorB == null)
        {
            Debug.LogError("Missing door transform.");
            return;
        }

        Vector3 start = doorA.position;
        Vector3 end = doorB.position;

        Vector3 midpoint = (start + end) * 0.5f;

        Vector3 lookDir =
            (end - start).normalized;

        Quaternion rotation =
            Quaternion.LookRotation(lookDir);

        Instantiate(hallwayPrefab, midpoint, rotation);
    }

    //##################  HELPER for the DOOR DIRECTION  ####################
    DoorDirection GetDoorDirection(Vector2Int dir)
    {
        if (dir == Vector2Int.up)
            return DoorDirection.North;

        if (dir == Vector2Int.down)
            return DoorDirection.South;

        if (dir == Vector2Int.right)
            return DoorDirection.East;

        if (dir == Vector2Int.left)
            return DoorDirection.West;

        return DoorDirection.North;
    }

    Vector2Int DirectionToVector(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.North => Vector2Int.up,
            DoorDirection.South => Vector2Int.down,
            DoorDirection.East => Vector2Int.right,
            DoorDirection.West => Vector2Int.left,
            _ => Vector2Int.zero
        };
    }

    DoorDirection GetOppositeDoor(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.North => DoorDirection.South,
            DoorDirection.South => DoorDirection.North,
            DoorDirection.East => DoorDirection.West,
            DoorDirection.West => DoorDirection.East,
            _ => DoorDirection.North
        };
    }
}