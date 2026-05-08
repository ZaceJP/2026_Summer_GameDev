using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Direction direction; // which wall this spawn point belongs to

    void OnDrawGizmos()
    {
        Gizmos.color = direction switch
        {
            Direction.North => Color.blue,
            Direction.South => Color.green,
            Direction.East => Color.red,
            Direction.West => Color.yellow,
            _ => Color.white
        };
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}