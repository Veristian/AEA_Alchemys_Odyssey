using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhilePivotTrigger : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private Transform currentPivot;

    
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (virtualCamera == null) return;

        virtualCamera.Follow = currentPivot;
    }

}
