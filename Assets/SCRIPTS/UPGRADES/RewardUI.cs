using System.Collections.Generic;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    public static RewardUI Instance;

    public GameObject panel;
    public RewardCardUI[] cards;

    private PlayerStats playerStats;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    public void ShowRewards(List<UpgradeData> rewards)
    {
        panel.SetActive(true);

        Time.timeScale = 0f;

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < rewards.Count)
            {
                cards[i].gameObject.SetActive(true);
                cards[i].Setup(rewards[i], this);
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectReward(UpgradeData reward)
    {
        UpgradeApplier applier =
         FindFirstObjectByType<UpgradeApplier>();

        applier.ApplyUpgrade(reward);

        panel.SetActive(false);

        Time.timeScale = 1f;
    }
}