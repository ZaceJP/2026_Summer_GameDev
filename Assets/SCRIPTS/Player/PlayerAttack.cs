using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    // ATTACK DIRECTION
    private Vector2 moveInput;
    private Vector3 aimDirection;

    // ATTACK VISUAL EFFECT
    public GameObject slashEffectPrefab;
    public float slashLifetime = 0.3f;

    // This serves as a fallback or base; we'll prioritize stats if they exist
    public float attackOffset = 1.2f;

    private float lastAttackTime;
    private PlayerStats stats; // Reference to our stats script

    private void Start()
    {
        stats = GetComponent<PlayerStats>();
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
        {
            aimDirection = mouseDir.normalized;
        }
        else
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
            if (moveDir.sqrMagnitude > 0.01f)
            {
                aimDirection = moveDir.normalized;
            }
        }

        if (aimDirection != Vector3.zero)
        {
            transform.forward = aimDirection;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnAttack()
    {
        // DAMAGE FORMULA INTEGRATION: COOLDOWN
        // Uses 1 / attackSpeed (e.g., 2 speed = 0.5s cooldown)
        float cooldown = stats != null ? (1f / stats.attackSpeed) : 0.5f;

        if (Time.time < lastAttackTime + cooldown) return;

        lastAttackTime = Time.time;
        Attack();
    }

    void Attack()
    {
        Vector3 attackOrigin = transform.position + aimDirection * attackOffset;

        // Visuals
        if (slashEffectPrefab != null)
        {
            GameObject slash = Instantiate(slashEffectPrefab, attackOrigin, Quaternion.LookRotation(aimDirection));
            slash.transform.localScale = Vector3.one * 1.5f;
            Destroy(slash, slashLifetime);
        }

        // DAMAGE FORMULA INTEGRATION: RANGE & DAMAGE
        // Pulling range from stats if it exists
        float currentRange = stats != null ? stats.attackRange : 2f;
        Collider[] hits = Physics.OverlapSphere(attackOrigin, currentRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null && stats != null)
                {
                    // Pulling the calculated damage from PlayerStats
                    enemy.TakeDamage((int)stats.GetDamage());
                }
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.point;
        }
        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        // Visualizing the range in the editor
        float gizmoRange = stats != null ? stats.attackRange : 2f;
        Gizmos.DrawWireSphere(transform.position + aimDirection * attackOffset, gizmoRange);
    }
}