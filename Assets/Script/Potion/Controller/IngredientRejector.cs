using System;
using UnityEngine;

public class IngredientRejector : MonoBehaviour
{
    public event Action<PotionIngredientObject> OnIngredientRejected;

    void OnTriggerEnter2D(Collider2D collision)
    {
        PotionIngredientObject ingredient = collision.GetComponentInParent<PotionIngredientObject>();

        if (ingredient != null)
        {
            OnIngredientRejected?.Invoke(ingredient);
            ingredient.ingredientHouseItem.DecreaseObjTaken();
            ingredient.ingredientHouseItem.gameObject.SetActive(true);

        }
        Destroy(collision.gameObject.transform.parent.gameObject, 0.1f);
    }


}