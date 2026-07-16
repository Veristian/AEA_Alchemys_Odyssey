using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : Singleton<SceneLoadingManager>
{
    [Header("Loading UI")]
    [SerializeField] public GameObject loadingScreen;

    [Header("Settings")]
    [SerializeField] private float minimumLoadingTime = 1f;
    private bool isLoading = false;

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
        if (isLoading)
            return;

        isLoading = true;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        InputManager.Instance.DisableInputs();
        UITransitionManager.Instance.FadeIn(loadingScreen);
        AudioController.Instance.FadeOutCurrentBGM();
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
        InputManager.Instance.EnableInputs();
    }


    private IEnumerator LoadingSetupDelayRoutine()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
            //InputManager.Instance.canMove = false;
            //InputManager.Instance.canLook = false;
            InputManager.Instance.DisableInputs();
            yield return new WaitForSeconds(1.5f); // Wait 1 frame
            float fadeduration = 0.5f;
            UITransitionManager.Instance.FadeOut(loadingScreen,fadeduration);
            yield return new WaitForSeconds(.5f);
            InputManager.Instance.EnableInputs();
            //InputManager.Instance.canMove = true;
            //InputManager.Instance.canLook = true;
        }
    }

    private IEnumerator TeleportRoutine(Vector3 destination)
    {
        bool faded = false;
        UITransitionManager.Instance.FadeIn(loadingScreen, 0.25f, () =>
        {
            faded = true;
        });

        yield return new WaitUntil(() => faded);
        GameObject player = GameObject.FindAnyObjectByType<PlayerController>(FindObjectsInactive.Exclude).gameObject;
        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = destination;
        }
        else
        {
            Debug.LogError(" Could not find Player with tag 'Player'");
        }
        yield return new WaitForSeconds(0.25f);

        UITransitionManager.Instance.FadeOut(loadingScreen, 0.25f);
    }

    public void Teleport(Vector3 destination)
    {
        StartCoroutine(TeleportRoutine(destination));
    }

    public void Teleport(string destination)
    {
        string[] parts = destination.Split(',');

        if (parts.Length == 3)
        {
            Vector3 position = new Vector3(
            float.Parse(parts[0]),
            float.Parse(parts[1]),
            float.Parse(parts[2])
            );

            Teleport(position);
        }

        if (parts.Length == 1)
        {
            var point = GameObject.Find(parts[0]);
            if (point == null)
            {
                Debug.LogError(parts[0] +" object not found");
                return;
            }
            Teleport(point.transform.position);
        }

    }

}
