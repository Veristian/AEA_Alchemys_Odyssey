using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class JournalDisplay : MonoBehaviour, IPointerDownHandler
{
    private IngredientData ingredientData;
    private PotionData potionData;
    public Image itemImage;

    public void InitIngredientDisplay(IngredientData ingredientData)
    {
        this.ingredientData = ingredientData;
        itemImage.sprite = ingredientData.ingredientSprite;
    }

    public void InitPotionDisplay(PotionData potionData)
    {
        this.potionData = potionData;
        itemImage.sprite = potionData.potionSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (ingredientData != null)
        {
            // UiLoader.Instance.CallOpenIndexEvent();
            UiLoader.Instance.DisplayIngredientData(ingredientData);
        }
        else if (potionData != null)
        {
            // UiLoader.Instance.CallOpenIndexEvent();
            UiLoader.Instance.DisplayPotionData(potionData);
        }
    }

    


}
