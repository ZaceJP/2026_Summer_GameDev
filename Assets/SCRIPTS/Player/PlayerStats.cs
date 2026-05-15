using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats — set at runtime from HeroDefinition")]
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;
    public float baseDamage;
    public float attackSpeed;
    public float attackRange;

    [Header("Multipliers — modified by upgrades")]
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;
    public float rangeMultiplier = 1f;

    [Header("Income")]
    public int gold;

    public PlayerModifiers modifiers;

    // ── Computed getters ──────────────────────────────────────────
    public float GetDamage() => baseDamage * damageMultiplier;
    public float GetMoveSpeed() => moveSpeed * speedMultiplier;
    public float GetAttackSpeed() => attackSpeed * attackSpeedMultiplier;
    public float GetAttackRange() => attackRange * rangeMultiplier;


    private void Awake()
    {
        modifiers = GetComponentInChildren<PlayerModifiers>();
        Debug.Log("Modifiers found: " + modifiers);
    }
    // ── Health API ────────────────────────────────────────────────
    public void Init(int startingHealth)
    {
        maxHealth = startingHealth;
        currentHealth = startingHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player HP: {currentHealth} / {maxHealth}");
        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    void Die()
    {
        Debug.Log("Player died!");
        // TODO: game over
    }

    // ── Upgrade API ───────────────────────────────────────────────
    public void ApplyDamageBonus(float percent)
    {
        damageMultiplier += percent;
    }

    public void ApplySpeedBonus(float percent)
    {
        speedMultiplier += percent;
    }

    public void ApplyAttackSpeedBonus(float percent)
    {
        attackSpeedMultiplier += percent;
    }

    public void ApplyRangeBonus(float percent)
    {
        rangeMultiplier += percent;
    }
}