using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance;

    [Header("Rooms")]
    public GameObject[] normalRoomPrefabs;
    public GameObject bossRoomPrefab;
    public GameObject startRoomPrefab;

    [Header("Hallways")]
    public GameObject hallwayPrefab; // a straight hallway, oriented along Z axis by default

    [Header("Layout")]
    public int totalRooms = 10;
    public float roomWidth = 20f;   // X size of a room
    public float roomHeight = 20f;  // Z size of a room
    public float hallwayLength = 10f; // length of hallway between rooms

    [Header("Player")]
    public HeroSelection heroSelection;

    private Dictionary<Vector2Int, DungeonRoom> roomGrid = new();
    private DungeonRoom startRoom;

    void Awake() => Instance = this;

    void Start()
    {
        GenerateDungeon();
        SpawnPlayer();
    }

    void GenerateDungeon()
    {
        List<Vector2Int> layout = GenerateLayout();
        PlaceRooms(layout);
        PlaceHallways();
        ConnectDoors();
    }

    List<Vector2Int> GenerateLayout()
    {
        List<Vector2Int> positions = new();
        HashSet<Vector2Int> visited = new();

        Vector2Int current = Vector2Int.zero;
        positions.Add(current);
        visited.Add(current);

        Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        int attempts = 0;
        while (positions.Count < totalRooms && attempts < 1000)
        {
            attempts++;
            Vector2Int origin = positions[Random.Range(0, positions.Count)];
            Vector2Int dir = dirs[Random.Range(0, dirs.Length)];
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
        // Spacing = room size + hallway length so there's a gap for the hallway prefab
        float spacingX = roomWidth + hallwayLength;
        float spacingZ = roomHeight + hallwayLength;

        for (int i = 0; i < layout.Count; i++)
        {
            Vector2Int gridPos = layout[i];
            Vector3 worldPos = new Vector3(gridPos.x * spacingX, 0, gridPos.y * spacingZ);

            GameObject prefab = i == 0 ? (startRoomPrefab != null ? startRoomPrefab : normalRoomPrefabs[0])
                              : i == layout.Count - 1 ? bossRoomPrefab
                              : normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)];

            GameObject roomObj = Instantiate(prefab, worldPos, Quaternion.identity);
            DungeonRoom room = roomObj.GetComponent<DungeonRoom>();

            if (room == null) { Debug.LogError($"Missing DungeonRoom on {prefab.name}!"); continue; }

            room.Init();
            room.gridPosition = gridPos;
            room.isBossRoom = (i == layout.Count - 1);
            room.isStartRoom = (i == 0);
            roomGrid[gridPos] = room;

            if (i == 0) startRoom = room;
        }
    }

    void PlaceHallways()
{
    HashSet<string> placed = new();

    foreach (var kvp in roomGrid)
    {
        Vector2Int pos = kvp.Key;
        DungeonRoom room = kvp.Value;

        Vector2Int[] checkDirs = { Vector2Int.right, Vector2Int.up };

        foreach (Vector2Int gridDir in checkDirs)
        {
            Vector2Int neighbourPos = pos + gridDir;
            if (!roomGrid.ContainsKey(neighbourPos)) continue;

            string key = $"{pos}-{neighbourPos}";
            if (placed.Contains(key)) continue;
            placed.Add(key);

            DungeonRoom neighbour = roomGrid[neighbourPos];

            Vector3 roomCenter      = room.transform.position;
            Vector3 neighbourCenter = neighbour.transform.position;

            // Horizontal corridor (along X)
            float xLength = Mathf.Abs(neighbourCenter.x - roomCenter.x);
            if (xLength > 0.1f)
            {
                Vector3 xMid = new Vector3(
                    (roomCenter.x + neighbourCenter.x) / 2f,
                    roomCenter.y,
                    roomCenter.z
                );
                Quaternion xRot = Quaternion.FromToRotation(Vector3.right,
                    neighbourCenter.x > roomCenter.x ? Vector3.right : Vector3.left);
                GameObject hx = Instantiate(hallwayPrefab, xMid, xRot);
                Vector3 sx = hx.transform.localScale;
                sx.x = xLength / 2.857f; // your hallway base length
                hx.transform.localScale = sx;
            }

            // Vertical corridor (along Z)
            float zLength = Mathf.Abs(neighbourCenter.z - roomCenter.z);
            if (zLength > 0.1f)
            {
                Vector3 zMid = new Vector3(
                    neighbourCenter.x,
                    roomCenter.y,
                    (roomCenter.z + neighbourCenter.z) / 2f
                );
                Quaternion zRot = Quaternion.FromToRotation(Vector3.right,
                    neighbourCenter.z > roomCenter.z ? Vector3.forward : Vector3.back);
                GameObject hz = Instantiate(hallwayPrefab, zMid, zRot);
                Vector3 sz = hz.transform.localScale;
                sz.x = zLength / 2.857f;
                hz.transform.localScale = sz;
            }
        }
    }
}

    Door GetDoorInDirection(DungeonRoom room, Direction dir)
    {
        Door[] doors = room.GetComponentsInChildren<Door>(true);
        foreach (Door d in doors)
            if (d.direction == dir) return d;
        return null;
    }

    void ConnectDoors()
    {
        foreach (var kvp in roomGrid)
        {
            DungeonRoom room = kvp.Value;
            Vector2Int pos = kvp.Key;

            room.SetNeighbour(Direction.North, roomGrid.GetValueOrDefault(pos + Vector2Int.up));
            room.SetNeighbour(Direction.South, roomGrid.GetValueOrDefault(pos + Vector2Int.down));
            room.SetNeighbour(Direction.West, roomGrid.GetValueOrDefault(pos + Vector2Int.left));
            room.SetNeighbour(Direction.East, roomGrid.GetValueOrDefault(pos + Vector2Int.right));

            room.RefreshDoors();
        }
    }

    void SpawnPlayer()
    {
        if (startRoom == null) return;
        if (heroSelection == null || heroSelection.selectedHero == null)
        {
            Debug.LogError("No hero selected!"); return;
        }

        SpawnPoint sp = startRoom.GetComponentInChildren<SpawnPoint>();
        Vector3 spawnPos = sp != null ? sp.transform.position : startRoom.transform.position;

        GameObject player = Instantiate(heroSelection.selectedHero.prefab, spawnPos, Quaternion.identity);

        PlayerInitializer init = player.GetComponent<PlayerInitializer>();
        if (init != null) init.heroDefinition = heroSelection.selectedHero;

        PlayerTransition.Instance?.SetPlayer(player);
    }
}