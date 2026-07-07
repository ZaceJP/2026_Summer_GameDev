using UnityEngine;
using System.Collections;
using TMPro;

public class HeroSelectManager : MonoBehaviour
{
    [Header("Global Systems")]
    public LevelTransition levelTransitionScript;
    public HeroSelection heroSelection;

    [Header("UI")]
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private TMP_Text heroDescriptionText;

    [SerializeField]
    private float selectionCooldown = 2.5f;

    private HeroSelectCharacter currentSelection;

    private bool canSelect = true;


    public void SelectCharacter(HeroSelectCharacter hero)
    {
        if (!canSelect)
            return;

        if (currentSelection == hero)
            return;

        canSelect = false;

        if (currentSelection != null)
            currentSelection.Deselect();

        currentSelection = hero;
        currentSelection.Select();

        heroSelection.selectedHero = hero.heroDefinition;

        UpdateHeroUI(hero.heroDefinition);

        StartCoroutine(SelectionCooldown());
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

        heroNameText.text = hero.heroName;
        heroDescriptionText.text = hero.heroDescription;
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