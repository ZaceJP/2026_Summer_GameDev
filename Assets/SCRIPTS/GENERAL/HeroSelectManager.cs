using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroSelectManager : MonoBehaviour
{
    [Header("Global Systems Connection")]
    public LevelTransition levelTransitionScript;

    public HeroSelection heroSelection;     // drag the HeroSelection SO here
    public HeroDefinition warriorDefinition;
    public HeroDefinition mageDefinition;
    public HeroDefinition rikutoDefinition;
    // add more heroes here as you expand

    public void SelectWarrior()
    {
        heroSelection.selectedHero = warriorDefinition;
    }

    public void SelectRikuto()
    {
        heroSelection.selectedHero = rikutoDefinition;
    }
    public void SelectMage()
    {
        heroSelection.selectedHero = mageDefinition;
    }

    public void StartGame()
    {
        if (heroSelection.selectedHero == null)
        {
            Debug.LogWarning("No hero selected!");
            return;

        }
        if (levelTransitionScript != null)
        {
            levelTransitionScript.LoadSceneByName("GameScene");
        }
    }
}