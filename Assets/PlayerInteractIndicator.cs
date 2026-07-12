using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update

    public void interactableIndicatorChecker(string text)
    {
        if (text == null)
        {
            UITransitionManager.Instance.FadeOut(IiPanel);
        }
        else
        {
            UITransitionManager.Instance.FadeIn(IiPanel);
        }
    }
}
