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
        switch (upgrade.upgradeType)
        {
            // ─────────────────────────────
            // BASIC STATS
            // ─────────────────────────────

            case UpgradeType.Damage:
                stats.ApplyDamageBonus(upgrade.value);
                break;

            case UpgradeType.AttackSpeed:
                stats.ApplyAttackSpeedBonus(upgrade.value);
                break;

            case UpgradeType.MoveSpeed:
                stats.ApplySpeedBonus(upgrade.value);
                break;

            case UpgradeType.AttackRange:
                stats.ApplyRangeBonus(upgrade.value);
                break;

            case UpgradeType.MaxHealth:
                stats.maxHealth += Mathf.RoundToInt(upgrade.value);
                stats.currentHealth += Mathf.RoundToInt(upgrade.value);
                break;

            // ─────────────────────────────
            // UTILITY
            // ─────────────────────────────

            case UpgradeType.Heal:
                stats.Heal(Mathf.RoundToInt(stats.maxHealth * upgrade.value));
                break;

            // ─────────────────────────────
            // COMBAT MODIFIERS
            // ─────────────────────────────

            case UpgradeType.BonusProjectiles:
                modifiers.bonusProjectiles += Mathf.RoundToInt(upgrade.value);
                break;

            case UpgradeType.CritChance:
                modifiers.critChanceBonus += upgrade.value;
                break;

            case UpgradeType.CritDamage:
                modifiers.critDamageBonus += upgrade.value;
                break;

            case UpgradeType.Lifesteal:
                modifiers.lifestealPercent += upgrade.value;
                break;

            case UpgradeType.BurnOnHit:
                modifiers.burnOnHit = true;
                modifiers.burnChance += upgrade.value;
                break;

            case UpgradeType.ExplosionOnKill:
                modifiers.explosiveKills = true;
                break;

            case UpgradeType.Pierce:
                modifiers.bonusPierce += Mathf.RoundToInt(upgrade.value);
                break;

            case UpgradeType.AreaSize:
                modifiers.areaSizeMultiplier += upgrade.value;
                break;
        }

        Debug.Log("Applied upgrade: " + upgrade.upgradeName);
    }
}