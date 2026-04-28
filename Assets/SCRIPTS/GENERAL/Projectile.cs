using UnityEngine;

public class Projectile : MonoBehaviour
{
    private ProjectileData data;
    private Vector3 direction;
    private string targetTag;       // "Enemy" or "Player"
    private int damage;
    private float spawnTime;

    public void Init(ProjectileData projectileData, Vector3 dir, string target, int dmg)
    {
        data = projectileData;
        direction = dir.normalized;
        targetTag = target;
        damage = dmg;
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (data == null) return;

        // Destroy if lifetime exceeded
        if (Time.time - spawnTime >= data.lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Move forward
        transform.position += direction * data.speed * Time.deltaTime;
        transform.forward = direction;

        // Hit detection each frame via overlap sphere
        Collider[] hits = Physics.OverlapSphere(transform.position, data.hitRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(targetTag))
            {
                OnHit(hit);
                if (!data.piercing)
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    void OnHit(Collider hit)
    {
        // Spawn impact VFX
        if (data.impactVFXPrefab != null)
        {
            GameObject vfx = Instantiate(data.impactVFXPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, data.impactVFXLifetime);
        }

        // Deal damage depending on who we're hitting
        if (targetTag == "Enemy")
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            enemy?.TakeDamage(damage);
        }
        else if (targetTag == "Player")
        {
            PlayerStats stats = hit.GetComponent<PlayerStats>();
            stats?.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.hitRadius);
    }
}