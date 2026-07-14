using UnityEngine;

public enum AttackType { Melee, Projectile }


public enum AttackShape {Box,Circle}

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


[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    public AttackType attackType;

    [Header("Shared")]
    public float cooldown = 0.5f;
    public float attackRange = 2f;    // how close to trigger attack
    public int damage = 10;    // base damage — player overrides with PlayerStats
    public AttackShape attackShape = AttackShape.Box;

    [Header("Special Skill")]

    public SkillEffect skillEffect = SkillEffect.None;

    public int healAmount = 30;

    public int shieldAmount = 100;

    public float shieldDuration = 5f;

    public float effectRadius = 4f;


    [Header("Knockback")]
    public bool applyKnockback = false;
    public float knockbackForce = 3f;


    [Header("Melee Only")]
    public float meleeOffset = 0.2f;
    public GameObject skillVFXPrefab;
    public Vector2 skillVisualScale = Vector2.one;
    public float skillVFXLifetime = 0.3f;


    [Header("Projectile Only")]
    public ProjectileData projectileData;
    public int projectileCount = 1;
    public float spreadAngle = 15f;


}