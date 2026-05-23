using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
//change to potion
public class PotionHouseItem : MonoBehaviour, IPointerDownHandler
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI amountText;  
    [Header("Potion Info")]
    [SerializeField] private PotionInventoryData potion;
    public PotionInventoryData Potion
    {
        get {return potion;}
    }
    [SerializeField, ReadOnly] public int objTaken;
    [SerializeField] private int displayedObjectAmount => potion.amount - objTaken;
    
    //private
    private void Start()
    {
        objTaken = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GrabItem();
    }
    public void AssignPotion(PotionInventoryData potion)
    {
        this.potion = potion;
        UpdateDisplay();
        if (potion.amount <= 0)
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
        return GameObject.Instantiate(potion.potionData.potion2DObjectPrefab);
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