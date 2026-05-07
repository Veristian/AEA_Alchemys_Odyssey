using System;
using UnityEngine;

public class IngredientAcceptor : MonoBehaviour
{
    public event Action<PotionIngredientObject> OnIngredientAccepted;

    void OnTriggerEnter2D(Collider2D collision)
    {
        PotionIngredientObject ingredient = collision.GetComponentInParent<PotionIngredientObject>();

        if (ingredient != null)
        {
            OnIngredientAccepted?.Invoke(ingredient);
        }
        Destroy(collision.gameObject.transform.parent.gameObject);
    }


}