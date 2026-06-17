using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenManager : MonoBehaviour
{
    [Header("UI Canvas Panels")]
    public GameObject mainMenuCanvas;
    public GameObject optionsMenuCanvas;
    public GameObject exitMenuCanvas; // Drag your Exit Confirmation Panel here

    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button optionsButton;
    public Button exitButton;

    [Header("Exit Menu Buttons")]
    public Button confirmExitButton;
    public Button cancelExitButton;

    [Header("Cinemachine Cameras")]
    public GameObject mainMenuCam;
    public GameObject optionsMenuCam;
    public GameObject exitMenuCam;   // Drag vcam_ExitMenu here
    public GameObject startGameCam;  // Drag vcam_StartGame here

    [Header("Timing Configuration")]
    [Tooltip("Time it takes for camera to move to Options or Exit view")]
    public float standardBlendTime = 1.5f;

    [Tooltip("Time it takes for the camera to plunge into the portal")]
    public float portalFlyThroughTime = 2.0f;

    private void Start()
    {
        Time.timeScale = 1f;

        // Main Menu Button Links
        playButton.onClick.AddListener(OnPlayPressed);
        optionsButton.onClick.AddListener(OnOptionsPressed);
        exitButton.onClick.AddListener(OnExitPressed);

        // Exit Menu Button Links
        if (confirmExitButton != null) confirmExitButton.onClick.AddListener(ExecuteRealQuit);
        if (cancelExitButton != null) cancelExitButton.onClick.AddListener(OnCancelExitPressed);

        // Play title music
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic(MusicType.Title);
    }

    // ==========================================
    // 1. PLAY / PORTAL SEQUENCE
    // ==========================================
    private void OnPlayPressed()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySFX(MusicManager.Instance.confirmSound);

        StartCoroutine(PortalSequence());
    }

    private IEnumerator PortalSequence()
    {
        // Hide UI immediately so it doesn't clip through the world
        mainMenuCanvas.SetActive(false);

        // Activate portal camera (Cinemachine flies the main camera INTO the portal)
        mainMenuCam.SetActive(false);
        startGameCam.SetActive(true);

        // Optional: If you have a screen fade-to-black script, trigger it here!
        // e.g., FadeManager.Instance.FadeToBlack(portalFlyThroughTime);

        // Wait for the camera to completely pass through the portal
        yield return new WaitForSeconds(portalFlyThroughTime);

        // Load the next scene seamlessly
        SceneManager.LoadScene("HeroSelect");
    }

    // ==========================================
    // 2. OPTIONS MENU TRANSITIONS
    // ==========================================
    private void OnOptionsPressed()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySFX(MusicManager.Instance.confirmSound);

        StartCoroutine(TransitionToOptions());
    }

    private IEnumerator TransitionToOptions()
    {
        mainMenuCanvas.SetActive(false);
        mainMenuCam.SetActive(false);
        optionsMenuCam.SetActive(true);

        yield return new WaitForSeconds(standardBlendTime);

        optionsMenuCanvas.SetActive(true);
    }

    public void OnBackPressedFromOptions()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySFX(MusicManager.Instance.confirmSound);

        StartCoroutine(TransitionOptionsToMain());
    }

    private IEnumerator TransitionOptionsToMain()
    {
        optionsMenuCanvas.SetActive(false);
        optionsMenuCam.SetActive(false);
        mainMenuCam.SetActive(true);

        yield return new WaitForSeconds(standardBlendTime);

        mainMenuCanvas.SetActive(true);
    }

    // ==========================================
    // 3. EXIT MENU TRANSITIONS
    // ==========================================
    private void OnExitPressed()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySFX(MusicManager.Instance.confirmSound);

        StartCoroutine(TransitionToExitMenu());
    }

    private IEnumerator TransitionToExitMenu()
    {
        // Hide main text, pan camera to the exit framing angle
        mainMenuCanvas.SetActive(false);
        mainMenuCam.SetActive(false);
        exitMenuCam.SetActive(true);

        yield return new WaitForSeconds(standardBlendTime);

        // Show "Are you sure you want to quit?" menu
        exitMenuCanvas.SetActive(true);
    }

    private void OnCancelExitPressed()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySFX(MusicManager.Instance.confirmSound);

        StartCoroutine(TransitionExitToMain());
    }

    private IEnumerator TransitionExitToMain()
    {
        exitMenuCanvas.SetActive(false);
        exitMenuCam.SetActive(false);
        mainMenuCam.SetActive(true);

        yield return new WaitForSeconds(standardBlendTime);

        mainMenuCanvas.SetActive(true);
    }

    private void ExecuteRealQuit()
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