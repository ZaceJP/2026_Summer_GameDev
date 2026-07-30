using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RewardCardUI :
    MonoBehaviour,
    ISelectHandler,
    IDeselectHandler
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text rarityText;
    public Image icon;
    public Button button;

    private UpgradeData currentUpgrade;
    private RewardUI rewardUI;
    private Vector3 originalScale;

    public void Setup(UpgradeData upgrade, RewardUI ui)
    {
        Debug.Log("SETUP CARD: " + upgrade.upgradeName);

        currentUpgrade = upgrade;
        rewardUI = ui;
        originalScale = transform.localScale;

        upgrade.upgradeName.GetLocalizedStringAsync().Completed += handle =>
        {
            titleText.text = handle.Result;
        };

        upgrade.description.GetLocalizedStringAsync().Completed += handle =>
        {
            descriptionText.text = handle.Result;
        };
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

    public void OnSelect(BaseEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale * 1.1f, 0.15f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, 0.15f);
    }
}