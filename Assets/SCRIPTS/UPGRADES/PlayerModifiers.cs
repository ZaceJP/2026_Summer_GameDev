using UnityEngine;

public class PlayerModifiers : MonoBehaviour
{
    [Header("Projectile")]
    public int bonusProjectiles = 0;
    public float projectileSpreadBonus = 0f;
    public float projectileSpeedMultiplier = 1f;
    public int bonusPierce = 0;

    [Header("Critical")]
    public float critChanceBonus = 0f;
    public float critDamageBonus = 0.5f;

    [Header("Status Effects")]
    public bool burnOnHit = false;
    public float burnChance = 0f;
    public float burnDuration = 0f;
    public int burnDamagePerTick = 0;

    public bool slowOnHit = false;
    public float slowAmount = 0f;
    public float slowDuration = 0f;

    [Header("Explosions")]
    public bool explosiveKills = false;
    public float explosionRadius = 0f;
    public int explosionDamage = 0;

    [Header("Sustain")]
    public float lifestealPercent = 0f;

    [Header("Utility")]
    public bool leaveTrailEffect = false;
    public bool gainBonusWhileStandingStill = false;

    [Header("Area")]
    public float areaSizeMultiplier = 1f;

    public void ResetModifiers()
    {
        bonusProjectiles = 0;
        projectileSpreadBonus = 0f;
        projectileSpeedMultiplier = 1f;
        bonusPierce = 0;

        critChanceBonus = 0f;
        critDamageBonus = 0.5f;

        burnOnHit = false;
        burnChance = 0f;
        burnDuration = 0f;
        burnDamagePerTick = 0;

        slowOnHit = false;
        slowAmount = 0f;
        slowDuration = 0f;

        explosiveKills = false;
        explosionRadius = 0f;
        explosionDamage = 0;

        lifestealPercent = 0f;

        leaveTrailEffect = false;
        gainBonusWhileStandingStill = false;

        areaSizeMultiplier = 1f;
    }
}