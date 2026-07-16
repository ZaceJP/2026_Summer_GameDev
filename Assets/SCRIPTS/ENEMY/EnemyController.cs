using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour, IDamageable
{
   [Header("Hit Reaction")]
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.12f;

    private Vector3 knockbackVelocity;
    private float knockbackTimer;

    public EnemyData data;
    public Transform player;

    private float lastAttackTime;
    protected int currentHealth;
    protected CharacterController controller;
    protected RoomEncounter room;

    protected Animator animator;
    protected AudioSource audioSource;

    private bool isFrozen;
    private Coroutine freezeRoutine;
    private Renderer[] renderers;
    private Color[][] originalColors;

    private GameObject activeStatusVFX;  // to atatch like freezing vfx or burning , poisona nd so on

    protected bool isDead;

    protected virtual void Start()
    {
        
        room = GetComponentInParent<RoomEncounter>();
        Debug.Log("Enemy room found: " + room);
        controller = GetComponent<CharacterController>();
        currentHealth = data.maxHealth;

        animator = GetComponentInChildren<Animator>();

        renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].materials;
            originalColors[i] = new Color[mats.Length];

            for (int j = 0; j < mats.Length; j++)
            {
                if (mats[j].HasProperty("_Color"))
                    originalColors[i][j] = mats[j].color;
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    protected virtual void Update()
    {
        if (isDead)
            return;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else return;
        }

        if (knockbackTimer > 0f)
        {
            controller.Move(knockbackVelocity * Time.deltaTime);

            knockbackTimer -= Time.deltaTime;
            return;
        }

        if (isFrozen)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        float attackRange = data.attackData != null ? data.attackData.attackRange : 2f;

        if (distance <= data.viewDistance)
        {
            if (distance > attackRange)
                MoveTowardsPlayer();
            else
                TryAttack();
        }
        else
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0;
        direction = direction.normalized;

        int wallMask = LayerMask.GetMask("Default");
        if (Physics.Raycast(transform.position, direction, 0.6f, wallMask))
            return;

        Vector3 move = direction * data.moveSpeed;
        move.y = -1f;
        controller.Move(move * Time.deltaTime);

        if (animator != null)
        {
            animator.SetFloat("Speed", 1f);
        }

        if (direction != Vector3.zero)
            transform.forward = direction;
    }

    void TryAttack()
    {
        if (data.attackData == null) return;

        if (Time.time < lastAttackTime + data.attackData.cooldown)
            return;

        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void DealMeleeHit()
    {
        float range = data.attackData.attackRange;

        // Spawn slash effect
        if (data.attackData.skillVFXPrefab != null)
        {
            Vector3 vfxPos =
                transform.position +
                transform.forward * data.attackData.meleeOffset;

            GameObject slash =
                Instantiate(
                    data.attackData.skillVFXPrefab,
                    vfxPos,
                    Quaternion.LookRotation(transform.forward)
                );

            Destroy(slash, data.attackData.skillVFXLifetime);
        }

        // Existing hit detection

        Vector3 center =
            transform.position +
            transform.forward * (range * 0.5f);

        Vector3 halfExtents =
            new Vector3(
                0.75f,
                1f,
                range * 0.5f
            );

        Collider[] hits =
            Physics.OverlapBox(
                center,
                halfExtents,
                transform.rotation
            );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerStats player =
                    hit.GetComponent<PlayerStats>();

                if (player != null)
                {
                    player.TakeDamage(data.attackData.damage);
                }
            }
        }
    }

    public void SpawnProjectileFromAnimation()
    {
        if (data.attackData == null) return;

        Vector3 dir = (player.position - transform.position);
        dir.y = 0;
        dir.Normalize();

        Vector3 spawnPos =
            transform.position +
            dir * 0.6f +
            Vector3.up * 0.5f;

        GameObject obj =
            Instantiate(
                data.attackData.projectileData.projectilePrefab,
                spawnPos,
                Quaternion.LookRotation(dir)
            );

        Projectile p = obj.GetComponent<Projectile>();

        if (p == null)
            p = obj.AddComponent<Projectile>();

        p.Init(
            data.attackData.projectileData,
            dir,
            "Player",
            data.attackData.damage
        );
    }

    public virtual void TakeDamage(int amount)
    {
        DamageNumberManager.Instance.ShowDamage(
          amount,
          transform.position + Vector3.up * 2f
        );

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("GetHit");
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        Debug.Log("CALL KNOCKBACK");
        knockbackVelocity = direction.normalized * force;
        knockbackTimer = knockbackDuration;
    }

    public void Freeze(float duration, GameObject statusVFX)
    {
        if (freezeRoutine != null)
            StopCoroutine(freezeRoutine);

        freezeRoutine = StartCoroutine(
            FreezeRoutine(duration, statusVFX)
        );
    }

    IEnumerator FreezeRoutine(float duration, GameObject statusVFX)
    {
        isFrozen = true;

        if (animator != null)
            animator.speed = 0f;

        AttachStatusVFX(statusVFX);
        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_Color"))
                    mat.color = new Color(0.45f, 0.8f, 1f);
            }
        }

        yield return new WaitForSeconds(duration);

        
        if (animator != null)
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] mats = renderers[i].materials;

                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].HasProperty("_Color"))
                        mats[j].color = originalColors[i][j];
                }
            }
        RemoveStatusVFX();
        animator.speed = 1f;

        isFrozen = false;
    }

    // BOSS CHECK
    public int CurrentHealth => currentHealth;
    protected virtual void Die()
    {
        Debug.Log("DIE FUNCTION CALLED");

        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }
        // Instantiates a temporary dummy object that plays the audio, then automatically destroys itself.
        // This ensures the death sound doesn't get clipped when the enemy GameObject is destroyed.
        if (data != null && data.dieSFX != null)
        {
            AudioSource.PlayClipAtPoint(data.dieSFX, transform.position);
        }

        if (room != null)
        {
            Debug.Log("Sending death to room");
            room.OnEnemyDied();
        }
        else
        {
            Debug.Log("ROOM IS NULL");
        }

        if (data.isBoss)
        {
            GameEndManager.Instance.TriggerEndScreen(GameEndState.GameClear);
        }
        isDead = true;
        Destroy(gameObject, 5f);
    }


    //// HELPER FOT STATUS VFX
    ///
    public void AttachStatusVFX(GameObject prefab)
    {
        Debug.Log("AttachStatusVFX");
        if (prefab == null)
        {
            Debug.Log("STATUS VFX PREFAB IS NULL");
            return;
        }

        Debug.Log("Instantiating " + prefab.name);
        RemoveStatusVFX();

        activeStatusVFX = Instantiate(prefab, transform);
        activeStatusVFX.transform.localPosition = Vector3.zero;
    }

    public void RemoveStatusVFX()
    {
        if (activeStatusVFX != null)
        {
            Destroy(activeStatusVFX);
            activeStatusVFX = null;
        }
    }


}