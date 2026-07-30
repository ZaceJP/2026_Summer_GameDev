using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Localization.Components;

public class HeroSelectManager : MonoBehaviour
{
    [Header("Global Systems")]
    public LevelTransition levelTransitionScript;
    public HeroSelection heroSelection;

    [Header("UI")]
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private TMP_Text heroDescriptionText;

    [Header("Heroes")]
    [SerializeField] private HeroSelectCharacter[] heroes;

    private int currentIndex = 0;
    private float inputCooldown = 0.2f;
    private float inputTimer;

    [SerializeField]
    private float selectionCooldown = 2.5f;

    private HeroSelectCharacter currentSelection;

    private bool canSelect = true;


    private void Start()
    {
        if (heroes.Length == 0)
            return;

        currentIndex = 0;

        canSelect = true;

        SelectCharacter(heroes[currentIndex]);
    }

    public void SelectCharacter(HeroSelectCharacter hero)
    {
        if (!canSelect)
            return;

        if (currentSelection == hero)
            currentIndex = System.Array.IndexOf(heroes, hero);

        currentIndex = System.Array.IndexOf(heroes, hero);

        canSelect = false;

        if (currentSelection != null)
            currentSelection.Deselect();

        currentSelection = hero;
        currentSelection.Select();

        heroSelection.selectedHero = hero.heroDefinition;

        UpdateHeroUI(hero.heroDefinition);

        StartCoroutine(SelectionCooldown());
    }

    private void Update()
    {
        inputTimer -= Time.deltaTime;

        float horizontal = Input.GetAxisRaw("Horizontal");

        if (inputTimer <= 0f)
        {
            if (horizontal > 0.5f)
            {
                NextHero();
            }
            else if (horizontal < -0.5f)
            {
                PreviousHero();
            }
        }

        if (Input.GetButtonDown("Submit"))
        {
            StartGame();
        }
    }

    void NextHero()
    {
        if (heroes.Length == 0)
            return;

        int next = (currentIndex + 1) % heroes.Length;

        SelectCharacter(heroes[next]);
    }

    void PreviousHero()
    {
        if (heroes.Length == 0)
            return;

        int previous = (currentIndex - 1 + heroes.Length) % heroes.Length;

        SelectCharacter(heroes[previous]);
    }
    private IEnumerator SelectionCooldown()
    {
        yield return new WaitForSeconds(selectionCooldown);
        canSelect = true;
    }

    private void UpdateHeroUI(HeroDefinition hero)
    {
        if (hero == null)
            return;

        hero.heroName.GetLocalizedStringAsync().Completed += handle =>
        {
            heroNameText.text = handle.Result;
        };

        hero.heroDescription.GetLocalizedStringAsync().Completed += handle =>
        {
            heroDescriptionText.text = handle.Result;
        };
    }

    public void StartGame()
    {
        if (heroSelection.selectedHero == null)
        {
            Debug.LogWarning("No hero selected!");
            return;
        }

        levelTransitionScript.LoadSceneByName("GameScene");
    }
}