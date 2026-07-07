using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DailyTaskDisplay : MonoBehaviour
{
    [SerializeField] private Image requestingCharacterImage;
    [SerializeField] private TextMeshProUGUI requestingCharacterName;
    [SerializeField] private TextMeshProUGUI requestDescription;

    public void InitDisplay(LocalRequest localRequest)
    {
        if (localRequest == null) return;
        requestingCharacterImage.sprite = localRequest.requestImage;
        requestingCharacterName.text = localRequest.requestText;
        requestDescription.text = localRequest.requestDescription;
    }
}
