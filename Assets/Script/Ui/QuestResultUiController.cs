using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestResultUiController : MonoBehaviour
{
    private Animator animator;
    

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Start");

    }
    public void CloseObject()
    {
        animator.ResetTrigger("Start");
        gameObject.SetActive(false);
    }
    
}
