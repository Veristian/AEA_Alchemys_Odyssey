using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsObject2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 rotation;
    [SerializeField] private float defaultPlaneZ = 0;

    [SerializeField, ReadOnly] private bool isGrabbed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        if (isGrabbed)
        {
            LerpToDefault();
            SetPositionToMouse();
        }

    }

    private void LerpToDefault()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), Time.deltaTime * 5);
    }

    private void SetPositionToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = defaultPlaneZ;
        transform.position = mousePos;
    }
    

}
