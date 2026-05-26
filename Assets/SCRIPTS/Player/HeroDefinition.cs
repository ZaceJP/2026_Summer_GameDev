using UnityEngine;


[CreateAssetMenu(fileName = "NewHero", menuName = "Game/Hero Definition")]
public class HeroDefinition : ScriptableObject
{
    [Header("Identity")]
    public string heroName;
    public HeroClass heroClass = HeroClass.Universal;
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
    public AttackData specialSkill1;    // Q Key / Controller North (Triangle)
    public AttackData specialSkill2;    // E Key / Controller East (Circle)

    [Header("Skill Icons")]
    public Sprite primaryAttackIcon;
    public Sprite secondaryAttackIcon;
    public Sprite skill1Icon;
    public Sprite skill2Icon;

    [Header("Voice & Status SFX")]
    public AudioClip getHitSFX;
    public AudioClip dieSFX;

    [Header("Combat SFX")]
    public AudioClip primaryAttackSFX;
    public AudioClip secondaryAttackSFX;
    public AudioClip specialSkill1SFX;
    public AudioClip specialSkill2SFX;
}