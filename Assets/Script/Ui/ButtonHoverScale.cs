using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float duration = 0.15f;

    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScale(originalScale * hoverScale);
        AudioController.Instance.PlaySFX("MouseHover");
        //Debug.Log("Hovering");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScale(originalScale);
        //Debug.Log("NotHovering");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioController.Instance.PlaySFX("MouseClick");
    }

    private void StartScale(Vector3 targetScale)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleRoutine(targetScale));
    }

    private IEnumerator ScaleRoutine(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            // Smooth easing
            t = t * t * (3f - 2f * t);

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        transform.localScale = targetScale;
    }


}
