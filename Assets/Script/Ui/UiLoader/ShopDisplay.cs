using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ShopDisplay : MonoBehaviour, IPointerDownHandler
{   
    [SerializeField] private TextMeshProUGUI shopItemName;
    [SerializeField] private TextMeshProUGUI shopItemDescription;
    [SerializeField] private TextMeshProUGUI shopItemPrice;
    [SerializeField] private Image shopItemImage;
    private ShopItemData shopItemData;
    public void InitShopDisplay(ShopItemData shopItemData)
    {
        this.shopItemData = shopItemData;
        if (shopItemName)
            shopItemName.text = shopItemData.itemName;
        if (shopItemDescription)
            shopItemDescription.text = shopItemData.description;
        if (shopItemPrice)
            shopItemPrice.text = shopItemData.price.ToString();
        if (shopItemImage)
            shopItemImage.sprite = shopItemData.itemSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // UiLoader.Instance.CallOpenShopEvent();
            UiLoader.Instance.DisplayShopItemData(shopItemData);
        }
    }
}
