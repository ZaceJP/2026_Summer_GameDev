using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [Header("Hit Reaction")]
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.12f;

    private Vector3 knockbackVelocity;
    private float knockbackTimer;

    public EnemyData data;
    public Transform player;

    private float lastAttackTime;
    private int currentHealth;
    private CharacterController controller;
    private RoomEncounter room;

    private Animator animator;
    private AudioSource audioSource;

    private bool isDead;

    private void Start()
    {
        room = GetComponentInParent<RoomEncounter>();
        Debug.Log("Enemy room found: " + room);
        controller = GetComponent<CharacterController>();
        currentHealth = data.maxHealth;

        animator = GetComponentInChildren<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
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
            transform.position += knockbackVelocity * Time.deltaTime;
            knockbackTimer -= Time.deltaTime;
            return;
        }

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

        if (Time.time < lastAttackTime + data.attackData.cooldown) return;
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Ranged
        if (data.attackData.attackType == AttackType.Projectile
            && data.attackData.projectileData?.projectilePrefab != null)
        {
            Vector3 dir = (player.position - transform.position);
            dir.y = 0;
            dir.Normalize();

            Vector3 spawnPos = transform.position + dir * 0.6f + Vector3.up * 0.5f;
            GameObject obj = Instantiate(data.attackData.projectileData.projectilePrefab,
                                         spawnPos, Quaternion.LookRotation(dir));
            Projectile p = obj.AddComponent<Projectile>();
            p.Init(data.attackData.projectileData, dir, "Player", data.attackData.damage);
            return;
        }

        // Melee
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamage(data.attackData.damage);
            Debug.Log("Enemy hit player for " + data.attackData.damage);
        }
    }

    public void TakeDamage(int amount)
    {
        DamageNumberManager.Instance.ShowDamage(
          amount,
          transform.position + Vector3.up * 2f
        );

        currentHealth -= amount;
        if (animator != null)
        {
            animator.SetTrigger("GetHit");
        }
        
        // Play enemy hurt audio
        if (currentHealth > 0 && data != null && data.getHitSFX != null)
        {
            audioSource.PlayOneShot(data.getHitSFX);
        }

        if (currentHealth <= 0)
            Die();
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction.normalized * force;
        knockbackTimer = knockbackDuration;
    }

    void Die()
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

        isDead = true;
        Destroy(gameObject, 2f);
    }
}