using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [HideInInspector] public HeroDefinition heroDefinition;

    private void Start()
    {
        if (heroDefinition == null)
        {
            Debug.LogError("PlayerInitializer: No HeroDefinition assigned!");
            return;
        }

        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.maxHealth = heroDefinition.maxHealth;
            stats.moveSpeed = heroDefinition.moveSpeed;
            stats.baseDamage = heroDefinition.baseDamage;
            stats.attackSpeed = heroDefinition.attackSpeed;
            stats.attackRange = heroDefinition.attackRange;
            stats.Init(heroDefinition.maxHealth);  // sets currentHealth
        }

        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null)
            attack.attackData = heroDefinition.primaryAttack;
    }
}