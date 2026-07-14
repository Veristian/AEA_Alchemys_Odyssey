using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Activator : MonoBehaviour
{
    [SerializeField] private UnityEvent OnActivation;
    private void Reset()
    {
        gameObject.SetActive(false);
    }
    private void Awake()
    {
        if (OnActivation == null) OnActivation = new UnityEvent();
        // gameObject.SetActive(false);
    }
    private void Start()
    {
        OnActivation?.Invoke();
    }
}
