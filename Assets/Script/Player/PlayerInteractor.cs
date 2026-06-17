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

    private void Update()
    {
        if (InputManager.Instance.InteractWasPressed)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Vector3 origin = transform.position + castCenter;
        Vector3 direction = transform.forward;

        Collider[] hits = Physics.OverlapSphere(origin, radius, interactLayer);

        foreach (var col in hits)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
                break; // interact with first found
            }
        }
    }

   private void OnDrawGizmos()
    {
        if (!drawDebug) return;

        Gizmos.color = Color.green;

        Vector3 origin = transform.position + castCenter;

        Gizmos.DrawWireSphere(origin, radius);
    }
}