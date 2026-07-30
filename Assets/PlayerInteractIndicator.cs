using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractIndicator : Singleton<PlayerInteractIndicator>
{
    [System.Serializable]
    public class InteractIndicator
    {
        public string name;
        public Sprite image;
    }

    [Header("Interactable Indicator")]
    [SerializeField] private GameObject IiPanel;
    [SerializeField] private InteractIndicator[] iIndicators;
    [SerializeField] private Image indicatorImage;
    private bool isShowing = false;
    // Start is called before the first frame update

    private void Start()
    {
        if (IiPanel != null)
        {
            IiPanel.SetActive(false);
        }
    }

    public void interactableIndicatorChecker(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            if (!isShowing) return;
            //Debug.Log("CallHide");
            isShowing = false;
            UITransitionManager.Instance.FadeOut(IiPanel);
            return;
        }

        InteractIndicator indicator = Array.Find(iIndicators, x => x.name == text);

        if (indicator != null)
        {
            indicatorImage.sprite = indicator.image;
            if (isShowing) return;
            //Debug.Log("CallShow");
            isShowing = true;
            UITransitionManager.Instance.FadeIn(IiPanel);
        }
        else
        {
            Debug.LogWarning($"No indicator found for '{text}'");
            if (!isShowing) return;
            //Debug.Log("CallHide");
            isShowing= false;
            UITransitionManager.Instance.FadeOut(IiPanel);
        }
    }
}
