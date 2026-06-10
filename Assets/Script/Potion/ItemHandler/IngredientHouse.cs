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
        DataManager.Instance.OnGameLoaded -= SetupIngredients;
    }

    private void SetupIngredients()
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
            IngredientHouseItem item = Instantiate(ingredientHouseItemPrefab, contentTransform).GetComponent<IngredientHouseItem>();
            ingredientHouseItems.Add(item);
            item.AssignIngredient(InventoryManager.Instance.PassIngredientReference(ingredientData));
        }
    }

    public void UpdateIngredientsHousesObjTaken(List<IngredientData> ingredientToRemove)
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
                houseItem.objTaken += removeCount;
            }
        }
    }


}
