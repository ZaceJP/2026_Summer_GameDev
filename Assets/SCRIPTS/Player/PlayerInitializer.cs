using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [HideInInspector] public HeroDefinition heroDefinition;

    public void Initialize()
    {
        if (heroDefinition == null)
        {
            Debug.LogError($"[PlayerInitializer] No HeroDefinition assigned to {gameObject.name}!");
            return;
        }

        // 1. Initialize Stats
        // We use GetComponentInChildren in case the scripts are on a child "Logic" object
        PlayerStats stats = GetComponentInChildren<PlayerStats>();
        if (stats != null)
        {
            stats.heroDef = heroDefinition;

            stats.maxHealth = heroDefinition.maxHealth;
            stats.moveSpeed = heroDefinition.moveSpeed;
            stats.baseDamage = heroDefinition.baseDamage;
            stats.attackSpeed = heroDefinition.attackSpeed;
            stats.attackRange = heroDefinition.attackRange;

            // This sets currentHealth to maxHealth
            stats.Init(heroDefinition.maxHealth);

            Debug.Log($"[Initializer] {heroDefinition.heroName} stats initialized. HP: {stats.maxHealth}, Speed: {stats.moveSpeed}");
        }

        // 2. Initialize Combat
        PlayerAttack attack = GetComponentInChildren<PlayerAttack>();
        if (attack != null)
        {
            // Hand over the whole SO so the attack script can access primary/secondary attacks
            attack.heroDef = heroDefinition;
            Debug.Log($"[Initializer] {heroDefinition.heroName} combat skills linked.");
        }
    }
}