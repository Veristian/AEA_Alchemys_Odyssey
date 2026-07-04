using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotionHouse : Singleton<PotionHouse>
{
    [Header("Reference")]
    public Transform contentTransform;

    public GameObject potionHouseItemPrefab;
    public List<PotionHouseItem> potionHouseItems;

    private void OnEnable()
    {
        var manager = DataManager.Instance;
        manager.OnGameLoaded += SetupPotions;

        if (manager.IsGameLoaded)
            SetupPotions(); 
    }
    private void OnDisable()
    {
        if (DataManager.Instance != null)
            DataManager.Instance.OnGameLoaded -= SetupPotions;
    }

    private void SetupPotions()
    {
        if (contentTransform == null)
        {
            Debug.LogWarning("Content Transform reference is not assigned. Please assign a Transform reference to contentTransform in the inspector.");
            return;
        }
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        potionHouseItems.Clear();
        foreach (PotionInventoryData potionData in InventoryManager.Instance.PotionInventoryList.potionList)
        {
            PotionHouseItem item = Instantiate(potionHouseItemPrefab, contentTransform).GetComponent<PotionHouseItem>();
            potionHouseItems.Add(item);
            item.AssignPotion(InventoryManager.Instance.PassPotionReference(potionData));
        }
    }
    //note to self: check if this potion can remove properly
    // public void UpdatePotionHousesObjTaken(List<(PotionData, List<StoredData>)> potionToRemove)
    // {
    //     if (potionToRemove == null || potionToRemove.Count == 0) return;
    //     var groupedToRemove = potionToRemove
    //         .GroupBy(i => i)
    //         .ToDictionary(g => g.Key, g => g.Count());

    //     foreach (var houseItem in potionHouseItems)
    //     {
    //         PotionData potionData = houseItem.Potion.potionData;

    //         if (groupedToRemove.TryGetValue(potionData, out int removeCount))
    //         {
    //             houseItem.objTaken += removeCount;
    //         }
    //     }
    // }
    public void UpdatePotionHousesObjTaken(List<(PotionData, List<StoredData>)> potionToRemove)
    {
        if (potionToRemove == null || potionToRemove.Count == 0) return;

        var groupedToRemove = potionToRemove
            .GroupBy(i => (i.Item1, key: GetStoredDataKey(i.Item2)))
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var houseItem in potionHouseItems)
        {
            var potionData = houseItem.Potion.potionData;

            var houseKey = GetStoredDataKey(houseItem.Potion.ingredientsInside);

            var lookupKey = (potionData, houseKey);

            if (groupedToRemove.TryGetValue(lookupKey, out int removeCount))
            {
                houseItem.objTaken = true;
            }
        }
    }
    string GetStoredDataKey(List<StoredData> list)
    {
        return string.Join("|",
            list
            .GroupBy(s => s.ingredientData)
            .OrderBy(g => g.Key.name)
            .Select(g =>
            {
                // include all contactPoints, sorted
                var contacts = g
                    .Select(x => x.contactPoint)
                    .OrderBy(x => x);

                return $"{g.Key.name}:[{string.Join(",", contacts)}]";
            })
        );
    }


    public void UpdatePotionDisplay()
    {
        SetupPotions();
    }

}
