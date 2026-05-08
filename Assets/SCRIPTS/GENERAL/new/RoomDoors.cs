using UnityEngine;
using System.Collections.Generic;

public class RoomDoors : MonoBehaviour
{
    public Transform northDoor;
    public Transform southDoor;
    public Transform eastDoor;
    public Transform westDoor;

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
}

public enum DoorDirection
{
    North,
    South,
    East,
    West
}