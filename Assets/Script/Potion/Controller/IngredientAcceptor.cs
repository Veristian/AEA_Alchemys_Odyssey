using System;
using UnityEngine;

public class IngredientAcceptor : MonoBehaviour
{
    public event Action<PotionIngredientObject> OnIngredientAccepted;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (InputManager.Instance.isGrabbing) return;
        PotionIngredientObject ingredient = collision.GetComponentInParent<PotionIngredientObject>();

        if (ingredient != null)
        {
            OnIngredientAccepted?.Invoke(ingredient);
        }
        collision.enabled = false;
        Destroy(collision.gameObject.transform.parent.gameObject, 0.1f);
    }


}