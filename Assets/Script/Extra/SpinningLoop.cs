using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningLoop : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f; 

    void Update()
    {

        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

    }
}
