using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireballProjectile : MonoBehaviour
{
    private Rigidbody rb;

    private DamageDealer damageDealer;

    public float lifetime = 6f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        damageDealer = GetComponent<DamageDealer>();

        if (damageDealer == null)
            damageDealer = gameObject.AddComponent<DamageDealer>();
    }

    public void Initialize(
        Vector3 direction,
        float speed,
        int damage,
        float cooldown,
        LayerMask targets)
    {
        damageDealer.damage = damage;
        damageDealer.hitCooldown = cooldown;
        damageDealer.targetLayers = targets;

        rb.linearVelocity = direction.normalized * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        damageDealer.DamageTarget(other);

        Destroy(gameObject);
    }
}