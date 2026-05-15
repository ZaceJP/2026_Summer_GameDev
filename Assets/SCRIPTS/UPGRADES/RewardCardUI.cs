using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCardUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text rarityText;
    public Sprite icon;
    public Button button;

    private UpgradeData currentUpgrade;
    private RewardUI rewardUI;

    public void Setup(UpgradeData upgrade, RewardUI ui)
    {
        Debug.Log("SETUP CARD: " + upgrade.upgradeName);

        currentUpgrade = upgrade;
        rewardUI = ui;

        titleText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.description;
        // rarityText.text = upgrade.rarity;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnSelected);

        Debug.Log("BUTTON CONNECTED");
    }

    void OnSelected()
    {
        Debug.Log("CARD CLICKED");
        rewardUI.SelectReward(currentUpgrade);
    }
}