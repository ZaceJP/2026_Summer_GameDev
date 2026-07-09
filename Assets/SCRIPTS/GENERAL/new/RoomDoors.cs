using UnityEngine;
using System.Collections.Generic;

public class RoomDoors : MonoBehaviour
{
    [Header("Door")]
    public Transform northDoor;
    public Transform southDoor;
    public Transform eastDoor;
    public Transform westDoor;

    [Header("Closed Walls")]
    public GameObject northWall;
    public GameObject southWall;
    public GameObject eastWall;
    public GameObject westWall;

    public Transform GetDoor(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.North => northDoor,
            DoorDirection.South => southDoor,
            DoorDirection.East => eastDoor,
            DoorDirection.West => westDoor,
            _ => null
        };
    }

    public bool HasDoor(DoorDirection dir)
    {
        return GetDoor(dir) != null;
    }

    public List<DoorDirection> GetAvailableDoors()
    {
        List<DoorDirection> doors = new List<DoorDirection>();

        if (northDoor != null)
            doors.Add(DoorDirection.North);

        if (southDoor != null)
            doors.Add(DoorDirection.South);

        if (eastDoor != null)
            doors.Add(DoorDirection.East);

        if (westDoor != null)
            doors.Add(DoorDirection.West);

        return doors;
    }

    GameObject GetWall(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.North => northWall,
            DoorDirection.South => southWall,
            DoorDirection.East => eastWall,
            DoorDirection.West => westWall,
            _ => null
        };
    }

    public void SetConnection(DoorDirection dir, bool connected)
    {
        Transform door = GetDoor(dir);
        GameObject wall = GetWall(dir);

        if (door != null)
            door.gameObject.SetActive(connected);

        if (wall != null)
            wall.SetActive(!connected);
    }
}


public enum DoorDirection
{
    North,
    South,
    East,
    West
}