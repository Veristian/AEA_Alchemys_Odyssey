using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestroy : MonoBehaviour
{
    [SerializeField] private float destroyTime = 2.5f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
