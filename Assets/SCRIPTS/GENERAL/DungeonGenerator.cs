using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance;

    [Header("Rooms")]
    public GameObject[] normalRoomPrefabs;
    public GameObject bossRoomPrefab;
    public GameObject startRoomPrefab;

    [Header("Layout")]
    [Tooltip("Total number of rooms including start and boss")]
    public int totalRooms = 10;
    public float roomSpacing = 60f; // world units between room centers

    [Header("Player")]
    public HeroSelection heroSelection;

    // Grid ? room lookup
    private Dictionary<Vector2Int, DungeonRoom> roomGrid = new Dictionary<Vector2Int, DungeonRoom>();
    private DungeonRoom startRoom;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateDungeon();
        SpawnPlayer();
    }

    void GenerateDungeon()
    {
        List<Vector2Int> layout = GenerateLayout();
        PlaceRooms(layout);
        ConnectDoors();
    }

    // Random walk to carve room positions on a grid
    List<Vector2Int> GenerateLayout()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        Vector2Int current = Vector2Int.zero;
        positions.Add(current);
        visited.Add(current);

        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        int attempts = 0;
        while (positions.Count < totalRooms && attempts < 1000)
        {
            attempts++;
            // Bias towards unvisited neighbours of existing rooms
            Vector2Int origin = positions[Random.Range(0, positions.Count)];
            Vector2Int dir = directions[Random.Range(0, directions.Length)];
            Vector2Int next = origin + dir;

            if (!visited.Contains(next))
            {
                positions.Add(next);
                visited.Add(next);
            }
        }

        return positions;
    }

    void PlaceRooms(List<Vector2Int> layout)
    {
        for (int i = 0; i < layout.Count; i++)
        {
            Vector2Int gridPos = layout[i];
            Vector3 worldPos = new Vector3(gridPos.x * roomSpacing, 0, gridPos.y * roomSpacing);

            GameObject prefab;
            if (i == 0)
                prefab = startRoomPrefab != null ? startRoomPrefab : normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)];
            else if (i == layout.Count - 1)
                prefab = bossRoomPrefab;
            else
                prefab = normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)];

            GameObject roomObj = Instantiate(prefab, worldPos, Quaternion.identity);
            DungeonRoom room = roomObj.GetComponent<DungeonRoom>();

            if (room == null)
            {
                Debug.LogError($"Room prefab '{prefab.name}' is missing a DungeonRoom component!");
                continue;
            }

            room.Init(); //  register doors immediately before RefreshDoors can run
            room.gridPosition = gridPos;
            room.isBossRoom = (i == layout.Count - 1);
            room.isStartRoom = (i == 0);
            roomGrid[gridPos] = room;

            if (i == 0) startRoom = room;
        }
    }

    // Tell each room which neighbours exist so doors know where to send the player
    void ConnectDoors()
    {
        foreach (var kvp in roomGrid)
        {
            DungeonRoom room = kvp.Value;
            Vector2Int pos = kvp.Key;

            room.SetNeighbour(Direction.North, roomGrid.ContainsKey(pos + Vector2Int.up) ? roomGrid[pos + Vector2Int.up] : null);
            room.SetNeighbour(Direction.South, roomGrid.ContainsKey(pos + Vector2Int.down) ? roomGrid[pos + Vector2Int.down] : null);
            room.SetNeighbour(Direction.West, roomGrid.ContainsKey(pos + Vector2Int.left) ? roomGrid[pos + Vector2Int.left] : null);
            room.SetNeighbour(Direction.East, roomGrid.ContainsKey(pos + Vector2Int.right) ? roomGrid[pos + Vector2Int.right] : null);

            room.RefreshDoors();
        }
    }

    void SpawnPlayer()
    {
        if (startRoom == null) return;

        if (heroSelection == null || heroSelection.selectedHero == null)
        {
            Debug.LogError("DungeonGenerator: No hero selected!");
            return;
        }

        SpawnPoint sp = startRoom.GetComponentInChildren<SpawnPoint>();
        Vector3 spawnPos = sp != null ? sp.transform.position : startRoom.transform.position;

        GameObject player = Instantiate(heroSelection.selectedHero.prefab, spawnPos, Quaternion.identity);

        // Pass hero data before Start() runs on sub-components
        PlayerInitializer initializer = player.GetComponent<PlayerInitializer>();
        if (initializer != null)
            initializer.heroDefinition = heroSelection.selectedHero;

        PlayerTransition.Instance?.SetPlayer(player);

    }

    public DungeonRoom GetRoom(Vector2Int gridPos)
    {
        roomGrid.TryGetValue(gridPos, out DungeonRoom room);
        return room;
    }
}