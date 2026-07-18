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

    private SceneMusic music;

    private AudioSource currentSource;
    private AudioSource nextSource;
    private bool usingDualBGM = false;
    private bool usingSecondArea = false;

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
        nextSource = bgmSourceB;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        music = FindFirstObjectByType<SceneMusic>();

        if (music != null)
        {
            if (music.bgm02 != null)
            {
                PlayBothBGM();
            }
            else
            {
                PlayBGM(music.bgm01);
            }
            
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

    public void BGMToogle()
    {
        //if (music.bgm02 == null)
        //{
        //    return;
        //}
        //if (currentSource.clip == music.bgm01 && currentSource.isPlaying)
        //{
        //    if (fadeRoutine != null)
        //    {
        //        StopCoroutine(fadeRoutine);
        //    }
        //    fadeRoutine = StartCoroutine(CrossFade(music.bgm02));
        //}
        //else
        //{
        //    if (fadeRoutine != null)
        //    {
        //        StopCoroutine(fadeRoutine);
        //    }
        //    fadeRoutine = StartCoroutine(CrossFade(music.bgm01));
        //}

        if (!usingDualBGM)
            return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeBetweenSources());

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
        //StopCoroutine(fadeRoutine);
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

    public void PlayBothBGM()
    {
        usingDualBGM = true;

        PlayBGM(music.bgm01);

        bgmSourceB.clip = music.bgm02;
        bgmSourceB.loop = true;
        bgmSourceB.volume = 0f;
        bgmSourceB.Play();

        currentSource = bgmSourceA;
        nextSource = bgmSourceB;
    }

    public void SwitchArea(bool secondArea)
    {
        if (!usingDualBGM)
            return;

        if (usingSecondArea == secondArea)
            return;

        usingSecondArea = secondArea;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeTo(secondArea));
    }

    IEnumerator FadeTo(bool secondArea)
    {
        AudioSource fadeOut = secondArea ? bgmSourceA : bgmSourceB;
        AudioSource fadeIn = secondArea ? bgmSourceB : bgmSourceA;

        float outStart = fadeOut.volume;
        float inStart = fadeIn.volume;

        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float percent = t / fadeDuration;

            fadeOut.volume = Mathf.Lerp(outStart, 0, percent);
            fadeIn.volume = Mathf.Lerp(inStart, 1, percent);

            yield return null;
        }

        fadeOut.volume = 0;
        fadeIn.volume = 1;

        currentSource = fadeIn;
        nextSource = fadeOut;

        fadeRoutine = null;
    }

    //IEnumerator CrossFade(AudioClip clip)
    //{
    //    nextSource.clip = clip;
    //    nextSource.loop = true;
    //    nextSource.volume = 0f;
    //    nextSource.Play();

    //    float time = 0;

    //    while (time < fadeDuration)
    //    {
    //        time += Time.deltaTime;

    //        float t = time / fadeDuration;

    //        currentSource.volume = Mathf.Lerp(1, 0, t);
    //        nextSource.volume = Mathf.Lerp(0, 1, t);

    //        yield return null;
    //    }

    //    currentSource.Stop();

    //    AudioSource temp = currentSource;
    //    currentSource = nextSource;
    //    nextSource = temp;

    //    currentSource.volume = 1f;
    //    nextSource.volume = 0f;
    //}

    IEnumerator FadeBetweenSources()
    {
        float currentStart = currentSource.volume;
        float nextStart = nextSource.volume;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float t = time / fadeDuration;

            currentSource.volume = Mathf.Lerp(currentStart, 0f, t);
            nextSource.volume = Mathf.Lerp(nextStart, 1f, t);

            yield return null;
        }

        currentSource.volume = 0f;
        nextSource.volume = 1f;

        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;

        fadeRoutine = null;
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