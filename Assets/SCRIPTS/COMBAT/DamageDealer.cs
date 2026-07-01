using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;

    [Tooltip("How long before the same target can be damaged again.")]
    public float hitCooldown = 2f;

    [Header("Targets")]
    public LayerMask targetLayers;

    private Dictionary<IDamageable, float> lastHitTimes = new();

    public void DamageTarget(Collider other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        IDamageable target = other.GetComponentInParent<IDamageable>();

        if (target == null)
            return;

        if (lastHitTimes.TryGetValue(target, out float lastHit))
        {
            if (Time.time < lastHit + hitCooldown)
                return;
        }

        lastHitTimes[target] = Time.time;

        target.TakeDamage(damage);
    }
}