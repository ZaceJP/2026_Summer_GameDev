using UnityEngine;

public enum AttackType
{
    Melee,
    Projectile
}

public enum AttackShape
{
    Box,
    Circle
}

public enum SkillEffect
{
    None,

    Heal,
    ShadowExplosion,
    ShadowBarrier,

    EarthStomp,
    Rage,

    FrostNova,
    MeteorRain
}

public enum TargetType
{
    Self,
    Direction
}

[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    [Header("General")]
    public AttackType attackType;
    public AttackShape attackShape = AttackShape.Box;

    public float cooldown = 0.5f;
    public float attackRange = 2f;
    public int damage = 10;

    [Header("Skill")]
    public SkillEffect skillEffect = SkillEffect.None;

    [Header("Targeting")]
    public TargetType targetType = TargetType.Self;

    // Distance from player if using Direction targeting
    public float targetDistance = 6f;

    // Delay before the spell activates
    public float castDelay = 0.75f;

    // Optional targeting circle
    public GameObject targetIndicatorPrefab;

    [Header("Status Effects")]
    public GameObject statusEffectVFX;

    [Header("Heal")]
    public int healAmount = 30;

    [Header("Barrier")]
    public int shieldAmount = 100;
    public float shieldDuration = 5f;

    [Header("Area Skills")]
    public float effectRadius = 4f;

    [Header("Freeze")]
    public float freezeDuration = 3f;

    [Header("Knockback")]
    public bool applyKnockback = false;
    public float knockbackForce = 3f;

    [Header("Targeting ATtack")]
    public int meteorCount = 8;
    public float meteorSpread = 3f;
    public float meteorSpawnHeight = 15f;
    public float meteorSpawnInterval = 0.15f;

    [Header("Melee")]
    public float meleeOffset = 0.2f;
    public GameObject skillVFXPrefab;
    public Vector2 skillVisualScale = Vector2.one;
    public float skillVFXLifetime = 0.3f;

    [Header("Projectile")]
    public ProjectileData projectileData;
    public int projectileCount = 1;
    public float spreadAngle = 15f;
}