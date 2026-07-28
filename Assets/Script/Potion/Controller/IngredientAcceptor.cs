using System;
using UnityEngine;

public class IngredientAcceptor : MonoBehaviour
{
    public event Action<PotionIngredientObject> OnIngredientAccepted;

    void OnTriggerStay2D(Collider2D collision)
    {
        PotionIngredientObject ingredient = collision.GetComponentInParent<PotionIngredientObject>();
        if (InputManager.Instance.isGrabbing)
        {
            if (InputManager.Instance.assignedGrabbedObject.gameObject == ingredient.gameObject) return;
        }

        if (ingredient != null)
        {
            OnIngredientAccepted?.Invoke(ingredient);
        }
        collision.enabled = false;
        Destroy(collision.gameObject.transform.parent.gameObject, 0.1f);
    }


}