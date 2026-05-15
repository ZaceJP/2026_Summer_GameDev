using UnityEngine;

public class UpgradeApplier : MonoBehaviour
{
    private PlayerStats stats;
    private PlayerModifiers modifiers;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        modifiers = GetComponent<PlayerModifiers>();
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        float finalValue = upgrade.value;

        // Convert percentages automatically
        if (upgrade.isPercentage)
            finalValue *= 0.01f;

        switch (upgrade.upgradeType)
        {
            // ─────────────────────────────
            // BASIC STATS
            // ─────────────────────────────

            case UpgradeType.Damage:
                stats.ApplyDamageBonus(finalValue);
                break;

            case UpgradeType.AttackSpeed:
                stats.ApplyAttackSpeedBonus(finalValue);
                break;

            case UpgradeType.MoveSpeed:
                stats.ApplySpeedBonus(finalValue);
                break;

            case UpgradeType.AttackRange:
                stats.ApplyRangeBonus(finalValue);
                break;

            case UpgradeType.MaxHealth:
                stats.maxHealth += Mathf.RoundToInt(upgrade.value);
                stats.currentHealth += Mathf.RoundToInt(upgrade.value);
                break;

            case UpgradeType.Gold:
                stats.gold += Mathf.RoundToInt(upgrade.value);
                break;

            // ─────────────────────────────
            // UTILITY
            // ─────────────────────────────

            case UpgradeType.Heal:

                // percentage heal
                if (upgrade.isPercentage)
                {
                    int healAmount =
                        Mathf.RoundToInt(stats.maxHealth * finalValue);

                    stats.Heal(healAmount);
                }
                else
                {
                    stats.Heal(Mathf.RoundToInt(upgrade.value));
                }

                break;

            // ─────────────────────────────
            // COMBAT MODIFIERS
            // ─────────────────────────────

            case UpgradeType.BonusProjectiles:
                modifiers.bonusProjectiles += Mathf.RoundToInt(upgrade.value);
                break;

            case UpgradeType.CritChance:
                modifiers.critChanceBonus += finalValue;
                break;

            case UpgradeType.CritDamage:
                modifiers.critDamageBonus += finalValue;
                break;

            case UpgradeType.Lifesteal:
                modifiers.lifestealPercent += finalValue;
                break;

            case UpgradeType.BurnOnHit:
                modifiers.burnOnHit = true;
                modifiers.burnChance += finalValue;
                break;

            case UpgradeType.ExplosionOnKill:
                modifiers.explosiveKills = true;
                break;

            case UpgradeType.Pierce:
                modifiers.bonusPierce += Mathf.RoundToInt(upgrade.value);
                break;

            case UpgradeType.AreaSize:
                modifiers.areaSizeMultiplier += finalValue;
                break;
        }

        Debug.Log("Applied upgrade: " + upgrade.upgradeName);
    }
}