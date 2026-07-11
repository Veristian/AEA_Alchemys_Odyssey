using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactables : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionText;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private UnityEvent OnInteract;
    
    private void Awake()
    {
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
