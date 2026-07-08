using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class Herb : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientData ingredientData;
    [SerializeField] private GameObject herbModel;
    [SerializeField] private ParticleSystem CollectedEffect;
    private bool isTaken;
    private int dayTaken;
    SphereCollider col;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;  
    }
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
        {
            InventoryManager.Instance.AddIngredient(ingredientData);
            if (PickupPopupManager.Instance != null)
            {
                PickupPopupManager.Instance.ShowPickup(ingredientData);
                CollectedEffect.Play();
            }
        }
            
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
