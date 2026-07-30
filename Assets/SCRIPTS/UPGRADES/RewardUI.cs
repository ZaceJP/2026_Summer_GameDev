using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // ADDED: Required for gamepad control

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

        CameraFollow cam = FindFirstObjectByType<CameraFollow>();

        if (cam != null && cam.firstPersonMode)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        GameObject firstActiveCard = null;

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < rewards.Count)
            {
                cards[i].gameObject.SetActive(true);
                cards[i].Setup(rewards[i], this);

                // Track the very first card we set active so we can focus it
                if (firstActiveCard == null)
                {
                    firstActiveCard = cards[i].button.gameObject;
                }
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }

        // GAMEPAD FOCUS: Force selection onto the first active reward card!
        if (firstActiveCard != null)
        {
            StartCoroutine(SelectFirstCard(firstActiveCard));
        }
    }

    public void SelectReward(UpgradeData reward)
    {
        UpgradeApplier applier =
         FindFirstObjectByType<UpgradeApplier>();

        applier.ApplyUpgrade(reward);

        panel.SetActive(false);

        CameraFollow cam = FindFirstObjectByType<CameraFollow>();

        if (cam != null && cam.firstPersonMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Time.timeScale = 1f;
    }

    private IEnumerator SelectFirstCard(GameObject button)
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}