using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InventoryDisplay : MonoBehaviour
{
    private IngredientInventoryData ingredientInventoryData;
    private PotionInventoryData potionInventoryData;

    public Image itemImage;
    public TextMeshProUGUI itemAmountText;

    public void InitIngredientDisplay(IngredientInventoryData ingredientInventoryData)
    {
        this.ingredientInventoryData = ingredientInventoryData;
        itemImage.sprite = ingredientInventoryData.ingredientData.ingredientSprite;
        itemAmountText.text = ingredientInventoryData.amount.ToString();
    }

    public void InitPotionDisplay(PotionInventoryData potionInventoryData)
    {
        this.potionInventoryData = potionInventoryData;
        itemImage.sprite = potionInventoryData.potionData.potionSprite;
        itemAmountText.text = "1"; // no amounts
    }

    private void OnEnable()
    {
        UpdateAmountText();
    }

    private void UpdateAmountText()
    {
        if (ingredientInventoryData != null)
        {
            itemAmountText.text = ingredientInventoryData.amount.ToString();
        }
    }


}
