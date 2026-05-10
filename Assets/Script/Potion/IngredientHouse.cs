using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientHouse : MonoBehaviour
{
    [Header("Reference")]
    public Transform contentTransform;

    public GameObject ingredientHouseItemPrefab;

    private void Awake()
    {
        DataManager.Instance.OnGameLoaded += SetupIngredients;
    }

    private void SetupIngredients()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (IngredientData ingredientData in DataManager.Instance.ingredientDatas)
        {
            Instantiate(ingredientHouseItemPrefab, contentTransform).GetComponent<IngredientHouseItem>().AssignIngredient(InventoryManager.Instance.PassIngredientReference(ingredientData));
        }
    }

}
