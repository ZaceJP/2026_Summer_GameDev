using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Required for TextMeshPro components

public enum GameEndState
{
    GameOver,
    GameClear
}

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager Instance;

    [Header("UI Panels")]
    public GameObject endScreenPanel; // The master Canvas/Panel

    [Header("Dynamic UI Elements")]
    public TMP_Text endScreenTitleText;   // Swapped to TMP_Text for TextMeshPro
    public string gameOverMessage = "GAME OVER";
    public string gameClearMessage = "STAGE CLEAR!";

    [Header("Buttons")]
    public Button actionButton;       // Dual-purpose: "Restart" on loss, "Next Level" on win
    public Button mainMenuButton;

    [Header("Optional SFX Override")]
    public AudioClip victorySFX;
    public AudioClip defeatSFX;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Hide the panel when the stage starts
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);
    }

    private void Start()
    {
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuPressed);
    }

    /// <summary>
    /// Call this to trigger the ending screen. 
    /// </summary>
    public void TriggerEndScreen(GameEndState state)
    {
        if (endScreenPanel == null) return;

        // Clean up any listeners from previous runs before setting new ones
        actionButton.onClick.RemoveAllListeners();

        // Configure the UI based on Win vs. Loss
        if (state == GameEndState.GameOver)
        {
            if (endScreenTitleText != null) endScreenTitleText.text = gameOverMessage;

            // Set action button to restart the current level
            if (actionButton != null)
            {
                // Swapped to TMP_Text to find the text component inside the TMP Button
                actionButton.GetComponentInChildren<TMP_Text>().text = "Restart";
                actionButton.onClick.AddListener(OnRestartPressed);
            }

            // Play defeat audio via your MusicManager
            if (MusicManager.Instance != null && defeatSFX != null)
                MusicManager.Instance.PlaySFX(defeatSFX);
        }
        else if (state == GameEndState.GameClear)
        {
            if (endScreenTitleText != null) endScreenTitleText.text = gameClearMessage;

            // Set action button to progress
            if (actionButton != null)
            {
                // Swapped to TMP_Text to find the text component inside the TMP Button
                actionButton.GetComponentInChildren<TMP_Text>().text = "Next Stage";
                actionButton.onClick.AddListener(OnNextStagePressed);
            }

            // Play victory audio via your MusicManager
            if (MusicManager.Instance != null && victorySFX != null)
                MusicManager.Instance.PlaySFX(victorySFX);
        }

        // Display panel and pause game logic
        endScreenPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnRestartPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnNextStagePressed()
    {
        Time.timeScale = 1f;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    private void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }
}