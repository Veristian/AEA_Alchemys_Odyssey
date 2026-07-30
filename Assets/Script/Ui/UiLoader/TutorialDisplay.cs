using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
public class TutorialDisplay : MonoBehaviour, IPointerDownHandler
{
    private string tutorialId;
    [SerializeField] private TextMeshProUGUI tutorialName;

    public void Initialize(TutorialData tutorialData)
    {
        tutorialId = tutorialData.tutorialId;
        tutorialName.text = tutorialData.tutorialName == "" ? tutorialData.name : tutorialData.tutorialName;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tutorialId != "")
        {
            UiLoader.Instance.OpenTutorial(tutorialId);
        }
    }

}
