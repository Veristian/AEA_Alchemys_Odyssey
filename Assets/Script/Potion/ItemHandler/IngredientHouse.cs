using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientHouse : Singleton<IngredientHouse>
{
    [Header("Reference")]
    public Transform contentTransform;

    public GameObject ingredientHouseItemPrefab;
    public List<IngredientHouseItem> ingredientHouseItems;

    private void OnEnable()
    {
        var manager = DataManager.Instance;
        manager.OnGameLoaded += SetupIngredients;

        if (manager.IsGameLoaded)
            SetupIngredients(); 
    }
    private void OnDisable()
    {
        if (DataManager.Instance != null)
            DataManager.Instance.OnGameLoaded -= SetupIngredients;
    }

    public void SetupIngredients()
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
        ingredientHouseItems.Clear();
        foreach (IngredientData ingredientData in DataManager.Instance.ingredientDatas)
        {
            if (ingredientData == null)
            {
                Debug.LogWarning("IngredientData is null. Skipping this entry.");
                continue;
            }
            IngredientInventoryData ingredient = InventoryManager.Instance.PassIngredientReference(ingredientData);
            if (ingredient == null)
            {
                Debug.LogWarning($"IngredientInventoryData for {ingredientData.ingredientName} is null. Skipping this entry.");
                continue;
            }
            if (ingredient.amount <= 0)
            {
                continue;
            }
            IngredientHouseItem item = Instantiate(ingredientHouseItemPrefab, contentTransform).GetComponent<IngredientHouseItem>();
            ingredientHouseItems.Add(item);
            item.AssignIngredient(ingredient);
        }
    }

    public void UpdateIngredientsHousesObjTaken(List<IngredientData> ingredientToRemove, bool returnIngredient = false, bool forceReturnAll = false)
    {
        if (ingredientToRemove == null || ingredientToRemove.Count == 0) return;
        var groupedToRemove = ingredientToRemove
            .GroupBy(i => i)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var houseItem in ingredientHouseItems)
        {
            IngredientData ingredientData = houseItem.Ingredient.ingredientData;

            if (groupedToRemove.TryGetValue(ingredientData, out int removeCount))
            {
                if (returnIngredient)
                {
                    if (forceReturnAll)
                    {
                        houseItem.objTaken = 0;
                    }
                    else
                    {
                        houseItem.objTaken -= removeCount;
                    }
                    houseItem.UpdateDisplay();
                    houseItem.gameObject.SetActive(true);
                }
                else
                {
                    houseItem.objTaken += removeCount;
                    houseItem.UpdateDisplay();
                    if (houseItem.objTaken >= houseItem.Ingredient.amount)
                    {
                        houseItem.gameObject.SetActive(false);
                    }
                }
                
            }
        }
    }


}
