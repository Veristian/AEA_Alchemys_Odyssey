using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMArea : MonoBehaviour
{
    public bool useSecondBGM;
    private void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Player"))
            return;

        AudioController.Instance.SwitchArea(useSecondBGM);
    }
}
