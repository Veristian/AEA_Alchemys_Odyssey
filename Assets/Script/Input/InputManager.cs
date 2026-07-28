using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : Singleton<InputManager>
{
    [Header("Input Settings")]
    //bool
    [SerializeField] bool _canTakeInputs = true;
    public bool canTakeInputs
    {
        get => _canTakeInputs;
        set
        {
            if (_canTakeInputs == value) return;

            _canTakeInputs = value;
            UpdateCameraLookState();
        }
    }
    public bool canPause = true;
    public bool canMove = true;
    public bool canInteract = true;
    public bool canUiPopup = true;
    [SerializeField]
    private bool _canLook = true;

    public bool canLook
    {
        get => _canLook;
        set
        {
            if (_canLook == value) return;

            _canLook = value;
            UpdateCameraLookState();
        }
    }
    
    [Header("References")]
    //ref
    public PlayerInput playerInput;
    [Header("Camera")]
    public CinemachineInputProvider cameraInputProvider;
    [Header("Input Values")]
    //inputs
    [ReadOnly] public Vector2 Movement;
    [ReadOnly] public Vector2 Look;
    [ReadOnly] public Vector2 Scroll;
    [ReadOnly] public Vector2 MousePosition;
    [ReadOnly] public bool JumpWasPressed;
    [ReadOnly] public bool JumpIsHeld;
    [ReadOnly] public bool JumpWasReleased;
    [ReadOnly] public bool PauseWasPressed;
    [ReadOnly] public bool InteractWasPressed;
    [ReadOnly] public bool InteractIsHeld;
    [ReadOnly] public bool InteractWasReleased;
    [ReadOnly] public bool MouseLeftWasPressed;
    [ReadOnly] public bool MouseLeftIsHeld;
    [ReadOnly] public bool MouseLeftWasReleased;
    [ReadOnly] public bool InventoryWasPressed;
    [ReadOnly] public bool JournalWasPressed;


    //actions
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private InputAction _pauseAction;
    private InputAction _interactAction;
    private InputAction _inventoryAction;
    private InputAction _journalAction;

    private InputAction _mousePositionAction;

    private InputAction _mouseLeftAction;
    [Header("Grab")]
    public LayerMask grabLayer;
    [ReadOnly] public bool hoveringGrabbable; //updates every frame based on raycast
    [ReadOnly] public bool isGrabbing; //updates on mouse up or down
    [ReadOnly] public PhysicsObject2D assignedGrabbedObject; //updates on mouse up or down

    //private fields
    RaycastHit hit;
    Ray ray;
    Camera mainCamera;

    protected override void Awake()
    {
        base.Awake();
        canTakeInputs = true;
        playerInput = GetComponent<PlayerInput>();

        _moveAction = playerInput.actions["Move"];
        _lookAction = playerInput.actions["Look"];

        _jumpAction = playerInput.actions["Jump"];

        _pauseAction = playerInput.actions["Pause"];

        _interactAction = playerInput.actions["Interact"];

        _mousePositionAction = playerInput.actions["MousePos"];

        _mouseLeftAction = playerInput.actions["MouseLeft"];

        _inventoryAction = playerInput.actions["Inventory"];
        _journalAction = playerInput.actions["Journal"];
        mainCamera = Camera.main ?? FindFirstObjectByType<Camera>();
        
        UpdateCameraLookState();
    }

    private void Update()
    {
        if (canTakeInputs)
        {
            if (canMove)
            {
                Movement = _moveAction.ReadValue<Vector2>();
                JumpWasPressed = _jumpAction.WasPressedThisFrame();
                JumpIsHeld = _jumpAction.IsPressed();
                JumpWasReleased = _jumpAction.WasReleasedThisFrame();
            }
            else
            {
                Movement = Vector2.zero;
                JumpWasPressed = false;
                JumpIsHeld = false;
                JumpWasReleased = false;
            }
            if (canLook)
            {
                Look = _lookAction.ReadValue<Vector2>();
            }
            else
            {
                Look = Vector2.zero;
            }
            MousePosition = _mousePositionAction.ReadValue<Vector2>();

            if (canInteract)
            {
                InteractWasPressed = _interactAction.WasPressedThisFrame();
                InteractIsHeld = _interactAction.IsPressed();
                InteractWasReleased = _interactAction.WasReleasedThisFrame();
            }
            else
            {
                InteractWasPressed = false;
                InteractIsHeld = false;
                InteractWasReleased = false;
            }
            
            MouseLeftWasPressed = _mouseLeftAction.WasPressedThisFrame();
            MouseLeftIsHeld = _mouseLeftAction.IsPressed();
            MouseLeftWasReleased = _mouseLeftAction.WasReleasedThisFrame();

        }
        else
        {
            Movement = Vector2.zero;
            Look = Vector2.zero;
            Scroll = Vector2.zero;
            MousePosition = Vector2.zero;

            JumpWasPressed = false;
            JumpIsHeld = false;
            JumpWasReleased = false;

            InteractWasPressed = false;
            InteractIsHeld = false;
            InteractWasReleased = false;

            MouseLeftWasPressed = false;
            MouseLeftIsHeld = false;
            MouseLeftWasReleased = false;
        }
        if (canPause)
        {
            PauseWasPressed = _pauseAction.WasPressedThisFrame();
        }
        if (canUiPopup)
        {
            JournalWasPressed = _journalAction.WasPressedThisFrame();
            InventoryWasPressed = _inventoryAction.WasPressedThisFrame();
        }
        HandleGrab();

    }

    public void DisableInputs()
    {
        canTakeInputs = false;
    }
    public void EnableInputs()
    {
        canTakeInputs = true;
    }
    public void AssignThisAsGrabbable(GameObject grabbedObj)
    {
        Release();

        var obj = grabbedObj.GetComponent<PhysicsObject2D>();
        if (obj == null) return;

        assignedGrabbedObject = obj;
        Grab();
    }
    private void HandleGrab()
    {
        //raycast to find grabbable

        ray = mainCamera.ScreenPointToRay(MousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, grabLayer))
        {
            hoveringGrabbable = true;
        }
        else
        {
            hoveringGrabbable = false;
        }

        //on mouse left press, if hovering grabbable, assign it as grabbed
        if (MouseLeftWasPressed && hoveringGrabbable)
        {
            Release();
            assignedGrabbedObject = hit.collider.GetComponent<PhysicsObject2D>();
            Grab();
        }
        //on mouse left release, if currently grabbing, release it
        if (MouseLeftWasReleased && isGrabbing)
        {
            Release();
        }

        //draw raycast for debugging
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
    }

    private void Grab()
    {
        if (assignedGrabbedObject != null)
        {
            assignedGrabbedObject.Grab();
            isGrabbing = true;
        }

    }

    private void Release()
    {
        isGrabbing = false;
        if (assignedGrabbedObject != null)
        {
            assignedGrabbedObject.Release();
            assignedGrabbedObject = null;
        }

    }
    public void UpdateCameraLookState()
    {
        if (cameraInputProvider != null)
            cameraInputProvider.enabled = _canLook && _canTakeInputs;
    }


}
