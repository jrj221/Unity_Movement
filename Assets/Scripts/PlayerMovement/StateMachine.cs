using System;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    #region Object References
    [Header("Object References")]
    public Rigidbody rb;
    public Transform feet;
    [SerializeField] private State[] _states;
    #endregion

    #region Movement
    [Header("Movement")]
    public float sprintSpeed;
    public float normalSpeed;
    public float groundDrag;
    public float airMovementMultiplier;
    [NonSerialized] public Vector3 moveDirection;
    [NonSerialized] public bool isSprinting;
    [NonSerialized] public bool isMoving;
    #endregion

    #region Wallrunning
    [Header("Wallrunning")]
    public float wallrunRotationSpeed;
    public float wallrunAngle;
    public float pushIntoWallForce;
    public float moveLeftInputLockLength;
    public float moveRightInputLockLength;
    public float isLeftWallrunningBufferLength;
    public float isRightWallrunningBufferLength;
    public float wallrunBufferLength;
    [NonSerialized] public float wallrunBufferTime;
    private bool _wallrunBuffered;
    [NonSerialized] public bool isLeftWallrunning;
    [NonSerialized] public bool isRightWallrunning;
    private bool _leftWallrunStartTriggered;
    private bool _rightWallrunStartTriggered;
    [NonSerialized] public bool leftWallrunStopTriggered;
    [NonSerialized] public bool rightWallrunStopTriggered;
    [NonSerialized] public float moveRightInputLockTime = 0;
    [NonSerialized] public float moveLeftInputLockTime = 0;
    private bool _moveRightLocked;
    private bool _moveLeftLocked;
    [NonSerialized] public float isLeftWallrunningBufferTime = 0;
    [NonSerialized] public float isRightWallrunningBufferTime = 0;
    private bool _isLeftWallrunningIsBuffered;
    private bool _isRightWallrunningIsBuffered;
    #endregion

    #region Jumping
    [Header("Jumping")]
    public float jumpForce;
    public float wallVerticalJumpForce;
    public float wallSideJumpForce;
    public float jumpBufferTimeLength;
    [NonSerialized] public float jumpBufferTime;
    [NonSerialized] public bool jumpBuffered;
    private bool _jumpTriggered;
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
    [NonSerialized] private bool _slideTimerOngoing;
    [NonSerialized] public bool slideStopTriggered;
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
    [NonSerialized] public Vector3 wallDirection;
    [NonSerialized] public readonly float playerRadius = 0.5f;
    [NonSerialized] public readonly float playerHeight = 2f;
    #endregion

    #region Raycast Info
    private const float VerticalRaycastDist = 1.1f;
    private const float HorizontalRaycastDist = .51f;
    [NonSerialized] public int ignoreRaycastLayerMask; // selects everything EXCEPT IgnoreRaycast layer, thus ignoring those objects
    private bool _grounded;
    private bool _wallToLeft;
    private bool _wallToRight;
    [NonSerialized] public bool wallInSomeDirection;
    public RaycastHit groundHit;
    public RaycastHit leftWallHit;
    public RaycastHit rightWallHit;
    #endregion

    #region States
    private State _currentState;
    public State exitingState;
    private State _nextState;
    private IdleState _idleState;
    public GroundedMovingState groundedMovingState;
    private AirborneState _airborneState;
    private JumpState _jumpState;
    public LeftWallrunState leftWallrunState;
    public RightWallrunState rightWallrunState;
    public SlideState slideState;
    #endregion


    private void Awake()
    {
        foreach (State state in _states)
        {
            state.Initialize(this);
        }
        // Create all state instances once, then swap between them
        _idleState = GetComponent<IdleState>();
        groundedMovingState = GetComponent<GroundedMovingState>();
        _airborneState = GetComponent<AirborneState>();
        _jumpState = GetComponent<JumpState>();
        leftWallrunState = GetComponent<LeftWallrunState>();
        rightWallrunState = GetComponent<RightWallrunState>();
        slideState = GetComponent<SlideState>();
        
        ignoreRaycastLayerMask = ~LayerMask.GetMask("Ignore Raycast");
    }


    private void Start()
    {
        _currentState = _idleState;
        exitingState = _currentState;
        rb.useGravity = false; // we'll use our false playerGravity instead, toggling it with useCustomGravity
        useCustomGravity = true;
    }


    // Update is called once per frame
    private void Update()
    {
        // Time.timeScale = 0.1f;
        ApplyGeneralActions();

        // Moving
        moveDirection = Vector3.ProjectOnPlane(InputManager.Instance.InputMoveDirection.x * transform.right + InputManager.Instance.InputMoveDirection.y * transform.forward, Vector3.up).normalized;
        isMoving = InputManager.Instance.InputMoveDirection != Vector2.zero;
        if (InputManager.Instance.PressedSprint && _grounded) isSprinting = true;
        if (!InputManager.Instance.PressedSprint) isSprinting = false;
        // rbCollider.material = _grounded ? null : frictionless;

        // Sliding
        if (isSliding && (!_slideTimerOngoing || !InputManager.Instance.PressedSlide)) slideStopTriggered = true;

        // Jumping
        if (InputManager.Instance.PressedJump) jumpBufferTime = jumpBufferTimeLength;
        _jumpTriggered = jumpBuffered;

        // Wallrunning
        _leftWallrunStartTriggered = !_wallrunBuffered && !_isLeftWallrunningIsBuffered && InputManager.Instance.PressedLeftWallrun && _wallToLeft;
        if (isLeftWallrunning && (!InputManager.Instance.PressedLeftWallrun || !_wallToLeft)) leftWallrunStopTriggered = true;
        _rightWallrunStartTriggered = !_wallrunBuffered && !_isRightWallrunningIsBuffered && InputManager.Instance.PressedRightWallrun && _wallToRight;
        if (isRightWallrunning && !(InputManager.Instance.PressedRightWallrun && _wallToRight)) rightWallrunStopTriggered = true;

        // Debug.Log("Update: " + _currentState);
        _nextState = DetermineNextState();
        if (_nextState != _currentState) ChangeState(_nextState);
    }


    private void ChangeState(State nextState)
    {
        exitingState = _currentState;
        exitingState.OnExit();
        _currentState = nextState;
        _currentState.OnEnter();
    }


    private void FixedUpdate()
    {
        DrawRaycasts();
        // Debug.Log("FixedUpdate: " + _currentState);
        _currentState.Apply();

        ApplyPhysicsActions();
    }


    // Update actions that take place independent of states (like cooldowns)
    private void ApplyGeneralActions()
    {
        UpdateCooldowns();
    }


    private void UpdateCooldowns()
    {
        _moveRightLocked = TickTimer(ref moveRightInputLockTime);
        _moveLeftLocked = TickTimer(ref moveLeftInputLockTime);
        _isLeftWallrunningIsBuffered = TickTimer(ref isLeftWallrunningBufferTime);
        _isRightWallrunningIsBuffered = TickTimer(ref isRightWallrunningBufferTime);
        cameraSmoothingEnabled = TickTimer(ref cameraSmoothingEnableTime);
        _slideTimerOngoing = TickTimer(ref slideTime);
        jumpBuffered = TickTimer(ref jumpBufferTime);
        _wallrunBuffered = TickTimer(ref wallrunBufferTime);
    }


    private bool TickTimer(ref float timer)
    {
        timer -= Time.deltaTime;
        return timer >= 0;
    }


    // FixedUpdate physics actions that take place independent of states
    private void ApplyPhysicsActions()
    {
        ApplyExtraGravity();
        ApplyWallrunRotation();
        ApplySlideRotation();
        CapSpeed();
    }


    private void ApplyExtraGravity()
    {
        float gravity = useWallJumpGravity ? wallJumpGravity : playerGravity;
        // Debug.Log(gravity);
        if (useCustomGravity) rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }


    private void ApplyWallrunRotation()
    {
        // use rb IDK man
        Quaternion targetRotation;
        if (isLeftWallrunning)
        {
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -wallrunAngle);
        }
        else if (isRightWallrunning)
        {
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, wallrunAngle);
        }
        else
        {
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, wallrunRotationSpeed * Time.deltaTime);
        // rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, wallrunRotationSpeed * Time.deltaTime));
    }


    private void ApplySlideRotation()
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


    private void CapSpeed()
    {
        float speed = isSprinting ? sprintSpeed : normalSpeed;
        if (inAir) speed *= airMovementMultiplier;
        Vector3 flatVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > speed) rb.linearVelocity = flatVelocity.normalized * speed + new Vector3(0, rb.linearVelocity.y, 0);
    }


    private State DetermineNextState()
    {
        // Take the current state, and decide what the next state should be
        switch (_currentState)
        {
            case IdleState:
                if (_jumpTriggered) return _jumpState;
                else if (isMoving) return groundedMovingState;
                else return _idleState;
            case GroundedMovingState:
                if (_jumpTriggered) return _jumpState;
                else if (inAir) return _airborneState;
                else if (InputManager.Instance.PressedSlide) return slideState;
                else if (isMoving) return groundedMovingState;
                else return _idleState;
            case SlideState:
                if (_jumpTriggered) return _jumpState;
                else if (slideStopTriggered && isMoving) return groundedMovingState;
                else if (slideStopTriggered) return _idleState;
                else return slideState;
            case JumpState:
                if (jumpApplied) return _airborneState;
                else return _jumpState;
            case AirborneState:
                if (_leftWallrunStartTriggered) return leftWallrunState;
                else if (_rightWallrunStartTriggered) return rightWallrunState;
                else if (inAir) return _airborneState;
                else if (isMoving) return groundedMovingState;
                else return _idleState;
            case LeftWallrunState:
                if (_jumpTriggered) return _jumpState;
                else if (leftWallrunStopTriggered) return _airborneState;
                else return leftWallrunState;
            case RightWallrunState:
                if (_jumpTriggered) return _jumpState;
                else if (rightWallrunStopTriggered) return _airborneState;
                else return rightWallrunState;
        }
        return null; // won't logically happen but it wanted a return path
    }


    private void DrawRaycasts()
    {
        // actual raycasts (not debug ones)
        _grounded = Physics.Raycast(rb.transform.position, Vector3.down, out groundHit, VerticalRaycastDist);

        // Radial raycast search for walls in any direction, with a threshold for a valid leftwards or rightwards wall
        float rays = 16;
        _wallToLeft = false;
        _wallToRight = false;
        wallInSomeDirection = false;
        Ray leftRay = new(); // initalized will 0s, gets populated if _wallToLeft is true, which is the only time we use it anyway
        Ray rightRay = new();
        for (int i = 0; i < rays; i++)
        {
            float angle = (i / rays) * Mathf.PI * 2f;
            Vector3 dir = new(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 playerDir = transform.TransformDirection(dir); // dir follows player rotation
            Ray ray = new(feet.position, playerDir);
            if (Physics.Raycast(ray, out RaycastHit wallHit, HorizontalRaycastDist, ignoreRaycastLayerMask, QueryTriggerInteraction.Ignore))
            {
                wallInSomeDirection = true;
                wallDirection = Vector3.ProjectOnPlane(-wallHit.normal, Vector3.up).normalized;

                bool isRightSlice = i <= 2 || i >= 14;
                bool isLeftSlice = i >= 5 && i <= 10;
                if (isLeftSlice)
                {
                    _wallToLeft = true;
                    leftWallHit = wallHit;
                    leftRay = ray;
                }
                else if (isRightSlice)
                {
                    _wallToRight = true;
                    rightWallHit = wallHit;
                    rightRay = ray;
                }
                // break;
            }
        }

        // _grounded raycasts
        inAir = !_grounded;
        if (_grounded)
        {
            Debug.DrawRay(rb.transform.position, Vector3.down * VerticalRaycastDist, Color.green);
        }
        else
        {
            Debug.DrawRay(rb.transform.position, Vector3.down * VerticalRaycastDist, Color.red);
        }

        // left wall raycasts
        if (_wallToLeft)
        {
            Debug.DrawRay(leftRay.origin, leftRay.direction, Color.green);
        }
        else
        {
            Debug.DrawRay(leftRay.origin, leftRay.direction, Color.red);
        }

        // right wall raycasts
        if (_wallToRight)
        {
            Debug.DrawRay(rightRay.origin, rightRay.direction, Color.green);
        }
        else
        {
            Debug.DrawRay(rightRay.origin, rightRay.direction, Color.red);
        }
    }
}
