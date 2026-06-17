using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Header("Settings")]
    public int outOfBoundsDamage = 999;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone is the Player
        PlayerStats player = other.GetComponent<PlayerStats>();

        // If it didn't find it on the object directly, check its parent (sometimes colliders are on child objects)
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerStats>();
        }

        // If we found the player, smash them with damage
        if (player != null)
        {
            Debug.Log($"[KillZone] Player fell out of bounds! Inflicting {outOfBoundsDamage} damage.");
            player.TakeDamage(outOfBoundsDamage);
        }
    }
}