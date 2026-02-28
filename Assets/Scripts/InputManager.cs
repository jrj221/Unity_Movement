using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Input Actions
    [SerializeField] private InputActionAsset _actions;
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference sprint;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference rightWallrun;
    [SerializeField] private InputActionReference leftWallrun;
    [SerializeField] private InputActionReference slide;
    [SerializeField] private InputActionReference look;
    [SerializeField] private InputActionReference throwObject;
    [SerializeField] private InputActionReference pickup;
    [SerializeField] private InputActionReference pause;
    #endregion

    #region Input Properties
    public Vector2 InputMoveDirection { get; private set; } // auto-property that allows public getting, but not setting (encapsulation!!)
    public bool PressedSprint { get; private set; }
    public bool PressedJump { get; private set; }
    public bool PressedLeftWallrun { get; private set; }
    public bool PressedRightWallrun { get; private set; }
    public bool PressedSlide { get; private set; }
    public Vector2 DeltaCameraMovement { get; private set; }
    public bool PressedThrowObject { get; private set; }
    public bool PressedPause { get; private set; }
    #endregion

    
    public static InputManager Instance { get; private set; } // global singleton
    
    
    private void Awake()
    {
        Instance = this;
    }
    

    private void OnEnable()
    {
        _actions.Enable();
        move.action.performed += PerformMovement;
        sprint.action.started += OnSprintInput;
        sprint.action.canceled += OnSprintInput;
        jump.action.started += OnJumpInput;
        jump.action.canceled += OnJumpInput;
        // technically using an event approach for jumping isn't any better than just using .triggered, but I kept it for consistency across all actions
        rightWallrun.action.started += OnRightWallrunInput;
        rightWallrun.action.canceled += OnRightWallrunInput;
        leftWallrun.action.started += OnLeftWallrunInput;
        leftWallrun.action.canceled += OnLeftWallrunInput;
        slide.action.started += OnSlideInput;
        slide.action.canceled += OnSlideInput;

        // Camera
        look.action.performed += PerformLook;

        // Interaction Controller
        pickup.action.started += StartPickup;
        throwObject.action.started += StartThrowObject;
        throwObject.action.performed += PerformThrowObject;

        // UI
        pause.action.started += OnPauseInput;
    }


    private void OnDisable()
    {
        _actions.Disable();
        move.action.performed -= PerformMovement;
        sprint.action.started -= OnSprintInput;
        sprint.action.canceled -= OnSprintInput;
        jump.action.started -= OnJumpInput;
        jump.action.canceled -= OnJumpInput;
        rightWallrun.action.started -= OnRightWallrunInput;
        rightWallrun.action.canceled -= OnRightWallrunInput;
        leftWallrun.action.started -= OnLeftWallrunInput;
        leftWallrun.action.canceled -= OnLeftWallrunInput;
        slide.action.started -= OnSlideInput;
        slide.action.canceled -= OnSlideInput;

        // Camera
        look.action.performed -= PerformLook;

        // Interaction Controller
        pickup.action.started -= StartPickup;
        throwObject.action.started -= StartThrowObject;
        throwObject.action.performed -= PerformThrowObject;

        // UI
        pause.action.started -= OnPauseInput;
    }


    public void DisableInput()
    {
        _actions.Disable();
        InputMoveDirection = Vector2.zero;
        DeltaCameraMovement = Vector2.zero;
    }
    
    
    public void EnableInput() { _actions.Enable(); }


    private void PerformMovement(InputAction.CallbackContext ctx) { InputMoveDirection = ctx.ReadValue<Vector2>(); }


    private void StartSprint() { PressedSprint = true; }
    
    
    private void CancelSprint() { PressedSprint = false; }

    
    private void OnSprintInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started) StartSprint();
        else if (ctx.canceled) CancelSprint();
    }


    private void StartJump() { PressedJump = true; }


    public void CancelJump() { PressedJump = false; }


    private void OnJumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started) StartJump();
        else if (ctx.canceled) CancelJump();
    }
    

    private void StartRightWallrun() { PressedRightWallrun = true; }


    private void CancelRightWallrun() { PressedRightWallrun = false; }


    private void OnRightWallrunInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started) StartRightWallrun();
        else if (ctx.canceled) CancelRightWallrun();
    }
    

    private void StartLeftWallrun() { PressedLeftWallrun = true; }


    private void CancelLeftWallrun() { PressedLeftWallrun = false; }


    private void OnLeftWallrunInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started) StartLeftWallrun();
        else if (ctx.canceled) CancelLeftWallrun();
    }

    
    private void StartSlide() { PressedSlide = true; }


    public void CancelSlide() { PressedSlide = false; }


    private void OnSlideInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started) StartSlide();
        else if (ctx.canceled) CancelSlide();
    }


    private void PerformLook(InputAction.CallbackContext ctx)
    {
        Debug.Log("LOOKING");
        DeltaCameraMovement = ctx.ReadValue<Vector2>();
    }

    private void StartPickup(InputAction.CallbackContext ctx)
    {

    }

    private void StartThrowObject(InputAction.CallbackContext ctx)
    {
        PressedThrowObject = true;
    }


    private void PerformThrowObject(InputAction.CallbackContext ctx)
    {

    }
    

    public void TogglePause() { PressedPause = !PressedPause; } // this way you repress the button to toggle

    
    private void OnPauseInput(InputAction.CallbackContext ctx) { TogglePause(); }
}
