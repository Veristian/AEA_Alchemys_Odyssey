using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using System.Linq;
[Serializable]

public class PotionInventoryData
{
    public int id;
    public PotionData potionData;
    public List<StoredData> ingredientsInside;
    public string additionalData;
}
[Serializable]

public class IngredientInventoryData
{
    public IngredientData ingredientData;
    public int amount;
}
[Serializable]
[Metadata("Potions")]
public class PotionInventoryList
{
    public List<PotionInventoryData> potionList;
}
[Serializable]
[Metadata("Ingredients")]
public class IngredientInventoryList
{
    public List<IngredientInventoryData> ingredientsList;
}
public class InventoryManager : Singleton<InventoryManager>
{
    private const string potionInventoryListFileName = "PlayerPotionData";
    private const string ingredientInventoryListFileName = "PlayerIngredientData";
    [SerializeField] private PotionInventoryList potionInventoryList;
    public PotionInventoryList PotionInventoryList
    {
        get { return potionInventoryList; }
    }
    [SerializeField] private IngredientInventoryList ingredientInventoryList;
    public IngredientInventoryList IngredientInventoryList
    {
        get { return ingredientInventoryList; }
    }

    //add and subtract from inventory
    public void AddPotionObject(PotionData potionData, List<StoredData> ingredientsInside, string additionalData = null)
    {
        if (potionInventoryList.potionList == null)
            potionInventoryList.potionList = new List<PotionInventoryData>();

        int newId = potionInventoryList.potionList.Count > 0
            ? potionInventoryList.potionList[^1].id + 1
            : 0;

        PotionInventoryData newPotion = new PotionInventoryData
        {
            id = newId,
            potionData = potionData,
            ingredientsInside = ingredientsInside,
            additionalData = additionalData
        };

        potionInventoryList.potionList.Add(newPotion);
    }
    public void ChangePotionObjectWithId(int id, PotionData potionData, List<StoredData> ingredientsInside, string additionalData = null)
    {
        var potion = potionInventoryList.potionList.Find(p => p.id == id);

        if (potion == null)
        {
            Debug.LogWarning($"Potion with ID {id} not found.");
            return;
        }

        potion.potionData = potionData;
        potion.ingredientsInside = ingredientsInside;
        potion.additionalData = additionalData;
    }
    
    public void RemovePotionObject(PotionData potionData, List<StoredData> ingredientsInside, string additionalData = null)
    {
        if (potionInventoryList?.potionList == null) return;

        var potion = potionInventoryList.potionList.Find(p =>
            p.potionData.potionId == potionData.potionId &&
            AreIngredientListsEqual(p.ingredientsInside, ingredientsInside) &&
            p.additionalData == additionalData
        );

        if (potion != null)
        {
            potionInventoryList.potionList.Remove(potion);
        }
    }
    public void RemovePotionObject(int id)
    {
        if (potionInventoryList?.potionList == null) return;

        int index = potionInventoryList.potionList.FindIndex(p => p.id == id);
        if (index >= 0)
        {
            potionInventoryList.potionList.RemoveAt(index);
        }
    }
    public void AddIngredient(IngredientData ingredientData, int amount = 1)
    {
        if (ingredientInventoryList.ingredientsList == null)
            ingredientInventoryList.ingredientsList = new List<IngredientInventoryData>();

        var existing = ingredientInventoryList.ingredientsList
            .Find(i => i.ingredientData == ingredientData);

        if (existing != null)
        {
            existing.amount += amount;
        }
        else
        {
            ingredientInventoryList.ingredientsList.Add(new IngredientInventoryData
            {
                ingredientData = ingredientData,
                amount = amount
            });
        }
    }

    public void SubtractIngredient(IngredientData ingredientData, int amount = 1)
    {
        if (ingredientInventoryList?.ingredientsList == null) return;

        var existing = ingredientInventoryList.ingredientsList
            .Find(i => i.ingredientData == ingredientData);

        if (existing == null) return;

        existing.amount -= amount;

        if (existing.amount <= 0)
        {
            ingredientInventoryList.ingredientsList.Remove(existing);
        }
    }
    private bool AreIngredientListsEqual(List<StoredData> a, List<StoredData> b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (!Equals(a[i], b[i]))
                return false;
        }

        return true;
    }

    public IngredientInventoryData PassIngredientReference(IngredientData ingredientData)
    {
        if (ingredientInventoryList?.ingredientsList == null) return null;

        return ingredientInventoryList.ingredientsList
            .Find(i => i.ingredientData == ingredientData);
    }

    //save inventory data to json path
    public void Save()
    {
        DataManager.Instance.SaveToFile(potionInventoryListFileName, potionInventoryList);
        DataManager.Instance.SaveToFile(ingredientInventoryListFileName, ingredientInventoryList);

        Debug.Log("Inventory Saved");
    }

    //get inventory data from json path and return new data set if null
    public void Load()
    {

        potionInventoryList = DataManager.Instance.LoadFromFile(
            potionInventoryListFileName,
            () => new PotionInventoryList { potionList = new List<PotionInventoryData>() }
        );

        ingredientInventoryList = DataManager.Instance.LoadFromFile(
            ingredientInventoryListFileName,
            () => new IngredientInventoryList { ingredientsList = new List<IngredientInventoryData>() }
        );

        Debug.Log("Inventory Loaded");
        
        //fills out definitin for all ingredients for faster reload time
        // foreach (IngredientData ingredientData in DataManager.Instance.ingredientDatas)
        // {
            
        // }
        var existingIds = new HashSet<string>(
            ingredientInventoryList.ingredientsList
                .Select(inv => inv.ingredientData.ingredientId)
        );

        var newEntries = DataManager.Instance.ingredientDatas
            .Where(data => !existingIds.Contains(data.ingredientId))
            .Select(data => new IngredientInventoryData
            {
                ingredientData = data,
                amount = 0
            });

        ingredientInventoryList.ingredientsList.AddRange(newEntries);
    }


}
