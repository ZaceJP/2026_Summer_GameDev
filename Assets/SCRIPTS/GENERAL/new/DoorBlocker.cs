using UnityEngine;

public class DoorBlocker : MonoBehaviour
{
    private Collider blockerCollider;
    private Renderer[] renderers;

  

    public void LockDoor()
    {
        gameObject.SetActive(true);
        Debug.Log("LOCKED DOOR: " + gameObject.name);
    }

    public void UnlockDoor()
    {
        gameObject.SetActive(false);
        Debug.Log("UNLOCKED DOOR: " + gameObject.name);
    }
}