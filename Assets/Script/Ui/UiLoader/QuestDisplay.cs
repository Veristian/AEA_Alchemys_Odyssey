using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
public class QuestDisplay : MonoBehaviour, IPointerDownHandler
{
    private PlayerQuestData questData;
    [SerializeField] private TextMeshProUGUI questName;

    public void Initialize(PlayerQuestData questData)
    {
        this.questData = questData;
        questName.text = questData.questData.questName;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (questData != null)
        {
            UiLoader.Instance.DisplayQuestData(questData);
        }
    }
    
}
