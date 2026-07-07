using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player Info")]
    public Image heroImage;
    private PlayerAttack playerAttack;

    // ─────────────────────────────
    // HP
    // ─────────────────────────────

    [Header("HP")]
    public Image hpFillImage;
    public TMP_Text hpText;

    // ─────────────────────────────
    // SKILL ICONS
    // ─────────────────────────────

    [Header("Skill Icons")]
    public Image primaryIcon;
    public Image secondaryIcon;
    public Image skill1Icon;
    public Image skill2Icon;

    [Header("Cooldown Overlays")]

    public Image primaryCooldown;
    public Image secondaryCooldown;
    public Image skill1Cooldown;
    public Image skill2Cooldown;

    // ─────────────────────────────
    // INPUT PROMPTS
    // ─────────────────────────────

    [Header("Key Prompt Images")]
    public Image primaryInputImage;
    public Image secondaryInputImage;
    public Image skill1InputImage;
    public Image skill2InputImage;

    // ─────────────────────────────
    // KEYBOARD ICONS
    // ─────────────────────────────

    [Header("Keyboard Icons")]
    public Sprite lmbIcon;
    public Sprite rmbIcon;
    public Sprite qIcon;
    public Sprite eIcon;

    // ─────────────────────────────
    // CONTROLLER ICONS
    // ─────────────────────────────

    [Header("Controller Icons")]
    public Sprite xButtonIcon;
    public Sprite squareButtonIcon;
    public Sprite triangleButtonIcon;
    public Sprite circleButtonIcon;

    // ─────────────────────────────

    private PlayerStats playerStats;

    private bool usingController;

    // ─────────────────────────────
    // START
    // ─────────────────────────────

    private void Start()
    {
        FindPlayer();

        SetupHeroUI();

        UpdateInputPrompts();
    }

    // ─────────────────────────────
    // UPDATE
    // ─────────────────────────────

    private void Update()
    {
        if (playerStats == null)
        {
            FindPlayer();
            return;
        }

        UpdateHP();

        UpdateCooldowns();

        DetectInputType();
    }

    // ─────────────────────────────
    // FIND PLAYER
    // ─────────────────────────────

    void FindPlayer()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        playerAttack =
         player.GetComponent<PlayerAttack>();

        if (player != null)
        {
            playerStats =
                player.GetComponent<PlayerStats>();

            SetupHeroUI();
        }
    }

    // ─────────────────────────────
    // HERO UI
    // ─────────────────────────────

    void SetupHeroUI()
    {
        if (
            playerStats == null ||
            playerStats.heroDef == null
        )
            return;

        HeroDefinition hero =
            playerStats.heroDef;

        // HERO PORTRAIT
        if (heroImage != null)
            heroImage.sprite = hero.portrait;

        // SKILL ICONS
        if (primaryIcon != null)
            primaryIcon.sprite = hero.primaryAttackIcon;

        if (secondaryIcon != null)
            secondaryIcon.sprite = hero.secondaryAttackIcon;

        if (skill1Icon != null)
            skill1Icon.sprite = hero.skill1Icon;

        if (skill2Icon != null)
            skill2Icon.sprite = hero.skill2Icon;
    }

    // ─────────────────────────────
    // HP
    // ─────────────────────────────

    void UpdateHP()
    {
        if (hpText != null)
        {
            hpText.text =
                playerStats.currentHealth +
                " / " +
                playerStats.maxHealth;
        }

        if (hpFillImage != null)
        {
            hpFillImage.fillAmount =
                (float)playerStats.currentHealth /
                playerStats.maxHealth;
        }
    }


    void UpdateCooldowns()
    {
        if (playerAttack == null)
            return;

        HeroDefinition hero = playerStats.heroDef;

        if (primaryCooldown != null)
            primaryCooldown.fillAmount =
                playerAttack.GetCooldownPercent(hero.primaryAttack);

        if (secondaryCooldown != null)
            secondaryCooldown.fillAmount =
                playerAttack.GetCooldownPercent(hero.secondaryAttack);

        if (skill1Cooldown != null)
            skill1Cooldown.fillAmount =
                playerAttack.GetCooldownPercent(hero.specialSkill1);

        if (skill2Cooldown != null)
            skill2Cooldown.fillAmount =
                playerAttack.GetCooldownPercent(hero.specialSkill2);
    }

    // ─────────────────────────────
    // INPUT DETECTION
    // ─────────────────────────────

    void DetectInputType()
    {
        bool controllerNow =
            Gamepad.current != null &&
            Gamepad.current.wasUpdatedThisFrame;

        bool mouseNow =
            Mouse.current != null &&
            Mouse.current.wasUpdatedThisFrame;

        if (controllerNow && !usingController)
        {
            usingController = true;
            UpdateInputPrompts();
        }

        if (mouseNow && usingController)
        {
            usingController = false;
            UpdateInputPrompts();
        }
    }

    // ─────────────────────────────
    // UPDATE INPUT ICONS
    // ─────────────────────────────

    void UpdateInputPrompts()
    {
        if (usingController)
        {
            if (primaryInputImage != null)
                primaryInputImage.sprite = xButtonIcon;

            if (secondaryInputImage != null)
                secondaryInputImage.sprite = squareButtonIcon;

            if (skill1InputImage != null)
                skill1InputImage.sprite = triangleButtonIcon;

            if (skill2InputImage != null)
                skill2InputImage.sprite = circleButtonIcon;
        }
        else
        {
            if (primaryInputImage != null)
                primaryInputImage.sprite = lmbIcon;

            if (secondaryInputImage != null)
                secondaryInputImage.sprite = rmbIcon;

            if (skill1InputImage != null)
                skill1InputImage.sprite = qIcon;

            if (skill2InputImage != null)
                skill2InputImage.sprite = eIcon;
        }
    }
}