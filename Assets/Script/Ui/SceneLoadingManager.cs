using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : Singleton<SceneLoadingManager>
{
    [Header("Loading UI")]
    [SerializeField] private GameObject loadingScreen;

    [Header("Settings")]
    [SerializeField] private float minimumLoadingTime = 1f;

    private void OnEnable()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
            StartCoroutine(LoadingSetupDelayRoutine());
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        UITransitionManager.Instance.FadeIn(loadingScreen);
        //loadingScreen.SetActive(true);
        yield return new WaitForSeconds(.3f);
        float timer = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            if (operation.progress >= 0.9f && timer >= minimumLoadingTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }


    private IEnumerator LoadingSetupDelayRoutine()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);

            yield return new WaitForSeconds(0.2f); // Wait 1 frame
            float fadeduration = 0.5f;
            UITransitionManager.Instance.FadeOut(loadingScreen,fadeduration);
        }
    }

}
