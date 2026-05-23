using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Rigidbody))]
public class PhysicsObject2D : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    private Vector3 rotation;
    [Header("Settings")]
    [SerializeField] private float defaultPlaneZ = 0;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float drag = 0.1f;
    [SerializeField] private float popForce = -9.81f;


    [SerializeField, ReadOnly] private bool isGrabbed;
    [SerializeField, ReadOnly] private bool wasGrabbed;
    private Vector3 previousFramePosition;
    private Vector2 previousFrameSpeed;
    Camera cam;
    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
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

    private void FixedUpdate()
    {
        if (!isGrabbed)
        {
            ApplyPhysics();
        }
        previousFrameSpeed = (transform.position - previousFramePosition) / Time.fixedDeltaTime;
        previousFramePosition = transform.position;
        if (wasGrabbed != isGrabbed && previousFrameSpeed.magnitude < 0.1f)
        {
            ApplyPop();
        }

        wasGrabbed = isGrabbed;

    }
    private void ApplyPhysics()
    {
        ApplyGravity();
        ApplyDrag();
    }

    private void ApplyPop()
    {
        rb.AddForce(new Vector3(0, popForce * rb.mass, 0), ForceMode.Impulse);
    }
    
    private void ApplyGravity()
    {
        rb.AddForce(new Vector3(0, gravity * rb.mass, 0), ForceMode.Force);
        // rb.velocity += new Vector3(0, gravity * Time.deltaTime, 0);
    }
    private void ApplyDrag()
    {
        rb.AddForce(-rb.velocity * drag, ForceMode.Force);
        // rb.velocity *= (1 - drag * Time.deltaTime);
    }

    private void LerpToDefault()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), Time.deltaTime * 5);
    }

    private void SetPositionToMouse()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = defaultPlaneZ;
        transform.position = mousePos;
    }

    public void Grab()
    {
        isGrabbed = true;
        rb.isKinematic = true;
        SetPositionToMouse();
    }
    public void Release()
    {
        isGrabbed = false;
        rb.isKinematic = false;
        rb.velocity = previousFrameSpeed;
    }   

}
