using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{    
    //bool
    public bool canTakeInputs = true;
    public bool canPause = true;
    //ref
    public PlayerInput playerInput;

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


    //actions
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private InputAction _pauseAction; 
    private InputAction _interactAction;

    private InputAction _mousePositionAction;

    private InputAction _mouseLeftAction;


    private void Awake()
    {

        canTakeInputs = true;
        playerInput = GetComponent<PlayerInput>();

        _moveAction = playerInput.actions["Move"];
        _lookAction = playerInput.actions["Look"];

        _jumpAction = playerInput.actions["Jump"];

        _pauseAction = playerInput.actions["Pause"];

        _interactAction = playerInput.actions["Interact"];

        _mousePositionAction = playerInput.actions["MousePos"];

        _mouseLeftAction = playerInput.actions["MouseLeft"];


    }

    private void Update()
    {
        if (canTakeInputs)
        {
            Movement = _moveAction.ReadValue<Vector2>();
            Look = _lookAction.ReadValue<Vector2>();
            MousePosition = _mousePositionAction.ReadValue<Vector2>();
            
            JumpWasPressed = _jumpAction.WasPressedThisFrame();
            JumpIsHeld = _jumpAction.IsPressed();
            JumpWasReleased = _jumpAction.WasReleasedThisFrame();

            InteractWasPressed = _interactAction.WasPressedThisFrame();
            InteractIsHeld = _interactAction.IsPressed();
            InteractWasReleased = _interactAction.WasReleasedThisFrame();
            
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

    }

    public void DisableInputs()
    {
        canTakeInputs = false;
    }
    public void EnableInputs()
    {
        canTakeInputs = true;
    }
}
