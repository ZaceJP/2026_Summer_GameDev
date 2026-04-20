using UnityEngine;
using System.Collections;

public enum MusicType
{
    None,
    Title,
    Map,
    Rest,
    Shop,
    Battle,
    EliteBattle,
    BossBattle
}

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSourceA;
    public AudioSource musicSourceB;
    public AudioSource sfxSource; //  new SFX source

    [Header("Music Tracks")]
    public AudioClip titleMusic;
    public AudioClip mapMusic;
    public AudioClip restMusic;
    public AudioClip shopMusic;
    public AudioClip battleMusic;
    public AudioClip eliteBattleMusic;
    public AudioClip bossBattleMusic;

    [Header("UI & General SFX")]
    public AudioClip clickSound;
    public AudioClip confirmSound;
    public AudioClip cancelSound;
    public AudioClip nextPageSound;
    public AudioClip failSound; // e.g. "not enough gold"
    public AudioClip drawCardSound;
    public AudioClip discardCardSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public float fadeDuration = 2f;

    private AudioSource activeSource;
    private MusicType currentMusic = MusicType.None;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        activeSource = musicSourceA;

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
    }

    public void PlayMusic(MusicType type)
    {
        if (currentMusic == type || type == MusicType.None)
            return;

        currentMusic = type;
        AudioClip nextClip = GetClipForType(type);

        if (nextClip == null)
        {
            Debug.LogWarning("No clip assigned for " + type);
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewTrack(nextClip));
    }

    private IEnumerator FadeToNewTrack(AudioClip newClip)
    {
        AudioSource newSource = (activeSource == musicSourceA) ? musicSourceB : musicSourceA;
        newSource.clip = newClip;
        newSource.volume = 0f;
        newSource.loop = true;
        newSource.Play();

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float normalized = t / fadeDuration;
            activeSource.volume = Mathf.Lerp(musicVolume, 0f, normalized);
            newSource.volume = Mathf.Lerp(0f, musicVolume, normalized);
            yield return null;
        }

        activeSource.Stop();
        activeSource = newSource;
    }

    private AudioClip GetClipForType(MusicType type)
    {
        switch (type)
        {
            case MusicType.Title: return titleMusic;
            case MusicType.Map: return mapMusic;
            case MusicType.Rest: return restMusic;
            case MusicType.Shop: return shopMusic;
            case MusicType.Battle: return battleMusic;
            case MusicType.EliteBattle: return eliteBattleMusic;
            case MusicType.BossBattle: return bossBattleMusic;
            default: return null;
        }
    }

    public void SetVolume(float value)
    {
        musicSourceA.volume = value;
        musicSourceB.volume = value;
    }

    //  New helper for playing SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}
