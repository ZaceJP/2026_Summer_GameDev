using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject optionsMenu; // Reference to the Options Menu Panel
    public Button playButton;
    public Button optionsButton;
    public Button exitButton;

    private void Start()
    {
        // Button setup
        playButton.onClick.AddListener(OnPlayPressed);
        optionsButton.onClick.AddListener(OnOptionsPressed);
        exitButton.onClick.AddListener(OnExitPressed);

        // Play title music
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic(MusicType.Title);
    }

    private void OnPlayPressed()
    {
        // Play button sound
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySFX(MusicManager.Instance.confirmSound);

        // Load next scene
        SceneManager.LoadScene("HeroSelect"); // change to your first playable scene
    }

    private void OnOptionsPressed()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySFX(MusicManager.Instance.confirmSound);

        optionsMenu.SetActive(true);
        gameObject.SetActive(false); // hide main menu
    }

    private void OnExitPressed()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySFX(MusicManager.Instance.confirmSound);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
