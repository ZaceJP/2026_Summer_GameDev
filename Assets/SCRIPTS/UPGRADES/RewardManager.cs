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
        List<UpgradeData> pool = new(allUpgrades);

        for (int i = 0; i < amount; i++)
        {
            if (pool.Count == 0)
                break;

            int randomIndex = Random.Range(0, pool.Count);

            rewards.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return rewards;
    }
}