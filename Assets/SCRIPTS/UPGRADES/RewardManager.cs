using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    [Header("All Possible Upgrades")]
    public List<UpgradeData> allUpgrades = new();

    private void Awake()
    {
        Instance = this;
    }

    public List<UpgradeData> GenerateRewards(int amount = 3)
    {
        List<UpgradeData> rewards = new();

        // FIND PLAYER
        PlayerAttack player =
            FindFirstObjectByType<PlayerAttack>();

        if (player == null || player.heroDef == null)
        {
            Debug.LogWarning("No player or hero definition found!");
            return rewards;
        }

        HeroClass playerClass =
            player.heroDef.heroClass;

        // FILTER VALID UPGRADES
        List<UpgradeData> validPool = new();

        foreach (UpgradeData upgrade in allUpgrades)
        {
            if (upgrade.allowedClass == HeroClass.Universal ||
                upgrade.allowedClass == playerClass)
            {
                validPool.Add(upgrade);
            }
        }

        // RANDOMLY PICK REWARDS
        for (int i = 0; i < amount; i++)
        {
            if (validPool.Count == 0)
                break;

            int randomIndex =
                Random.Range(0, validPool.Count);

            rewards.Add(validPool[randomIndex]);

            validPool.RemoveAt(randomIndex);
        }

        return rewards;
    }
}