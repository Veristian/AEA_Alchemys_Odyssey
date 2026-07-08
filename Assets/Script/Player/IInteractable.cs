using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string text { get; }
    bool interactable {get; }
    void Interact(GameObject interactor);
}