using UnityEngine;

public class Door : MonoBehaviour
{
    public Direction direction;
    public Transform hallwayAnchor;
    private bool isLocked = true;

    public void LockDoor() => isLocked = true;
    public void UnlockDoor() => isLocked = false;
    public bool IsLocked() => isLocked;

    public Vector3 GetAnchorPosition()
    {
        return hallwayAnchor != null ? hallwayAnchor.position : transform.position;
    }
}