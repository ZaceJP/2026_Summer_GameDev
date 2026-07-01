using UnityEngine;

public class FireTrapShooter : MonoBehaviour
{
    [Header("Projectile")]
    public FireballProjectile projectilePrefab;

    public Transform firePoint;

    [Header("Settings")]
    public float shootInterval = 3f;

    public float projectileSpeed = 8f;

    public int damage = 10;

    public float hitCooldown = 0.5f;

    public LayerMask targetLayers;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= shootInterval)
        {
            timer = 0f;
            Shoot();
        }
    }

    void Shoot()
    {
        FireballProjectile projectile =
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        projectile.Initialize(
            firePoint.forward,
            projectileSpeed,
            damage,
            hitCooldown,
            targetLayers
        );
    }
}