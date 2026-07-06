using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItemData
{
    public string itemName;
    public string description;
    public Sprite itemSprite;
    public int price;
    public string itemGroupId;
    public bool IsUnlocked =>
        PlayerDataManager.Instance.IsUnlocked(itemGroupId);
}
public class ShopManager : Singleton<ShopManager>
{
    public List<ShopItemData> shopItems;
}
