using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Cast Settings")]
    [SerializeField] private Vector3 castCenter = Vector3.zero;
    [SerializeField] private float radius = 0.5f;

    [Header("Layer")]
    [SerializeField] private LayerMask interactLayer;

    [Header("Debug")]
    [SerializeField] private bool drawDebug = true;

    IInteractable interactable;
    public string interactText;
    private void FixedUpdate()
    {
        CheckInteractable();
        if (InputManager.Instance.InteractWasPressed)
        {
            Interact();
        }
    }

    private void CheckInteractable()
    {
        Vector3 origin = transform.position + castCenter;
        Vector3 direction = transform.forward;

        Collider[] hits = Physics.OverlapSphere(origin, radius, interactLayer);
        if (hits.Length == 0)
        {
            interactText = null;
            interactable = null;
        }
        foreach (var col in hits)
        {
            interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (!interactable.interactable)
                {
                    continue;
                }
                interactText = interactable.text;
                // interactable.Interact(gameObject);
                break; // interact with first found
            }
        }
    }

    private void Interact()
    {
        if (interactable == null) 
        {
            interactText = null;
            return;
        }
        interactable.Interact(gameObject);
        if (!interactable.interactable) interactText = null;
    }    

   private void OnDrawGizmos()
    {
        if (!drawDebug) return;

        Gizmos.color = Color.green;

        Vector3 origin = transform.position + castCenter;

        Gizmos.DrawWireSphere(origin, radius);
    }
}