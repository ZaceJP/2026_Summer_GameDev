using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;
    public Transform player;

    private float lastAttackTime;
    private int currentHealth;
    private CharacterController controller;
    private DungeonRoom room;

    private void Start()
    {
        room = GetComponentInParent<DungeonRoom>();
        controller = GetComponent<CharacterController>();
        currentHealth = data.maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else return;
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

        if (direction != Vector3.zero)
            transform.forward = direction;
    }

    void TryAttack()
    {
        if (data.attackData == null) return;

        if (Time.time < lastAttackTime + data.attackData.cooldown) return;
        lastAttackTime = Time.time;

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
        currentHealth -= amount;
        Debug.Log($"Enemy HP: {currentHealth} / {data.maxHealth}");
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        room?.OnEnemyDied();
        Destroy(gameObject);
    }
}