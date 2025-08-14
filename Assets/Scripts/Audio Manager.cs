using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private int sfxPoolSize = 10;
    private List<AudioSource> sfxPool;

    [Header("Music Clips (scene-based)")]
    public AudioClip menuMusic;
    public AudioClip story1Music;
    public AudioClip story2Music;
    public AudioClip man1Music;
    public AudioClip man2Music;
    public AudioClip man3Music;

    [Header("Ending Music")]
    public AudioClip ending1Music;
    public AudioClip ending2Music;

    [Header("Player SFX (public so old code still compiles)")]
    public AudioClip punch;
    public AudioClip kick;
    public AudioClip hit;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip dash;
    public AudioClip walk;
    public AudioClip attackCombo;
    public AudioClip hurt;
    public AudioClip death;

    [Header("Boss SFX (public)")]
    public AudioClip bossAttack;
    public AudioClip bossSpecialAttack;
    public AudioClip bossHit;
    public AudioClip bossRoar;
    public AudioClip bossPhaseChange;
    public AudioClip bossDeath;

    [Header("Boss1 Skills")]
    public AudioClip boss1Dash;
    public AudioClip boss1Slash1;
    public AudioClip boss1Slash2;
    public AudioClip boss1Slash3;

    [Header("Boss2 Skills")]
    public AudioClip boss2Slash;

    [Header("UI sliders (optional)")]
    public Slider musicSlider;
    public Slider sfxSlider;

    // internal
    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitAudio();
            BuildSfxDictionary();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // If other AudioManager exists in scenes, destroy this one
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitAudio()
    {
        // music source
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // SFX pool
        sfxPool = new List<AudioSource>(sfxPoolSize);
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject go = new GameObject("SFXSource_" + i);
            go.transform.SetParent(transform);
            AudioSource a = go.AddComponent<AudioSource>();
            a.playOnAwake = false;
            a.spatialBlend = 0f; // 2D by default
            a.ignoreListenerPause = true;
            sfxPool.Add(a);
        }

        // load saved volumes
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSource != null) musicSource.volume = musicVolume;
        foreach (var s in sfxPool) s.volume = sfxVolume;

        // setup sliders if provided
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.value = musicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    private void BuildSfxDictionary()
    {
        // add all public AudioClip fields to dictionary so you can call by name
        AddToDict("punch", punch);
        AddToDict("kick", kick);
        AddToDict("hit", hit);
        AddToDict("jump", jump);
        AddToDict("land", land);
        AddToDict("dash", dash);
        AddToDict("walk", walk);
        AddToDict("attackCombo", attackCombo);
        AddToDict("hurt", hurt);
        AddToDict("death", death);

        AddToDict("bossAttack", bossAttack);
        AddToDict("bossSpecialAttack", bossSpecialAttack);
        AddToDict("bossHit", bossHit);
        AddToDict("bossRoar", bossRoar);
        AddToDict("bossPhaseChange", bossPhaseChange);
        AddToDict("bossDeath", bossDeath);

        AddToDict("boss1Dash", boss1Dash);
        AddToDict("boss1Slash1", boss1Slash1);
        AddToDict("boss1Slash2", boss1Slash2);
        AddToDict("boss1Slash3", boss1Slash3);

        AddToDict("boss2Slash", boss2Slash);
    }

    private void AddToDict(string key, AudioClip clip)
    {
        if (clip != null && !sfxDict.ContainsKey(key))
            sfxDict.Add(key, clip);
    }

    // --------------- Music (scene) ---------------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);

        // --- Music Slider ---
        var foundMusicSlider = GameObject.FindWithTag("MusicSlider")?.GetComponent<Slider>();
        if (foundMusicSlider != null)
        {
            musicSlider = foundMusicSlider;
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        // --- SFX Slider ---
        var foundSFXSlider = GameObject.FindWithTag("SFXSlider")?.GetComponent<Slider>();
        if (foundSFXSlider != null)
        {
            sfxSlider = foundSFXSlider;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    public void PlayMusicForScene(string sceneName)
    {
        AudioClip toPlay = sceneName switch
        {
            "UI" => menuMusic,
            "Cottruyen1" => story1Music,
            "Cottruyen2" => story2Music,
            "man1" => man1Music,
            "man2" => man2Music,
            "man3" => man3Music,
            "Ending1" => ending1Music,
            "Ending2" => ending2Music,
            _ => menuMusic
        };

        if (toPlay != null && musicSource.clip != toPlay)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeMusicSmoothly(toPlay, 0.8f));
        }
    }


    private IEnumerator ChangeMusicSmoothly(AudioClip newClip, float fadeTime)
    {
        float startVol = musicSource.volume;
        // fade out
        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();

        // fade in
        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / fadeTime);
            yield return null;
        }
        musicSource.volume = musicVolume;
    }

    // --------------- SFX playback (zero-delay) ---------------
    // Play by AudioClip (existing code uses this style: PlaySFX(AudioManager.Instance.walk))
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        // find free source
        for (int i = 0; i < sfxPool.Count; i++)
        {
            var src = sfxPool[i];
            if (!src.isPlaying)
            {
                src.PlayOneShot(clip, sfxVolume);
                return;
            }
        }

        // if all busy, play on first (overwrite)
        sfxPool[0].PlayOneShot(clip, sfxVolume);
    }

    // Play by name (newer style): PlaySFX("jump")
    public void PlaySFX(string clipName)
    {
        if (string.IsNullOrEmpty(clipName)) return;
        if (sfxDict.TryGetValue(clipName, out AudioClip clip))
            PlaySFX(clip);
        else
            Debug.LogWarning($"[AudioManager] SFX '{clipName}' not found in dictionary.");
    }

    // convenience small wrappers for animator events or quick calls
    public void PlayPunch() => PlaySFX(punch);
    public void PlayBossAttack() => PlaySFX(bossAttack);
    public void PlayBossDeath() => PlaySFX(bossDeath);
    public void PlayWalkLoop()
    {
        // optional: a simple continuous footstep implementation (keeps tasks simple)
        // We'll use the first SFX source as looping footstep if needed
        var src = sfxPool[0];
        if (walk == null) return;
        if (src.clip == walk && src.isPlaying) return;
        src.clip = walk;
        src.loop = true;
        src.Play();
    }
    public void StopWalkLoop()
    {
        var src = sfxPool[0];
        if (src.clip == walk)
        {
            src.Stop();
            src.clip = null;
            src.loop = false;
        }
    }

    // --------------- Volume -----------------
    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        if (musicSource != null) musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        foreach (var s in sfxPool)
            s.volume = sfxVolume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }


    // helper to allow UI to initialize sliders (call from Start of a Settings script if needed)
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;


}