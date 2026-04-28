using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectile", menuName = "Combat/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("Movement")]
    public float speed = 12f;
    public float lifetime = 3f;         // destroy after this many seconds if no hit

    [Header("Hit")]
    public float hitRadius = 0.3f;      // sphere overlap on arrival/per frame
    public bool piercing = false;       // pass through multiple enemies (e.g. arrow upgrades)

    [Header("Visuals")]
    public GameObject projectilePrefab; // the travelling object (arrow mesh, fireball VFX)
    public GameObject impactVFXPrefab;  // explosion/puff spawned on hit
    public float impactVFXLifetime = 0.5f;
}