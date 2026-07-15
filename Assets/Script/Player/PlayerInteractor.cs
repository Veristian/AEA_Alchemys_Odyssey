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
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

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
            PlayerInteractIndicator.Instance.interactableIndicatorChecker(interactText);
        }
        // foreach (var col in hits)
        // {
        //     interactable = col.GetComponent<IInteractable>();
        //     if (interactable != null)
        //     {
        //         if (!interactable.interactable)
        //         {
        //             continue;
        //         }
        //         interactText = interactable.text;
        //         // interactable.Interact(gameObject);
        //         break; // interact with first found
                
        //     }
        // }
        Collider closest = null;

        foreach (var col in hits)
        {
            var candidate = col.GetComponent<IInteractable>();

            if (candidate == null || !candidate.interactable)
                continue;

            if (closest == null ||
                (col.transform.position - transform.position).sqrMagnitude <
                (closest.transform.position - transform.position).sqrMagnitude)
            {
                closest = col;
                interactable = candidate;
                interactText = candidate.text;
                PlayerInteractIndicator.Instance.interactableIndicatorChecker(interactText);
            }
        }
    }

    private void Interact()
    {
        if (interactable == null) 
        {
            interactText = null;
            PlayerInteractIndicator.Instance.interactableIndicatorChecker(interactText);
            return;
        }
        if (interactable.GetType() == typeof(Herb))
        {
            StartCoroutine(PickUpAnimation());
        }
        else
        {
            interactable.Interact(gameObject);
            if (!interactable.interactable)
            {
                interactText = null;
                PlayerInteractIndicator.Instance.interactableIndicatorChecker(interactText);
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

    IEnumerator PickUpAnimation()
    {
        rb.velocity = Vector3.zero;
        animator.SetTrigger("PickUp");
        InputManager.Instance.canMove = false;
        yield return new WaitForSeconds(0.5f);
        interactable.Interact(gameObject);
        InputManager.Instance.canMove = true;
        if (!interactable.interactable)
        {
            interactText = null;
            PlayerInteractIndicator.Instance.interactableIndicatorChecker(interactText);
        }

    }
}