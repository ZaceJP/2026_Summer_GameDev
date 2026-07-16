using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public HeroDefinition heroDef; // Set by PlayerInitializer

    [SerializeField] private LayerMask groundMask;

    private Vector2 moveInput;
    private Vector3 aimDirection;
    private Vector3 aimTarget;
    private bool usingControllerAim;
    private Vector2 lookInput;
    private Dictionary<AttackData, float> attackCooldowns = new Dictionary<AttackData, float>();
    // private float lastAttackTime;

    private PlayerStats stats;
    private AudioSource audioSource;
    PlayerAnimationController anim;

    private void Start()
    {
        anim = GetComponent<PlayerAnimationController>();
        stats = GetComponent<PlayerStats>();
        aimDirection = Vector3.forward;

        // Ensure we have an AudioSource component ready
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private bool IsAttackReady(AttackData attack)
    {
        if (attack == null)
            return false;

        if (!attackCooldowns.TryGetValue(attack, out float lastUseTime))
            return true;

        return Time.time >= lastUseTime + attack.cooldown;
    }

    private void PutAttackOnCooldown(AttackData attack)
    {
        attackCooldowns[attack] = Time.time;
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

        anim.PlayPrimaryAttack();

        PerformAttack(heroDef.primaryAttack, heroDef.primaryAttackSFX);
    }

    // Called by Right Click (Make sure this action exists in your Input Actions asset)
    public void OnSecondaryAttack()
    {
        if (heroDef == null || heroDef.secondaryAttack == null) return;
        anim.PlaySecondaryAttack();
        PerformAttack(heroDef.secondaryAttack, heroDef.secondaryAttackSFX);
    }

    // Called by Q Key / Controller North (Triangle/Y)
    public void OnSpecial1()
    {
        Debug.Log("Q pressed");

        if (heroDef == null || heroDef.specialSkill1 == null) return;
        anim.PlaySkill1();
        PerformAttack(heroDef.specialSkill1, heroDef.specialSkill1SFX);
    }

    // Called by E Key / Controller East (Circle/B)
    public void OnSpecial2()
    {
        Debug.Log("E pressed");

        if (heroDef == null || heroDef.specialSkill2 == null) return;
        anim.PlaySkill2();
        PerformAttack(heroDef.specialSkill2, heroDef.specialSkill2SFX);
    }
    // ########## COMBAT LOGIC ##########

    private void PerformAttack(AttackData data, AudioClip sfxClip)
    {
        Debug.Log("Skill Effect: " + data.skillEffect);
        if (!IsAttackReady(data))
            return;

        PutAttackOnCooldown(data);

        if (audioSource != null && sfxClip != null)
            audioSource.PlayOneShot(sfxClip);

        Debug.Log(data.skillEffect);

        switch (data.skillEffect)
        {
            case SkillEffect.Heal:
                PerformHeal(data);
                return;

            case SkillEffect.ShadowExplosion:
                PerformShadowExplosion(data);
                return;

            case SkillEffect.ShadowBarrier:
                PerformBarrier(data);
                return;

            case SkillEffect.FrostNova:
                PerformFrostNova(data);
                return;
            case SkillEffect.MeteorRain:
                StartCoroutine(PerformMeteorRain(data));
                return;
        }

        if (data.attackType == AttackType.Melee)
            AttackMelee(data);
        else
            AttackProjectile(data);
    }

    void AttackMelee(AttackData data)
    {

        float range =
            stats != null
            ? stats.GetAttackRange()
            : data.attackRange;

        float areaSize =
            stats != null
            ? stats.modifiers.areaSizeMultiplier
            : 1f;

        // ==========================
        // VFX
        // ==========================

        if (data.skillVFXPrefab != null)
        {
            Vector3 visualPos;

            if (data.attackShape == AttackShape.Circle)
            {
                visualPos = transform.position;
            }
            else
            {
                visualPos =
                    transform.position +
                    aimDirection * data.meleeOffset;
            }

            GameObject slash =
                Instantiate(
                    data.skillVFXPrefab,
                    visualPos,
                    Quaternion.LookRotation(aimDirection)
                );

            Destroy(slash, data.skillVFXLifetime);
        }

        // ==========================
        // HIT DETECTION
        // ==========================

        Collider[] hits;

        if (data.attackShape == AttackShape.Circle)
        {
            hits = Physics.OverlapSphere(
                transform.position,
                range
            );

            Debug.Log("Sphere Hits: " + hits.Length);
        }
        else
        {
            Vector3 boxHalfExtents =
                new Vector3(
                    areaSize * 0.75f,
                    1f,
                    range * 0.5f
                );

            Vector3 boxCenter =
                transform.position +
                aimDirection * (range * 0.5f);

            hits = Physics.OverlapBox(
                boxCenter,
                boxHalfExtents,
                Quaternion.LookRotation(aimDirection)
            );

            Debug.Log("Box Hits: " + hits.Length);
        }

        // ==========================
        // DAMAGE + KNOCKBACK
        // ==========================

        foreach (Collider hit in hits)
        {
            Debug.Log("Collider Hit: " + hit.name);
            if (!hit.CompareTag("Enemy"))
                continue;

            EnemyController enemy =
                hit.GetComponentInParent<EnemyController>();
            Debug.Log("Enemy Found: " + enemy);

            if (enemy == null)
                continue;

            int damage =
                stats != null
                ? (int)stats.GetDamage()
                : data.damage;

            enemy.TakeDamage(damage);

            if (data.applyKnockback)
            {
                Vector3 knockDir =
                    enemy.transform.position - transform.position;

                knockDir.y = 0f;
                knockDir.Normalize();

                enemy.ApplyKnockback(
                    knockDir,
                    data.knockbackForce
                );
            }
        }
    }

    void AttackProjectile(AttackData data)
    {
        if (data.projectileData?.projectilePrefab == null) return;

         int bonus =
             stats.modifiers != null
             ? stats.modifiers.bonusProjectiles
                 : 0;

        int count = data.projectileCount + bonus;

        Debug.Log("Projectile Count: " + count);

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

    void PerformHeal(AttackData data)
    {
        // Heal
        stats.Heal(data.healAmount);

        // Spawn VFX
        if (data.skillVFXPrefab != null)
        {
            GameObject vfx = Instantiate(
                data.skillVFXPrefab,
                transform.position,
                Quaternion.identity);

            Destroy(vfx, data.skillVFXLifetime);
        }
    }

    void PerformShadowExplosion(AttackData data)
    {
        // VFX
        if (data.skillVFXPrefab != null)
        {
            GameObject vfx = Instantiate(
                data.skillVFXPrefab,
                transform.position,
                Quaternion.identity);

            Destroy(vfx, data.skillVFXLifetime);
        }

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            data.effectRadius);

        Debug.Log("Shadow Explosion hits: " + hits.Length);

        Debug.DrawLine(transform.position,
               transform.position + Vector3.up * data.effectRadius,
               Color.magenta,
               2f);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            EnemyController enemy =
                hit.GetComponentInParent<EnemyController>();

            if (enemy == null)
                continue;

            enemy.TakeDamage(data.damage);

            if (data.applyKnockback)
            {
                Vector3 dir =
                    enemy.transform.position - transform.position;

                dir.y = 0;
                dir.Normalize();

                Debug.Log("Knockback!");
                enemy.ApplyKnockback(
                    dir,
                    data.knockbackForce);
            }
        }
    }

    void PerformFrostNova(AttackData data)
    {
        // Spawn VFX
        if (data.skillVFXPrefab != null)
        {
            GameObject vfx = Instantiate(
                data.skillVFXPrefab,
                transform.position,
                Quaternion.identity);

            Destroy(vfx, data.skillVFXLifetime);
        }

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            data.effectRadius);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            EnemyController enemy =
                hit.GetComponent<EnemyController>();

            if (enemy == null)
                continue;

            enemy.TakeDamage(data.damage);

            enemy.Freeze(data.freezeDuration,data.statusEffectVFX);

            if (data.applyKnockback)
            {
                Vector3 dir =
                    enemy.transform.position -
                    transform.position;

                dir.y = 0;
                dir.Normalize();

                enemy.ApplyKnockback(
                    dir,
                    data.knockbackForce);
            }
        }
    }

    IEnumerator PerformMeteorRain(AttackData data)
    {
        Vector3 target = GetSkillTargetPosition(data);

        // 1. Spawn targeting indicator
        GameObject indicator = null;

        if (data.targetIndicatorPrefab != null)
        {
            indicator = Instantiate(
                data.targetIndicatorPrefab,
                target,
                Quaternion.identity);

            Destroy(indicator, data.castDelay);
        }

        // 2. Wait before impact
        yield return new WaitForSeconds(data.castDelay);

        for (int i = 0; i < data.meteorCount; i++)
        {
            Vector2 offset =
                Random.insideUnitCircle * data.meteorSpread;

            Vector3 impactPos =
                target +
                new Vector3(offset.x, 0, offset.y);

            SpawnMeteor(data, impactPos);

            yield return new WaitForSeconds(data.meteorSpawnInterval);
        }


    }

    void SpawnMeteor(AttackData data, Vector3 impactPos)
    {
        // Find the ground beneath the impact position
        Vector3 spawnPos = impactPos;

        RaycastHit groundHit;

        if (Physics.Raycast(
      impactPos + Vector3.up * 10f,
      Vector3.down,
      out groundHit,
      20f,
      groundMask))
        {
            spawnPos = groundHit.point;
        }
        else
        {
            Debug.Log("Raycast hit nothing!");
        }

        // Spawn meteor VFX
        if (data.skillVFXPrefab != null)
        {
            GameObject meteor =
                Instantiate(
                    data.skillVFXPrefab,
                    spawnPos,
                    Quaternion.identity);

            Destroy(meteor, data.skillVFXLifetime);
        }

        // Damage enemies
        Collider[] hits =
            Physics.OverlapSphere(
                impactPos,
                data.effectRadius);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            EnemyController enemy =
                hit.GetComponentInParent<EnemyController>();

            if (enemy == null)
                continue;

            enemy.TakeDamage(data.damage);

            if (data.applyKnockback)
            {
                Vector3 dir =
                    enemy.transform.position - impactPos;

                dir.y = 0;
                dir.Normalize();

                enemy.ApplyKnockback(
                    dir,
                    data.knockbackForce);
            }

        }
    }

    void PerformBarrier(AttackData data)
    {
        Debug.Log("Barrier Activated");

        stats.AddShield(data.shieldAmount);

        if (data.skillVFXPrefab != null)
        {
            GameObject vfx = Instantiate(
                data.skillVFXPrefab,
                transform.position,
                Quaternion.identity,
                transform   // <-- parent it to player
            );

            Destroy(vfx, data.skillVFXLifetime);
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

    Vector3 GetSkillTargetPosition(AttackData data)
    {
        switch (data.targetType)
        {
            case TargetType.Self:
                return transform.position;

            case TargetType.Direction:

                if (usingControllerAim)
                {
                    return transform.position +
                           aimDirection * data.targetDistance;
                }

                return aimTarget;

            default:
                return transform.position;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            return hit.point;

        return transform.position;
    }
    void HandleAiming()
    {
        if (lookInput.sqrMagnitude > 0.1f)
        {
            usingControllerAim = true;

            aimDirection = new Vector3(
                lookInput.x,
                0,
                lookInput.y).normalized;

            aimTarget =
                transform.position +
                aimDirection * 8f;
        }
        else
        {
            usingControllerAim = false;

            Vector3 mousePos = GetMouseWorldPosition();
            Vector3 mouseDir = mousePos - transform.position;
            mouseDir.y = 0;

            if (mouseDir.sqrMagnitude > 0.01f)
            {
                aimDirection = mouseDir.normalized;
                aimTarget = mousePos;
            }
        }

        if (aimDirection != Vector3.zero)
            transform.forward = aimDirection;
    }

    public float GetCooldownRemaining(AttackData attack)
    {
        if (attack == null)
            return 0f;

        if (!attackCooldowns.TryGetValue(attack, out float lastUse))
            return 0f;

        float remaining =
            attack.cooldown -
            (Time.time - lastUse);

        return Mathf.Max(0f, remaining);
    }

    
    public float GetCooldownPercent(AttackData attack)
    {
        if (attack == null)
            return 0f;

        if (attack.cooldown <= 0)
            return 0f;

        return GetCooldownRemaining(attack) / attack.cooldown;
    }

    private void OnDrawGizmosSelected()
    {
        if (heroDef == null || heroDef.primaryAttack == null)
            return;

        AttackData data = heroDef.primaryAttack;

        // Only show melee gizmo
        if (data.attackType != AttackType.Melee)
            return;

        // Fallback values when not in play mode
        float range = data.attackRange;
        float width = 1f;

        if (stats != null && stats.modifiers != null)
        {
            range = stats.GetAttackRange();

            width =
                stats.modifiers.areaSizeMultiplier;
        }

        Vector3 origin =
            transform.position +
            aimDirection * data.meleeOffset;

        Vector3 halfExtents =
    new Vector3(
        width * 0.75f,
        1f,
        range * 0.5f
    );

        Vector3 center =
            transform.position +
            aimDirection * (range * 0.5f);

        Gizmos.color = Color.red;

        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix =
            Matrix4x4.TRS(
                center,
                Quaternion.LookRotation(aimDirection),
                Vector3.one
            );

        Gizmos.DrawWireCube(
            Vector3.zero,
            halfExtents * 2f
        );

        Gizmos.matrix = oldMatrix;
    }
}