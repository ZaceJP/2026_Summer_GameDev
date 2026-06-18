using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 2f;

    // Optional: Keep your global spacebar check for debugging, or remove it entirely if unwanted
    void Update()
    {
        // LoadNextLevel();
    }

    // Standard progression (Build Index + 1)
    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex + 1));
    }

    // BRAND NEW: Public method allowing other managers to call specific scenes by name
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadLevelRoutine(sceneName));
    }

    // Overloaded Coroutine handling build indexes
    private IEnumerator LoadLevelRoutine(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }

    // Overloaded Coroutine handling scene names
    private IEnumerator LoadLevelRoutine(string sceneName)
    {
        yield return null; // wait one frame

        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }


}