using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransitionManager : MonoBehaviour
{
    public static UITransitionManager Instance { get; private set; }

    [SerializeField] private float defaultDuration = 0.25f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void FadeIn(GameObject target)
    {
        StartCoroutine(Fade(target, 0f, 1f, defaultDuration, true));
    }

    public void FadeOut(GameObject target, System.Action onComplete)
    {
        StartCoroutine(Fade(target, 1f, 0f, defaultDuration, false, onComplete));
    }

    public void FadeOut(GameObject target)
    {
        StartCoroutine(Fade(target, 1f, 0f, defaultDuration, false));
    }

    public void FadeIn(GameObject target, float duration)
    {
        StartCoroutine(Fade(target, 0f, 1f, duration, true));
    }

    public void FadeOut(GameObject target, float duration)
    {
        StartCoroutine(Fade(target, 1f, 0f, duration, false));
    }

    private IEnumerator Fade(GameObject target, float startAlpha, float endAlpha, float duration, bool showBeforeFade, System.Action onComplete = null)
    {
        if (target == null) yield break;

        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = target.AddComponent<CanvasGroup>();

        if (showBeforeFade)
            target.SetActive(true);

        canvasGroup.alpha = startAlpha;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (endAlpha <= 0f)
            target.SetActive(false);

        onComplete?.Invoke();
    }
}
