using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems; // ADDED: Required for targeting active buttons
using System.Collections;

public class TitleScreenManager : MonoBehaviour
{
    [Header("UI Canvas Panels")]
    public GameObject mainMenuCanvas;
    public GameObject optionsMenuCanvas;
    public GameObject exitMenuCanvas;

    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button optionsButton;
    public Button exitButton;

    [Header("Exit Menu Buttons")]
    public Button confirmExitButton;
    public Button cancelExitButton;

    [Header("Options Menu Buttons (Add your back button here)")]
    public Button optionsBackButton; // ADDED: Crucial to focus when entering options

    [Header("Cinemachine Cameras")]
    public GameObject mainMenuCam;
    public GameObject optionsMenuCam;
    public GameObject exitMenuCam;
    public GameObject startGameCam;

    [Header("Global Systems Connection")]
    public LevelTransition levelTransitionScript;

    [Header("Timing Configuration")]
    public float standardBlendTime = 1.5f;
    public float portalFlyThroughTime = 2.0f;

    private void Start()
    {
        Debug.Log("TITLE SCREEN START");


        Time.timeScale = 1f;

        playButton.onClick.AddListener(OnPlayPressed);
        optionsButton.onClick.AddListener(OnOptionsPressed);
        exitButton.onClick.AddListener(OnExitPressed);

        if (confirmExitButton != null) confirmExitButton.onClick.AddListener(ExecuteRealQuit);
        if (cancelExitButton != null) cancelExitButton.onClick.AddListener(OnCancelExitPressed);

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic(MusicType.Title);

        // Focus the first button on startup so controllers work immediately
        SetFocusedButton(playButton.gameObject);
    }

    // Helper method to cleanly set gamepad focus
    private void SetFocusedButton(GameObject buttonToSelect)
    {
        if (buttonToSelect == null) return;

        // Clear current focus first
        EventSystem.current.SetSelectedGameObject(null);
        // Assign new target
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }

    public void OnPlayPressed()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySFX(MusicManager.Instance.confirmSound);

        StartCoroutine(PortalSequence());
    }

    private IEnumerator PortalSequence()
    {
        mainMenuCanvas.SetActive(false);
        mainMenuCam.SetActive(false);
        startGameCam.SetActive(true);

        float delayBeforeFade = Mathf.Max(0, portalFlyThroughTime - levelTransitionScript.transitionTime);
        yield return new WaitForSeconds(delayBeforeFade);

        if (levelTransitionScript != null)
        {
            levelTransitionScript.LoadSceneByName("Hero_Select");
        }
    }

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
        // Focus the back button inside the options panel
        SetFocusedButton(optionsBackButton != null ? optionsBackButton.gameObject : null);
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
        // Give control back to the main menu choices
        SetFocusedButton(optionsButton.gameObject);
    }

    private void OnExitPressed()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlaySFX(MusicManager.Instance.confirmSound);

        StartCoroutine(TransitionToExitMenu());
    }

    private IEnumerator TransitionToExitMenu()
    {
        mainMenuCanvas.SetActive(false);
        mainMenuCam.SetActive(false);
        exitMenuCam.SetActive(true);

        yield return new WaitForSeconds(standardBlendTime);

        exitMenuCanvas.SetActive(true);
        // Default controller selector safely onto "Cancel" so players don't accidentally rage quit
        SetFocusedButton(cancelExitButton.gameObject);
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
        SetFocusedButton(exitButton.gameObject);
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