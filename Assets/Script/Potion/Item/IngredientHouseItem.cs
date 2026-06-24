using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class IngredientHouseItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image ItemImage;
    [Header("Ingredient Info")]
    [SerializeField] private IngredientInventoryData ingredient;
    public IngredientInventoryData Ingredient
    {
        get {return ingredient;}
    }
    [SerializeField, ReadOnly] public int objTaken;
    [SerializeField] private int displayedObjectAmount => ingredient.amount - objTaken;
    
    //private
    private void Start()
    {
        objTaken = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //gives self info and sets to show
        IngredientCurveDisplay.Instance.ingredientInventoryData = ingredient;
        IngredientCurveDisplay.Instance.SetVisibility(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //check if info is self and set null if yes and turn off
        if (IngredientCurveDisplay.Instance.ingredientInventoryData == ingredient)
        {
            IngredientCurveDisplay.Instance.ingredientInventoryData = null;
            IngredientCurveDisplay.Instance.SetVisibility(false);
        }
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

        if (ingredient != null && ingredient.ingredientData != null)
        {
            ItemImage.sprite = ingredient.ingredientData.ingredientSprite;
        }
    }

    private void GrabItem()
    {
        if (displayedObjectAmount <= 0) return;
        
        InputManager.Instance.AssignThisAsGrabbable(CreateGrabbable());
        IncreaseObjTaken();
        
    }

    private GameObject CreateGrabbable()
    {
        GameObject grabbable = GameObject.Instantiate(ingredient.ingredientData.ingredient2DObjectPrefab);
        grabbable.GetComponent<PotionIngredientObject>().ingredientHouseItem = this;
        return grabbable;
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