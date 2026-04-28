using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "Game/Hero Definition")]
public class HeroDefinition : ScriptableObject
{
    [Header("Identity")]
    public string heroName;
    public Sprite portrait;
    public GameObject prefab;

    [Header("Base Stats")]
    public int maxHealth = 100;
    public float moveSpeed = 5f;
    public float attackSpeed = 1f;      // attacks per second
    public int baseDamage = 10;
    public float attackRange = 2f;

    [Header("Skills")]
    public AttackData primaryAttack;    // left click / main attack
    public AttackData secondaryAttack;  // right click / skill — null if none
    // expand later: public SkillData[] skillSlots;
}