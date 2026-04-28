using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    // Drag the correct AttackData SO here per character class
    public AttackData attackData;

    private Vector2 moveInput;
    private Vector3 aimDirection;

    private float lastAttackTime;
    private PlayerStats stats;

    private void Start()
    {
        stats = GetComponent<PlayerStats>();
        aimDirection = Vector3.forward;
    }

    void Update()
    {
        HandleAiming();
    }

    void HandleAiming()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        Vector3 mouseDir = mousePos - transform.position;
        mouseDir.y = 0;

        if (mouseDir.sqrMagnitude > 0.01f)
            aimDirection = mouseDir.normalized;
        else
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
            if (moveDir.sqrMagnitude > 0.01f)
                aimDirection = moveDir.normalized;
        }

        if (aimDirection != Vector3.zero)
            transform.forward = aimDirection;
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    public void OnAttack()
    {
        if (attackData == null) return;

        // Before: 1f / stats.attackSpeed
        float cooldown = stats != null ? 1f / stats.GetAttackSpeed() : attackData.cooldown;

        if (Time.time < lastAttackTime + cooldown) return;
        lastAttackTime = Time.time;

        if (attackData.attackType == AttackType.Melee)
            AttackMelee();
        else
            AttackProjectile();
    }

    void AttackMelee()
    {
        Vector3 origin = transform.position + aimDirection * attackData.meleeOffset;

        if (attackData.slashVFXPrefab != null)
        {
            GameObject slash = Instantiate(attackData.slashVFXPrefab, origin,
                                           Quaternion.LookRotation(aimDirection));
            slash.transform.localScale = Vector3.one * 1.5f;
            Destroy(slash, attackData.slashVFXLifetime);
        }

        // Before: stats.attackRange
        float range = stats != null ? stats.GetAttackRange() : attackData.attackRange;
        Collider[] hits = Physics.OverlapSphere(origin, range);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                    enemy.TakeDamage(stats != null ? (int)stats.GetDamage() : 10);
            }
        }
    }

    void AttackProjectile()
    {
        if (attackData.projectileData?.projectilePrefab == null) return;

        int count = attackData.projectileCount;
        int damage = stats != null ? (int)stats.GetDamage() : 10;

        // Calculate spread directions
        float totalSpread = attackData.spreadAngle * (count - 1);
        float startAngle = -totalSpread / 2f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + attackData.spreadAngle * i;
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * aimDirection;

            SpawnProjectile(dir, "Enemy", damage);
        }
    }

    void SpawnProjectile(Vector3 dir, string targetTag, int damage)
    {
        Vector3 spawnPos = transform.position + dir * 0.6f + Vector3.up * 0.5f;
        GameObject obj = Instantiate(attackData.projectileData.projectilePrefab,
                                     spawnPos, Quaternion.LookRotation(dir));

        Projectile p = obj.AddComponent<Projectile>();
        p.Init(attackData.projectileData, dir, targetTag, damage);
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            return hit.point;
        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackData == null) return;
        Gizmos.color = Color.blue;
        // Before: stats.attackRange
        float range = stats != null ? stats.GetAttackRange() : attackData.attackRange;
        Gizmos.DrawWireSphere(transform.position + aimDirection * attackData.meleeOffset, range);
    }
}