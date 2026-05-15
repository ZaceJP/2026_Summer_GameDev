using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public HeroDefinition heroDef; // Set by PlayerInitializer

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

    // ########## INPUT HANDLING ##########

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    // Called by Left Click
    public void OnAttack()
    {
        if (heroDef == null || heroDef.primaryAttack == null) return;
        PerformAttack(heroDef.primaryAttack);
    }

    // Called by Right Click (Make sure this action exists in your Input Actions asset)
    public void OnSecondaryAttack()
    {
        if (heroDef == null || heroDef.secondaryAttack == null) return;
        PerformAttack(heroDef.secondaryAttack);
    }

    // ########## COMBAT LOGIC ##########

    private void PerformAttack(AttackData data)
    {
        // Use PlayerStats attack speed if available, otherwise use default SO cooldown
        float cooldown = stats != null ? 1f / stats.GetAttackSpeed() : data.cooldown;

        if (Time.time < lastAttackTime + cooldown) return;
        lastAttackTime = Time.time;

        if (data.attackType == AttackType.Melee)
            AttackMelee(data);
        else
            AttackProjectile(data);
    }

    void AttackMelee(AttackData data)
    {
        Vector3 origin = transform.position + aimDirection * data.meleeOffset;

        if (data.slashVFXPrefab != null)
        {
            GameObject slash = Instantiate(data.slashVFXPrefab, origin, Quaternion.LookRotation(aimDirection));
            slash.transform.localScale = Vector3.one * 1.5f;
            Destroy(slash, data.slashVFXLifetime);
        }

        // Use PlayerStats range if available, otherwise use default SO range
        float range = stats != null ? stats.GetAttackRange() : data.attackRange;

        Collider[] hits = Physics.OverlapSphere(origin, range);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    int damage = stats != null ? (int)stats.GetDamage() : data.damage;
                    enemy.TakeDamage(damage);
                }
            }
        }
    }

    void AttackProjectile(AttackData data)
    {
        if (data.projectileData?.projectilePrefab == null) return;

        int count = data.projectileCount;
        int damage = stats != null ? (int)stats.GetDamage() : data.damage;

        float totalSpread = data.spreadAngle * (count - 1);
        float startAngle = -totalSpread / 2f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + data.spreadAngle * i;
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * aimDirection;
            SpawnProjectile(data, dir, "Enemy", damage);
        }
    }

    void SpawnProjectile(AttackData data, Vector3 dir, string targetTag, int damage)
    {
        Vector3 spawnPos = transform.position + dir * 0.6f + Vector3.up * 0.5f;
        GameObject obj = Instantiate(data.projectileData.projectilePrefab, spawnPos, Quaternion.LookRotation(dir));

        Projectile p = obj.GetComponent<Projectile>();
        if (p == null) p = obj.AddComponent<Projectile>();

        p.Init(data.projectileData, dir, targetTag, damage);
    }

    // ########## UTILS ##########

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

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            return hit.point;
        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        // Preview the primary attack range in the editor
        if (heroDef == null || heroDef.primaryAttack == null) return;
        Gizmos.color = Color.blue;
        float range = stats != null ? stats.GetAttackRange() : heroDef.primaryAttack.attackRange;
        Gizmos.DrawWireSphere(transform.position + aimDirection * heroDef.primaryAttack.meleeOffset, range);
    }
}