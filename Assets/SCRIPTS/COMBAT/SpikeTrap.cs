using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private DamageDealer damageDealer;

    private bool damageActive;

    private readonly List<Collider> collidersInside = new();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRAP ENTER: " + other.name);


        if (!collidersInside.Contains(other))
            collidersInside.Add(other);

        if (damageActive)
            damageDealer.DamageTarget(other);
    }

    private void OnTriggerExit(Collider other)
    {
        collidersInside.Remove(other);
    }

    // Animation Event
    public void EnableDamage()
    {
        damageActive = true;

        foreach (Collider col in collidersInside)
        {
            if (col != null)
                damageDealer.DamageTarget(col);
        }
    }

    // Animation Event
    public void DisableDamage()
    {
        damageActive = false;
    }
}