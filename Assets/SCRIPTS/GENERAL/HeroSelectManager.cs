using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroSelectManager : MonoBehaviour
{
    public HeroSelection heroSelection;     // drag the HeroSelection SO here
    public HeroDefinition warriorDefinition;
    public HeroDefinition mageDefinition;
    // add more heroes here as you expand

    public void SelectWarrior()
    {
        heroSelection.selectedHero = warriorDefinition;
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

        SceneManager.LoadScene("GameScene");
    }
}