using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

public class OptionsMenuManager : MonoBehaviour
{
    [Header("Title Screen Script Reference")]
    public TitleScreenManager titleScreenManager;

    [Header("Audio UI Elements")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Input Control UI Elements")]
    public Toggle controlTypeToggle;
    // ADD THESE TWO NEW TEXT REFERENCES:
    public TMP_Text keyboardText;
    public TMP_Text controllerText;

    [Header("Color Styling")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.3f); // Faded/dimmed look

    [Header("Language UI Elements")]
    public TMP_Dropdown languageDropdown;

    [Header("Return Button")]
    public Button backButton;

    public bool isUsingController { get; private set; } = false;
    

    private void Start()
    {
        InitializeUIValues();

        // Hook up UI Event listeners
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        controlTypeToggle.onValueChanged.AddListener(OnControlTypeChanged);
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        backButton.onClick.AddListener(OnBackClicked);

        // Run the visual update once at start so the colors match the default state
        UpdateToggleVisuals(controlTypeToggle.isOn);
    }

    private void InitializeUIValues()
    {
        if (MusicManager.Instance != null)
        {
            musicSlider.value = MusicManager.Instance.musicVolume;
            sfxSlider.value = MusicManager.Instance.sfxVolume;
        }
        else
        {
            musicSlider.value = 0.7f;
            sfxSlider.value = 1.0f;
        }

        controlTypeToggle.isOn = isUsingController;

        // Populate language dropdown from Unity Localization
        languageDropdown.ClearOptions();

        List<string> languageNames = new();

        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            languageNames.Add(locale.LocaleName);
        }

        languageDropdown.AddOptions(languageNames);

        languageDropdown.value =
            LocalizationSettings.AvailableLocales.Locales.IndexOf(
                LocalizationSettings.SelectedLocale);

        languageDropdown.RefreshShownValue();
    }

    private void OnControlTypeChanged(bool value)
    {
        isUsingController = value;
        PlayClickSound();

        // Call the visual update whenever the player clicks it!
        UpdateToggleVisuals(value);
    }

    // New method to update the text states dynamically
    private void UpdateToggleVisuals(bool toggleIsOn)
    {
        if (toggleIsOn)
        {
            // Controller is selected (Toggle is Checked)
            controllerText.color = activeColor;
            keyboardText.color = inactiveColor;

            // OPTIONAL: If you want to change font styles (like bolding the active one)
            controllerText.fontStyle = FontStyles.Bold;
            keyboardText.fontStyle = FontStyles.Normal;
        }
        else
        {
            // Keyboard/Mouse is selected (Toggle is Unchecked)
            controllerText.color = inactiveColor;
            keyboardText.color = activeColor;

            controllerText.fontStyle = FontStyles.Normal;
            keyboardText.fontStyle = FontStyles.Bold;
        }
    }

    // ==========================================
    // Keep the rest of your methods exactly the same...
    // ==========================================
    private void OnMusicVolumeChanged(float value) { if (MusicManager.Instance != null) MusicManager.Instance.SetMusicVolume(value); }
    private void OnSFXVolumeChanged(float value) { if (MusicManager.Instance != null) MusicManager.Instance.SetSFXVolume(value); }
    private void OnLanguageChanged(int index)
    {
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.Locales[index];
        Debug.Log("Language changed to: " +
     LocalizationSettings.SelectedLocale.LocaleName);

        PlayClickSound();
    }
    private void OnBackClicked() { if (titleScreenManager != null) titleScreenManager.OnBackPressedFromOptions(); }
    private void PlayClickSound() { if (MusicManager.Instance != null) MusicManager.Instance.PlaySFX(MusicManager.Instance.clickSound); }
}