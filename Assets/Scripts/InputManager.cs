using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Input Actions

    [SerializeField] private InputActionAsset _actions;
    public InputActionAsset Actions { get; private set; }
    public InputActionReference move;
    public InputActionReference sprint;
    public InputActionReference jump;
    public InputActionReference rightWallrun;
    public InputActionReference leftWallrun;
    public InputActionReference slide;
    public InputActionReference look;
    public InputActionReference throwObject;
    public InputActionReference pickup;
    public InputActionReference pause;
    private List<InputActionReference> _actionReferences;
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
        Actions = _actions;
        _actionReferences = new List<InputActionReference> { move, sprint, jump, leftWallrun, rightWallrun, slide, look, pickup, throwObject, pause };
    }


    private void OnEnable()
    {
        foreach (InputActionReference actionReference in _actionReferences)
        {
            actionReference.action.Enable();
        }
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
        foreach (InputActionReference actionReference in _actionReferences)
        {
            actionReference.action.Disable();
        }
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


    private void PerformMovement(InputAction.CallbackContext ctx)
    {
        InputMoveDirection = ctx.ReadValue<Vector2>();
    }


    private void StartSprint() { PressedSprint = true; }
    
    
    private void CancelSprint() { PressedSprint = false; }

    
    private void OnSprintInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) StartSprint();
        else if (ctx.canceled) CancelSprint();
    }


    private void StartJump() { PressedJump = true; }


    private void CancelJump() { PressedJump = false; }


    private void OnJumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) StartJump();
        else if (ctx.canceled) CancelJump();
    }
    

    private void StartRightWallrun() { PressedRightWallrun = true; }


    private void CancelRightWallrun() { PressedRightWallrun = false; }


    private void OnRightWallrunInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) StartRightWallrun();
        else if (ctx.canceled) CancelRightWallrun();
    }
    

    private void StartLeftWallrun() { PressedLeftWallrun = true; }


    private void CancelLeftWallrun() { PressedLeftWallrun = false; }


    private void OnLeftWallrunInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) StartLeftWallrun();
        else if (ctx.canceled) CancelLeftWallrun();
    }

    
    private void StartSlide() { PressedSlide = true; }


    public void CancelSlide() { PressedSlide = false; }


    private void OnSlideInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) StartSlide();
        else if (ctx.canceled) CancelSlide();
    }


    private void PerformLook(InputAction.CallbackContext ctx)
    {
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
