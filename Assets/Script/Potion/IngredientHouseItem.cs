using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class IngredientHouseItem : MonoBehaviour, IPointerDownHandler
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI amountText;  
    [Header("Ingredient Info")]
    [SerializeField] private IngredientInventoryData ingredient;
    [SerializeField, ReadOnly] private int objTaken;
    [SerializeField] private int displayedObjectAmount => ingredient.amount - objTaken;
    
    //private
    private void Start()
    {
        objTaken = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GrabItem();
    }
    public void AssignIngredient(IngredientInventoryData ingredient)
    {
        this.ingredient = ingredient;
        UpdateDisplay();
        if (ingredient.amount <= 0)
        {
            // gameObject.SetActive(false);
        }
        
    }

    private void UpdateDisplay()
    {
        amountText.text = displayedObjectAmount.ToString();
    }

    private void GrabItem()
    {
        if (displayedObjectAmount <= 0) return;
        
        InputManager.Instance.AssignThisAsGrabbable(CreateGrabbable());
        IncreaseObjTaken();
        
    }

    private GameObject CreateGrabbable()
    {
        return GameObject.Instantiate(ingredient.ingredientData.ingredient2DObjectPrefab);
    }

    public void IncreaseObjTaken()
    {
        objTaken++;
        UpdateDisplay();
    }
    public void DecreaseObjTaken()
    {
        objTaken = (int)Mathf.Clamp(objTaken - 1, 0, Mathf.Infinity);
        UpdateDisplay();
    }

    
}