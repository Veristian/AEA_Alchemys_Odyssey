using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class Interactables : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionText;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private UnityEvent OnInteract;
    private void Reset()
    {
        SetLayer();
    }

    private void SetLayer()
    {
        int layer = LayerMask.NameToLayer("Interactable");

        if (layer != -1)
        {
            gameObject.layer = layer;
        }
        else
        {
            Debug.LogWarning("Layer 'Interactables' does not exist.");
        }
    }
    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        if (OnInteract == null) OnInteract = new UnityEvent();
    }
    public string text
    {
        get {return interactionText; }
    }
    public bool interactable
    {
        get {return isInteractable;}
    }

    public void Interact(GameObject interactor)
    {
        OnInteract?.Invoke();
    }

    public void ToggleInteractable()
    {
        isInteractable = !interactable;
    }
    public void SetInteractable(bool isInteractable)
    {
        this.isInteractable = isInteractable;
    }
    
}
