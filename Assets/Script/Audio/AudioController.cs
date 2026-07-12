using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [Header("SFX_Clip")]
    [SerializeField] private Sound[] SfxClip; 

    [Header("Mixer")]
    public AudioMixer mixer;

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSourceA;
    [SerializeField] private AudioSource bgmSourceB;
    [SerializeField] private AudioSource sfxSource;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1.5f;

    private AudioSource currentSource;
    private AudioSource nextSource;

    Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        currentSource = bgmSourceA;
        //nextSource = bgmSourceB;
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        SceneMusic music = FindFirstObjectByType<SceneMusic>();

        if (music != null)
        {
            PlayBGM(music.bgm);
        }
        else
        {
            Debug.LogWarning("Pls Add MusicSource for the Scene");
        }
    }
    private void Start()
    {
        VolumeSetup();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(SfxClip, x => x.sname == name);
        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip);
        }
        else
        {
            Debug.Log("SfxClip Not Found");
        }

    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
            return;

        if (currentSource.clip == clip)
            return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeIn(clip));
    }

    

    IEnumerator FadeIn(AudioClip clip)
    {
        currentSource.clip = clip;
        currentSource.loop = true;
        currentSource.volume = 0f;
        currentSource.Play();

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float t = time / fadeDuration;

            //currentSource.volume = Mathf.Lerp(1, 0, t);
            currentSource.volume = Mathf.Lerp(0, 1, t);

            yield return null;
        }
        currentSource.volume = 1f;
    }

    public void FadeOutCurrentBGM()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutBGM());
    }


    IEnumerator FadeOutBGM()
    {
        StopCoroutine(fadeRoutine);
        float startVolume = currentSource.volume;
        //float nstartVolume = nextSource.volume;
        float time = 0;
        while (time < 1.5f)
        {
            time += Time.deltaTime;
            float t = time / 1.5f;
            currentSource.volume = Mathf.Lerp(startVolume, 0, t);
            //nextSource.volume = Mathf.Lerp(nstartVolume, 0, t);
            yield return null;
        }
        currentSource.Stop();
        currentSource.volume = 0f;

        fadeRoutine = null;
    }

    IEnumerator CrossFade(AudioClip clip)
    {
        nextSource.clip = clip;
        nextSource.loop = true;
        nextSource.volume = 0f;
        nextSource.Play();

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float t = time / fadeDuration;

            currentSource.volume = Mathf.Lerp(1, 0, t);
            nextSource.volume = Mathf.Lerp(0, 1, t);

            yield return null;
        }

        currentSource.Stop();

        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;

        currentSource.volume = 1f;
        nextSource.volume = 0f;
    }

    //public void SetMasterVolume(float value)
    //{
    //    mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    //}

    //public void SetBGMVolume(float value)
    //{
    //    mixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
    //}

    //public void SetSFXVolume(float value)
    //{
    //    mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    //}

    private void VolumeSetup()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float bgm = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMasterVolume(master);
        SetBGMVolume(bgm);
        SetSFXVolume(sfx);
    }
    public void SetMasterVolume(float value)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetBGMVolume(float value)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}