using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
[Serializable]

public class PotionInventoryData
{
    public int id;
    public PotionData potionData; // problem
    [ReadOnly] public string potionDataId;
    public List<StoredData> ingredientsInside;
    public string additionalData;
    public void OnBeforeSerialize()
    {
        if (potionData != null)
            potionDataId = potionData.potionId;
    }
    public void OnAfterDeserialize()
    {
        if (!string.IsNullOrEmpty(potionDataId))
        {
            potionData = DataManager.Instance.potionDatas
                .Find(data => data.potionId == potionDataId);
        }
    }
}
[Serializable]

public class IngredientInventoryData
{
    public IngredientData ingredientData; 
    [ReadOnly] public string ingredientDataId;
    public int amount;

    public void OnBeforeSerialize()
    {
        if (ingredientData != null)
            ingredientDataId = ingredientData.ingredientId;
    }
    public void OnAfterDeserialize()
    {
        if (!string.IsNullOrEmpty(ingredientDataId))
        {
            ingredientData = DataManager.Instance.ingredientDatas
                .Find(data => data.ingredientId == ingredientDataId);
        }
    }
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
    protected override void Awake()
    {
        transform.parent = null;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        base.Awake();
        DontDestroyOnLoad(gameObject);
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
            AreIngredientExactListsEqual(p.ingredientsInside, ingredientsInside) &&
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

        PickupPopupManager.Instance?.ShowPickup(ingredientData);
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
    private bool AreIngredientExactListsEqual(List<StoredData> a, List<StoredData> b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;
        a = a.OrderBy(i => i.ingredientData.ingredientId).ToList();
        b = b.OrderBy(i => i.ingredientData.ingredientId).ToList();
        for (int i = 0; i < a.Count; i++)
        {
            if (!Equals(a[i], b[i]))
                return false;
        }

        return true;
    }
    private bool AreIngredientIDListsEqual(List<StoredData> a, List<StoredData> b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;
        a = a.OrderBy(i => i.ingredientData.ingredientId).ToList();
        b = b.OrderBy(i => i.ingredientData.ingredientId).ToList();
        for (int i = 0; i < a.Count; i++)
        {
            if (!Equals(a[i].ingredientData.ingredientId, b[i].ingredientData.ingredientId))
                return false;
        }

        return true;
    }

    private bool CompareIngredients(List<StoredData> a, List<StoredData> b, bool exact)
    {
        if (exact)
            return AreIngredientExactListsEqual(a, b);

        return AreIngredientIDListsEqual(a, b);
    }

    public (bool exist, int id) CheckPotionExists(
        PotionData potionData,
        List<StoredData> ingredientsInside,
        bool ignoreAdditionalData = true,
        string additionalData = null,
        bool ignoreIngredientList = true,
        bool matchIngredientsExactly = false)
    {
        if (potionInventoryList?.potionList == null) return (false, -1);

        var potion = potionInventoryList.potionList.Find(p =>
            p.potionData.potionId == potionData.potionId &&
            (ignoreIngredientList || CompareIngredients(p.ingredientsInside, ingredientsInside, matchIngredientsExactly)) &&
            (ignoreAdditionalData || p.additionalData == additionalData)
        );

        if (potion == null) return (false, -1);

        return (true, potion.id);

    }

    public bool CheckPotionExistsAndRemove(
        PotionData potionData,
        List<StoredData> ingredientsInside,
        bool ignoreAdditionalData = true,
        string additionalData = null,
        bool ignoreIngredientList = true,
        bool matchIngredientsExactly = false)
    {
        if (potionInventoryList?.potionList == null) return false;

        var potion = potionInventoryList.potionList.Find(p =>
            p.potionData.potionId == potionData.potionId &&
            (ignoreIngredientList || CompareIngredients(p.ingredientsInside, ingredientsInside, matchIngredientsExactly)) &&
            (ignoreAdditionalData || p.additionalData == additionalData)
        );

        if (potion == null) return false;

        RemovePotionObject(potion.id);
        
        return true;

    }

    // public int GetIngredientCount(string ingredientId)
    // {
    //     if (ingredientInventoryList?.ingredientsList == null) return 0;

    //     var ingredient = ingredientInventoryList.ingredientsList
    //         .Find(i => i.ingredientData.ingredientId == ingredientId);

    //     return ingredient != null ? ingredient.amount : 0;
    // }

    public IngredientInventoryData PassIngredientReference(IngredientData ingredientData)
    {
        if (ingredientInventoryList?.ingredientsList == null) return null;

        return ingredientInventoryList.ingredientsList
            .Find(i => i.ingredientData == ingredientData);
    }
    public PotionInventoryData PassPotionReference(PotionInventoryData potionInventoryData)
    {
        if (potionInventoryList?.potionList == null) return null;

        return potionInventoryList.potionList
            .Find(i => i.potionData == potionInventoryData.potionData && i.ingredientsInside == potionInventoryData.ingredientsInside);
    }
    //note to self: check this func

    //save inventory data to json path
    public void Save()
    {
        potionInventoryList.potionList.ForEach(p => p.OnBeforeSerialize());
        ingredientInventoryList.ingredientsList.ForEach(i => i.OnBeforeSerialize());
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

        potionInventoryList.potionList.ForEach(p => p.OnAfterDeserialize());
        ingredientInventoryList.ingredientsList.ForEach(i => i.OnAfterDeserialize());
        potionInventoryList.potionList.RemoveAll(i => i.potionData == null);
        ingredientInventoryList.ingredientsList.RemoveAll(i => i.ingredientData == null);
        Debug.Log("Inventory Loaded");
        
        //fills out definitin for all ingredients for faster reload time
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
