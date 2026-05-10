using UnityEngine;

public class DoorBlocker : MonoBehaviour
{
    private Collider blockerCollider;
    private Renderer[] renderers;

    private void Start()
    {
        blockerCollider = GetComponent<Collider>();

        renderers =
            GetComponentsInChildren<Renderer>(true);

        UnlockDoor();
    }

    public void LockDoor()
    {
        if (blockerCollider != null)
            blockerCollider.enabled = true;

        foreach (Renderer r in renderers)
            r.enabled = true;

        Debug.Log("LOCKED DOOR: " + gameObject.name);
    }

    public void UnlockDoor()
    {
        if (blockerCollider != null)
            blockerCollider.enabled = false;

        foreach (Renderer r in renderers)
            r.enabled = false;

        Debug.Log("UNLOCKED DOOR: " + gameObject.name);
    }
}