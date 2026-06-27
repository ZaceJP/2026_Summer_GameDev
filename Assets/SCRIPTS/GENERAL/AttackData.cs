using UnityEngine;

public enum AttackType { Melee, Projectile }


public enum AttackShape {Box,Circle}


[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    public AttackType attackType;

    [Header("Shared")]
    public float cooldown = 0.5f;
    public float attackRange = 2f;    // how close to trigger attack
    public int damage = 10;    // base damage — player overrides with PlayerStats
    public AttackShape attackShape = AttackShape.Box;

    [Header("Knockback")]
    public bool applyKnockback = false;
    public float knockbackForce = 3f;


    [Header("Melee Only")]
    public float meleeOffset = 0.2f;
    public GameObject slashVFXPrefab;
    public Vector2 slashVisualScale = Vector2.one;
    public float slashVFXLifetime = 0.3f;


    [Header("Projectile Only")]
    public ProjectileData projectileData;
    public int projectileCount = 1;
    public float spreadAngle = 15f;


}