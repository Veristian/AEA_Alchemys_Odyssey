using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientData ingredientData;
    [SerializeField] private GameObject herbModel;
    private bool isTaken;
    private int dayTaken;
    public void Interact(GameObject interactor)
    {
        TakeHerb();
    }

    void TakeHerb()
    {
        if (isTaken) return;
        isTaken = true;
        dayTaken = DayManager.Instance.Day;
        
        herbModel?.SetActive(false);
        if (ingredientData)
            InventoryManager.Instance.AddIngredient(ingredientData);
    }

    void RefreshHerb()
    {
        isTaken = false;
        herbModel?.SetActive(true);
    }

    void OnEnable()
    {
        if (isTaken && dayTaken < DayManager.Instance.Day)
        {
            RefreshHerb();
        }
    }

}
