using UnityEngine;
using System.Collections;
public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("References")]
    [HideInInspector] public HeroDefinition heroDef;


    [Header("Base Stats — set at runtime from HeroDefinition")]
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;
    public float baseDamage;
    public float attackSpeed;
    public float attackRange;

    [Header("Shield")]
    public int shieldAmount;
    public bool HasShield => shieldAmount > 0;

    [Header("Multipliers — modified by upgrades")]
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;
    public float rangeMultiplier = 1f;

    [Header("Income")]
    public int gold;

    public PlayerModifiers modifiers;
    private AudioSource audioSource;

    // ── Computed getters ──────────────────────────────────────────
    public float GetDamage() => baseDamage * damageMultiplier;
    public float GetMoveSpeed() => moveSpeed * speedMultiplier;
    public float GetAttackSpeed() => attackSpeed * attackSpeedMultiplier;
    public float GetAttackRange() => attackRange * rangeMultiplier;


    private void Awake()
    {
        modifiers = GetComponentInChildren<PlayerModifiers>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    // ── Health API ────────────────────────────────────────────────
    public void Init(int startingHealth)
    {
        maxHealth = startingHealth;
        currentHealth = startingHealth;
    }

    public void TakeDamage(int amount)
    {
        if (shieldAmount > 0)
        {
            int absorbed = Mathf.Min(amount, shieldAmount);

            shieldAmount -= absorbed;
            amount -= absorbed;

            // Optional: play shield hit VFX/SFX

            if (amount <= 0)
                return;
        }

        if (DamageNumberManager.Instance != null)
        {
            DamageNumberManager.Instance.ShowDamage(
            amount,
            transform.position + Vector3.up * 2f,
            false,
            true
               );
        }

        currentHealth -= amount;
        Debug.Log($"Player HP: {currentHealth} / {maxHealth}");

        // Play hit feedback sound
        if (currentHealth > 0 && heroDef != null && heroDef.getHitSFX != null)
        {
            audioSource.PlayOneShot(heroDef.getHitSFX);
        }

        if (currentHealth <= 0)
            Die();
    }

    public void AddShield(int amount)
    {
        shieldAmount += amount;

        Debug.Log("Shield: " + shieldAmount);
    }
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    void Die()
    {
        Debug.Log("Player died!");

        PlayerAnimationController anim =
        GetComponent<PlayerAnimationController>();

        if (anim != null)
            anim.PlayDeath();

        // Use static world position sound emitter so death audio survives object destruction
        if (heroDef != null && heroDef.dieSFX != null)
        {
            AudioSource.PlayClipAtPoint(heroDef.dieSFX, transform.position);
        }

        // GAME OVER SCREEN CALL
        if (GameEndManager.Instance != null)
        {
            GameEndManager.Instance.TriggerEndScreen(GameEndState.GameOver);
        }

        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(2.5f);

        gameObject.SetActive(false);
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