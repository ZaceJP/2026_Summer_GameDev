using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isLocked = true;

    public void LockDoor()
    {
        isLocked = true;
        gameObject.SetActive(false); // hide door (or block it)
    }

    public void UnlockDoor()
    {
        isLocked = false;
        gameObject.SetActive(true); // show/open door
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLocked) return;

        if (other.CompareTag("Player"))
        {
            RoomManager.Instance.LoadNextRoom();
        }
    }
}