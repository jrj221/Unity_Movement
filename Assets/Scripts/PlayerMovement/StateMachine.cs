using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class StateMachine : MonoBehaviour
{
    #region Objects and Action References
    [Header("Objects and Action References")]
    public Rigidbody rb;
    public Transform feet;
    public InputActionReference move;
    public InputActionReference sprint;
    public InputActionReference jump;
    public InputActionReference rightWallrun;
    public InputActionReference leftWallrun;
    public InputActionReference slide;
    #endregion

    #region Movement
    [Header("Movement")]
    public float sprintSpeed;
    public float normalSpeed;
    public float groundDrag;
    public float airMovementMultiplier;
    private Vector2 inputMoveDirection;
    [NonSerialized] public Vector3 moveDirection;
    private bool pressedSprint;
    [NonSerialized] public bool isSprinting;
    [NonSerialized] public bool isMoving;
    #endregion

    #region Wallrunning
    [Header("Wallrunning")]
    public float WallrunRotationSpeed;
    public float WallrunAngle;
    public float pushIntoWallForce;
    public float moveLeftInputLockLength;
    public float moveRightInputLockLength;
    public float isLeftWallrunningBufferLength;
    public float isRightWallrunningBufferLength;
    private bool pressedLeftWallrun;
    private bool pressedRightWallrun;
    [NonSerialized] public bool isLeftWallrunning;
    [NonSerialized] public bool isRightWallrunning;
    private bool leftWallrunStartTriggered;
    private bool rightWallrunStartTriggered;
    [NonSerialized] public bool leftWallrunStopTriggered;
    [NonSerialized] public bool rightWallrunStopTriggered;
    [NonSerialized] public float moveRightInputLockTime = 0;
    [NonSerialized] public float moveLeftInputLockTime = 0;
    private bool moveRightLocked;
    private bool moveLeftLocked;
    [NonSerialized] public float isLeftWallrunningBufferTime = 0;
    [NonSerialized] public float isRightWallrunningBufferTime = 0;
    private bool isLeftWallrunningIsBuffered;
    private bool isRightWallrunningIsBuffered;
    #endregion

    #region Jumping
    [Header("Jumping")]
    public float jumpForce;
    public float wallVerticalJumpForce;
    public float wallSideJumpForce;
    public float slideJumpHorizontalForce;
    public float slideJumpVerticalForce;
    [NonSerialized] public bool pressedJump;
    private bool jumpTriggered;
    [NonSerialized] public bool jumpApplied;
    #endregion

    #region Air
    [Header("Air")]
    [NonSerialized] public bool inAir;
    #endregion

    #region Sliding
    [Header("Sliding")]
    public float slideRotationSpeed;
    public float slideAngle;
    public float slideSpeed;
    public float maxSlideTime;
    [NonSerialized] public float slideTime;
    [NonSerialized] public bool slideTimerOngoing;
    [NonSerialized] public bool slideStopTriggered;
    [NonSerialized] public bool pressedSlide;
    [NonSerialized] public bool isSliding;
    #endregion

    #region Stairs
    [Header("Stairs")]
    public float minStepLength;
    public float maxStepHeight;
    public float maxStepSlope;
    [NonSerialized] public bool justSteppedUp;
    #endregion

    #region Slopes
    [Header("Slopes")]
    public float maxSlopeAngle;
    public float stickToSlopeForce;
    #endregion

    #region Gravity
    [Header("Gravity")]
    public float playerGravity;
    public float wallJumpGravity;
    [NonSerialized] public bool useCustomGravity;
    [NonSerialized] public bool useWallJumpGravity;
    #endregion

    #region Camera
    [Header("Camera")]
    public float cameraSmoothingEnableTimeLength;
    [NonSerialized] public float cameraSmoothingEnableTime;
    [NonSerialized] public bool cameraSmoothingEnabled;
    [NonSerialized] public bool forceMeshSnap;
    #endregion

    #region Misc.
    private List<InputActionReference> actionReferences;
    [NonSerialized] public Vector3 wallDirection;
    [NonSerialized] public readonly float playerRadius = 0.5f;
    [NonSerialized] public readonly float playerHeight = 2f;
    #endregion  

    #region Raycast Info
    public readonly float verticalRaycastDist = 1.1f;
    public readonly float horizontalRaycastDist = .51f;
    private bool grounded;
    private bool wallToLeft;
    private bool wallToRight;
    [NonSerialized] public bool wallInSomeDirection;
    public RaycastHit groundHit;
    public RaycastHit leftWallHit;
    public RaycastHit rightWallHit;
    #endregion

    #region States
    private IState currentState;
    public IState exitingState;
    public IState nextState;
    public IdleState idleState;
    public GroundedMovingState groundedMovingState;
    public AirborneState airborneState;
    public JumpState jumpState;
    public LeftWallrunState leftWallrunState;
    public RightWallrunState rightWallrunState;
    public SlideState slideState;
    #endregion


    void Awake()
    {
        actionReferences = new() { move, sprint, jump, leftWallrun, rightWallrun, slide };

        // Create all state instances once, then swap between them
        idleState = new IdleState(this);
        groundedMovingState = new GroundedMovingState(this);
        airborneState = new AirborneState(this);
        jumpState = new JumpState(this);
        leftWallrunState = new LeftWallrunState(this);
        rightWallrunState = new RightWallrunState(this);
        slideState = new SlideState(this);
    }


    void Start()
    {
        currentState = idleState;
        exitingState = currentState;
        rb.useGravity = false; // we'll use our false playerGravity instead, toggling it with useCustomGravity
        useCustomGravity = true;
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
    }


    // Update is called once per frame
    void Update()
    {
        ApplyGeneralActions();

        // Moving
        moveDirection = Vector3.ProjectOnPlane(inputMoveDirection.x * transform.right + inputMoveDirection.y * transform.forward, Vector3.up).normalized;
        isMoving = inputMoveDirection != Vector2.zero;
        if (pressedSprint && grounded) isSprinting = true;
        if (!pressedSprint) isSprinting = false;

        // Sliding
        if (isSliding && (!slideTimerOngoing || !pressedSlide)) slideStopTriggered = true;
        
        // Jumping
        jumpTriggered = pressedJump;

        // Wallrunning
        leftWallrunStartTriggered = !isLeftWallrunningIsBuffered && pressedLeftWallrun && wallToLeft;
        if (isLeftWallrunning && (!pressedLeftWallrun || !wallToLeft)) leftWallrunStopTriggered = true;
        rightWallrunStartTriggered = !isRightWallrunningIsBuffered && pressedRightWallrun && wallToRight;
        if (isRightWallrunning && !(pressedRightWallrun && wallToRight)) rightWallrunStopTriggered = true;

        // Debug.Log("Update: " + currentState);
        nextState = DetermineNextState();
        if (nextState != currentState) ChangeState(nextState);
    }


    void ChangeState(IState nextState)
    {
        exitingState = currentState;
        exitingState.OnExit();
        currentState = nextState;
        currentState.OnEnter();
    }


    void FixedUpdate()
    {
        DrawRaycasts();
        // Debug.Log("FixedUpdate: " + currentState);
        currentState.Apply();

        ApplyPhysicsActions();
    }


    // Update actions that take place independent of states (like cooldowns)
    void ApplyGeneralActions()
    {
        UpdateCooldowns();
    }


    void UpdateCooldowns()
    {
        moveRightLocked = TickTimer(ref moveRightInputLockTime);
        moveLeftLocked = TickTimer(ref moveLeftInputLockTime);
        isLeftWallrunningIsBuffered = TickTimer(ref isLeftWallrunningBufferTime);
        isRightWallrunningIsBuffered = TickTimer(ref isRightWallrunningBufferTime);
        cameraSmoothingEnabled = TickTimer(ref cameraSmoothingEnableTime);
        slideTimerOngoing = TickTimer(ref slideTime);
    }


    bool TickTimer(ref float timer)
    {
        timer -= Time.deltaTime;
        return timer >= 0;
    }


    // FixedUpdate physics actions that take place independent of states
    void ApplyPhysicsActions()
    {
        ApplyExtraGravity();
        ApplyWallrunRotation();
        ApplySlideRotation();
        CapSpeed();
    }


    void ApplyExtraGravity()
    {
        float gravity = useWallJumpGravity ? wallJumpGravity : playerGravity;
        // Debug.Log(gravity);
        if (useCustomGravity) rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }


    void ApplyWallrunRotation()
    {
        // use rb idk man
        Quaternion targetRotation;
        if (isLeftWallrunning)
        {
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -WallrunAngle);
        }
        else if (isRightWallrunning)
        {
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, WallrunAngle);
        }
        else
        {
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, WallrunRotationSpeed * Time.deltaTime);
        // rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, WallrunRotationSpeed * Time.deltaTime));
    }


    void ApplySlideRotation()
    {
        Quaternion targetRotation;
        // Rotate back to normal
        if (!isSliding)
        {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, slideRotationSpeed * Time.deltaTime);
            return;
        }

        // Rotate slideAngle degrees when sliding
        else
        {
            targetRotation = Quaternion.Euler(-slideAngle, transform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, slideRotationSpeed * Time.deltaTime);
        }
    }


    void CapSpeed()
    {
        float speed = isSprinting ? sprintSpeed : normalSpeed;
        // if (inAir) speed *= airMovementMultiplier;
        Vector3 flatVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > speed) rb.linearVelocity = flatVelocity.normalized * speed + new Vector3(0, rb.linearVelocity.y, 0);
    }


    IState DetermineNextState()
    {
        // Take the current state, and decide what the next state should be
        switch (currentState)
        {
            case IdleState:
                if (jumpTriggered) return jumpState;
                else if (isMoving) return groundedMovingState;
                else return idleState;
            case GroundedMovingState:
                if (jumpTriggered) return jumpState;
                else if (inAir) return airborneState;
                else if (pressedSlide) return slideState;
                else if (isMoving) return groundedMovingState;
                else return idleState;
            case SlideState:
                if (jumpTriggered) return jumpState;
                else if (slideStopTriggered && isMoving) return groundedMovingState;
                else if (slideStopTriggered) return idleState;
                else return slideState;
            case JumpState:
                if (jumpApplied) return airborneState;
                else return jumpState;
            case AirborneState:
                if (leftWallrunStartTriggered) return leftWallrunState;
                else if (rightWallrunStartTriggered) return rightWallrunState;
                else if (inAir) return airborneState;
                else if (isMoving) return groundedMovingState;
                else return idleState;
            case LeftWallrunState:
                if (jumpTriggered) return jumpState;
                else if (leftWallrunStopTriggered) return airborneState;
                else return leftWallrunState;
            case RightWallrunState:
                if (jumpTriggered) return jumpState;
                else if (rightWallrunStopTriggered) return airborneState;
                else return rightWallrunState;
        }
        return null; // won't logically happen but it wanted a return path
    }


    void DrawRaycasts()
    {
        // actual raycasts (not debug ones)
        int layerMask = ~LayerMask.GetMask("IgnoreRaycast"); // selects everything EXCEPT IgnoreRaycast layer, thus ignoring those objects
        grounded = Physics.Raycast(rb.transform.position, Vector3.down, out groundHit, verticalRaycastDist);
        // grounded = Physics.CheckCapsule(feet.position, feet.position + Vector3.up * 0.2f, 1);

        // Radial raycast search for walls in any direction, with a threshold for a valid leftwards or rightwards wall
        float rays = 16;
        wallToLeft = false;
        wallToRight = false;
        wallInSomeDirection = false;
        Ray leftRay = new(); // initalized will 0s, gets populated if wallToLeft is true, which is the only time we use it anyway
        Ray rightRay = new();
        for (int i = 0; i < rays; i++)
        {
            float angle = (i / rays) * Mathf.PI * 2f;
            Vector3 dir = new(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 playerDir = transform.TransformDirection(dir); // dir follows player rotation
            Ray ray = new(feet.position, playerDir);
            if (Physics.Raycast(ray, out RaycastHit wallHit, horizontalRaycastDist, layerMask, QueryTriggerInteraction.Ignore))
            {
                wallInSomeDirection = true;
                wallDirection = Vector3.ProjectOnPlane(-wallHit.normal, Vector3.up).normalized;

                bool isRightSlice = i <= 2 || i >= 14;
                bool isLeftSlice = i >= 5 && i <= 10;
                if (isLeftSlice)
                {
                    wallToLeft = true;
                    leftWallHit = wallHit;
                    leftRay = ray;
                }
                else if (isRightSlice)
                {
                    wallToRight = true;
                    rightWallHit = wallHit;
                    rightRay = ray;
                }
                // break;
            }
        }

        // grounded raycasts
        inAir = !grounded;
        if (grounded)
        {
            Debug.DrawRay(rb.transform.position, Vector3.down * verticalRaycastDist, Color.green);
        }
        else
        {
            Debug.DrawRay(rb.transform.position, Vector3.down * verticalRaycastDist, Color.red);
        }

        // left wall raycasts
        if (wallToLeft)
        {
            Debug.DrawRay(leftRay.origin, leftRay.direction, Color.green);
        }
        else
        {
            Debug.DrawRay(leftRay.origin, leftRay.direction, Color.red);
        }

        // right wall raycasts
        if (wallToRight)
        {
            Debug.DrawRay(rightRay.origin, rightRay.direction, Color.green);
        }
        else
        {
            Debug.DrawRay(rightRay.origin, rightRay.direction, Color.red);
        }
    }


    void PerformMovement(InputAction.CallbackContext ctx)
    {
        inputMoveDirection = ctx.ReadValue<Vector2>();
    }


    void StartSprint(InputAction.CallbackContext ctx)
    {
        pressedSprint = true;
    }


    void CancelSprint(InputAction.CallbackContext ctx)
    {
        pressedSprint = false;
    }


    void StartJump(InputAction.CallbackContext ctx)
    {
        pressedJump = true;
    }


    void CancelJump(InputAction.CallbackContext ctx)
    {
        pressedJump = false;
    }


    void StartRightWallrun(InputAction.CallbackContext ctx)
    {
        pressedRightWallrun = true;
    }


    void CancelRightWallrun(InputAction.CallbackContext ctx)
    {
        pressedRightWallrun = false;
    }


    void StartLeftWallrun(InputAction.CallbackContext ctx)
    {
        pressedLeftWallrun = true;
    }


    void CancelLeftWallrun(InputAction.CallbackContext ctx)
    {
        pressedLeftWallrun = false;
    }
    

    void StartSlide(InputAction.CallbackContext ctx)
    {
        pressedSlide = true;
    }


    void CancelSlide(InputAction.CallbackContext ctx)
    {
        pressedSlide = false;
    }
}
