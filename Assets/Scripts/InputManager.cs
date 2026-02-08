using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputActionReference move;
    public InputActionReference sprint;
    public InputActionReference jump;
    public InputActionReference rightWallrun;
    public InputActionReference leftWallrun;
    public InputActionReference slide;
    public InputActionReference look;
    public InputActionReference throwObject;
    public InputActionReference pickup;
    private List<InputActionReference> actionReferences;


    public Vector2 InputMoveDirection { get; private set; } // auto-property that allows public getting, but not setting (encapsulation!!)
    public bool PressedSprint { get; private set; }
    public bool PressedJump { get; private set; }
    public bool PressedLeftWallrun { get; private set; }
    public bool PressedRightWallrun { get; private set; }
    public bool PressedSlide { get; private set; }
    public Vector2 DeltaCameraMovement { get; private set; }
    public bool PressedThrowObject { get; private set; }



    void Awake()
    {
        actionReferences = new() { move, sprint, jump, leftWallrun, rightWallrun, slide, look, pickup, throwObject };
    }

    void OnEnable()
    {
        foreach (InputActionReference actionReference in actionReferences)
        {
            actionReference.action.Enable();
        }
        move.action.performed += PerformMovement;
        sprint.action.started += StartSprint;
        sprint.action.canceled += CancelSprint;
        jump.action.started += StartJump;
        jump.action.canceled += CancelJump;
        // technically using an event approach for jumping isn't any better than just using .triggered, but I kept it for consistency across all actions
        rightWallrun.action.started += StartRightWallrun;
        rightWallrun.action.canceled += CancelRightWallrun;
        leftWallrun.action.started += StartLeftWallrun;
        leftWallrun.action.canceled += CancelLeftWallrun;
        slide.action.started += StartSlide;
        slide.action.canceled += CancelSlide;

        // Camera
        look.action.performed += PerformLook;

        // Interaction Controller
        pickup.action.started += StartPickup;
        throwObject.action.started += StartThrowObject;
        throwObject.action.performed += PerformThrowObject;
    }

    void OnDisable()
    {
        foreach (InputActionReference actionReference in actionReferences)
        {
            actionReference.action.Disable();
        }
        move.action.performed -= PerformMovement;
        sprint.action.started -= StartSprint;
        sprint.action.canceled -= CancelSprint;
        jump.action.started -= StartJump;
        jump.action.canceled -= CancelJump;
        rightWallrun.action.started -= StartRightWallrun;
        rightWallrun.action.canceled -= CancelRightWallrun;
        leftWallrun.action.started -= StartLeftWallrun;
        leftWallrun.action.canceled -= CancelLeftWallrun;
        slide.action.started -= StartSlide;
        slide.action.canceled -= CancelSlide;

        // Camera
        look.action.performed -= PerformLook;

        // Interaction Controller
        pickup.action.started -= StartPickup;
        throwObject.action.started -= StartThrowObject;
        throwObject.action.performed -= PerformThrowObject;
    }


    void PerformMovement(InputAction.CallbackContext ctx)
    {
        InputMoveDirection = ctx.ReadValue<Vector2>();
    }


    void StartSprint(InputAction.CallbackContext ctx)
    {
        PressedSprint = true;
    }


    void CancelSprint(InputAction.CallbackContext ctx)
    {
        PressedSprint = false;
    }


    void StartJump(InputAction.CallbackContext ctx)
    {
        PressedJump = true;
    }


    void CancelJump(InputAction.CallbackContext ctx)
    {
        PressedJump = false;
    }


    void StartRightWallrun(InputAction.CallbackContext ctx)
    {
        PressedRightWallrun = true;
    }


    void CancelRightWallrun(InputAction.CallbackContext ctx)
    {
        PressedRightWallrun = false;
    }


    void StartLeftWallrun(InputAction.CallbackContext ctx)
    {
        PressedLeftWallrun = true;
    }


    void CancelLeftWallrun(InputAction.CallbackContext ctx)
    {
        PressedLeftWallrun = false;
    }


    void StartSlide(InputAction.CallbackContext ctx)
    {
        PressedSlide = true;
    }


    void CancelSlide(InputAction.CallbackContext ctx)
    {
        PressedSlide = false;
    }


    void PerformLook(InputAction.CallbackContext ctx)
    {
        DeltaCameraMovement = ctx.ReadValue<Vector2>();
    }

    void StartPickup(InputAction.CallbackContext ctx)
    {

    }

    void StartThrowObject(InputAction.CallbackContext ctx)
    {
        PressedThrowObject = true;
    }


    void PerformThrowObject(InputAction.CallbackContext ctx)
    {

    }
}
