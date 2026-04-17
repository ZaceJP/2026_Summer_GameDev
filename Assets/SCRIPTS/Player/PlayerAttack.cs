using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public int damage = 2;
    public float attackCooldown = 0.5f;

    private float lastAttackTime;
    PlayerStats stats;

    private void Start()
    {
        stats = GetComponent<PlayerStats>();
    }
    public void OnAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        Attack();
    }

    void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage((int)stats.GetDamage());
                }
            }
        }

        Debug.Log("Player attacked!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}