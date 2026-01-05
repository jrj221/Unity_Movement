using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovementController : MonoBehaviour
{
    [Header("Objects and Action References")]
    public Rigidbody rb;
    public Transform feet;
    public InputActionReference move;
    public InputActionReference sprint;
    public InputActionReference jump;
    public InputActionReference rightWallrun;
    public InputActionReference leftWallrun;
    public InputActionReference slide;

    [Header("Movement")]
    public float sprintSpeed;
    public float normalSpeed;
    public float groundDrag;
    public float airMovementMultiplier;
    public float inAirGravityMultiplier;

    [Header("Wallrunning")]
    public float wallrunRotationSpeed;
    public float wallrunAngle;
    public float moveLeftInputLockLength;
    public float moveRightInputLockLength;

    [Header("Jumping")]
    public float jumpForce;
    public float wallVerticalJumpForce;
    public float wallSideJumpForce;
    public float slideJumpHorizontalForce;
    public float groundedJumpBufferLength;
    
    [Header("Sliding")]
    public float slideSlowdownSpeed;
    public float baseSlideSlowdownFactor;
    public float minSlideSlowdownFactor;
    public float slideRotationSpeed;
    public float slideAngle;
    public float slideSlowdownLockLength;

    [Header("Stairs")]
    public float minStepLength;
    public float maxStepHeight;
    public float maxStepSlope;

    [Header("Slopes")]
    public float maxSlopeAngle;
    public float stickToSlopeForce;

    // Action Values
    private List<InputActionReference> actionReferences;
    private Vector2 inputMoveDirection;
    private Vector3 wallDirection;
    private bool pressedSprint;
    private bool isSprinting;
    private bool pressedJump;
    private bool canJump;
    private bool inJump;
    private bool pressedLeftWallrun;
    private bool pressedRightWallrun;
    private bool isLeftWallRunning;
    private bool isRightWallRunning;
    private float moveRightInputLockTime = 0;
    private bool moveRightLocked;
    private float moveLeftInputLockTime = 0;
    private bool moveLeftLocked;
    private bool inAir;
    private enum JumpType
    {
        None, 
        Normal,
        RightWallrunning,
        LeftWallrunning,
        Slide,
    }
    private JumpType jumpType;
    private bool groundedJumpBufferActive;
    private float groundedJumpBufferTime;
    private enum LastSurface
    {
        Ground,
        RightWall,
        LeftWall,
    }
    private LastSurface lastSurface;
    private bool pressedSlide;
    private bool isSliding;
    private float slideSlowdownFactor;
    private float slideSlowdownLockTime = 0;
    private bool slideSlowdownLocked;
    private Vector3 stairUpPosition;
    private Vector3 stairDownPosition;
    private readonly float playerRadius = 0.5f;
    private readonly float playerHeight = 2f;
    [HideInInspector]
    public bool forceMeshSnap;

    // Raycast Info
    private bool grounded;
    private readonly float verticalRaycastDist = 1.1f;
    private readonly float horizontalRaycastDist = .6f;
    private bool wallToLeft;
    private bool wallToRight;
    private bool wallInSomeDirection;
    private RaycastHit groundHit;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
 
    
    void Awake()
    {
        actionReferences = new() {move, sprint, jump, leftWallrun, rightWallrun, slide};
    }


    void Start()
    {
        slideSlowdownFactor = baseSlideSlowdownFactor;
    }


    // OnEnable is called when the object tied to the script instance is enabled. OnDisable is the opposite
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


    void Update()
    {
        UpdateCooldowns();
        CheckMovement();
    }


    void FixedUpdate()
    {
        DrawRaycasts();
        ApplyMovement();  
    }


    // Called during FixedUpdate to apply any needed physics updates
    void ApplyMovement()
    {
        ApplyWallrunning();
        if (!(isLeftWallRunning || isRightWallRunning)) ApplyHorizontalMovement(); // since wallrunning has it's own movement we shouldn't override
        ApplyStairMovement();
        ApplyJump();
        ApplySlide();
        CapSpeed();
    }


    void CapSpeed()
    {
        float speed = isSprinting ? sprintSpeed : normalSpeed;
        if (inAir) speed *= airMovementMultiplier;
        Vector3 flatVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > speed) rb.linearVelocity = flatVelocity.normalized * speed + new Vector3(0, rb.linearVelocity.y, 0);
    }


    void ApplyStairMovement()
    {
        // Movement checking is normally done in Update, but since stair movement requires up to date PHYSICS information, 
        // it needs to be done in FixedUpdate where physics are updated
        if (CheckUpStairs()) rb.MovePosition(stairUpPosition);
        else if (CheckDownStairs()) rb.MovePosition(stairDownPosition);
    }


    bool CheckUpStairs()
    {
        if (!wallInSomeDirection) return false;
        if (!IsMovingTowardsStair()) return false; // (otherwise it might trigger when going down stairs)
        
        // Get information about the height of the step
        Vector3 horizontalStepOffset = wallDirection * (minStepLength + playerRadius); // how far from player to check
        Vector3 verticalStepOffset = new(0, maxStepHeight + playerHeight, 0); // how far to check above step, making sure there's room for the capsule after snapping
        // Debug.DrawRay(feet.position + (wallDirection * (minStepLength + playerRadius)) + new Vector3(0, maxStepHeight + playerHeight, 0), Vector3.down * (playerHeight + maxStepHeight), Color.orange);
        Physics.Raycast(feet.position + horizontalStepOffset + verticalStepOffset, Vector3.down, out RaycastHit heightHit, playerHeight + maxStepHeight);
        
        if (!ValidStepSlopeClearance(heightHit)) return false;
        float stepHeight = heightHit.point.y - feet.position.y;
        if (!ValidStepHeight(stepHeight)) return false;
        if (!ValidStepLength(stepHeight)) return false;

        // Success! You can go up the step
        Vector3 movingDirection = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up).normalized;
        Vector3 amountToMoveHorizontally = movingDirection * minStepLength;
        Vector3 amountToMoveVertically = Vector3.up * stepHeight;
        stairUpPosition = transform.position + amountToMoveHorizontally + amountToMoveVertically;
        return true;
    }


    bool IsMovingTowardsStair()
    {
        return Vector3.Dot(wallDirection, rb.linearVelocity.normalized) > 0;
    }

    
    bool ValidStepSlopeClearance(RaycastHit raycastHit)
    {
        return raycastHit.normal.y > maxStepSlope; // otherwise too sloped to be a step, perhaps it's a ramp
    }


    bool ValidStepHeight(float stepHeight)
    {
        // if there was a low ceiling, stepHeight would appear to be bigger that it actually is, which is okay to fail since we wouldn't want to make the step anyway
        return stepHeight <= maxStepHeight; // valid step
    }


    bool ValidStepLength(float stepHeight)
    {
        // raycast forward to see if the step is long enough. We want it to be false since that means the step is long enough
        // Debug.DrawRay(feet.position + new Vector3(0, stepHeight + 0.01f, 0), wallDirection * minStepLength, Color.purple);
        return !Physics.Raycast(feet.position + new Vector3(0, stepHeight + 0.01f, 0), wallDirection, minStepLength);
    }


    bool CheckDownStairs()
    {
        if (grounded) return false;
        if (inJump) return false;
        if (!ValidDownwardStep(out RaycastHit downHit)) return false; // a step vs a drop below you (like walking off a cliff)
        if (!ValidStepSlopeClearance(downHit)) return false;

        // Success! You can move down a step
        Vector3 amountToMoveVertically = Vector3.down * (feet.position.y - downHit.point.y);
        stairDownPosition = transform.position + amountToMoveVertically;
        return true;
    }


    bool ValidDownwardStep(out RaycastHit downHit)
    {
        // feet are slightly elevated above capsule, so this is slightly off of maxStepHeight
        return Physics.Raycast(feet.position, Vector3.down, out downHit, maxStepHeight);
    }


    void ApplyWallrunning()
    {
        if (isLeftWallRunning || isRightWallRunning)
        {
            PerformWallrun();
        } 
        ApplyWallrunRotation(); // gets called even if you aren't wall running in case you need to rotate back to normal
    }


    void ApplyWallrunRotation()
    {
        Quaternion targetRotation;
        if (isLeftWallRunning)
        {
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -wallrunAngle);
            
        } else if (isRightWallRunning)
        {
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, wallrunAngle);
        }
        else
        {
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, wallrunRotationSpeed * Time.deltaTime);
    }


    void ApplyHorizontalMovement()
    {
        // Apply drag
        rb.linearDamping = inAir ? 0 : groundDrag;

        // Zero out left/right input if we have recently jumped off a wall
        if (moveRightLocked && inputMoveDirection.x > 0) inputMoveDirection.x = 0;
        if (moveLeftLocked && inputMoveDirection.x < 0) inputMoveDirection.x = 0;  

        // Determine the direction the player is moving in (world space) and their speed
        float speed = isSprinting ? sprintSpeed : normalSpeed;
        if (inAir) speed *= airMovementMultiplier;
        Vector3 moveDirection = Vector3.ProjectOnPlane(inputMoveDirection.x * transform.right + inputMoveDirection.y * transform.forward, Vector3.up).normalized;
        Debug.Log(moveDirection);

        if (OnSlope()) 
        {
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundHit.normal);
            rb.useGravity = false;
            if (rb.linearVelocity.y > 0) rb.AddForce(Vector3.down * stickToSlopeForce); // if going up slopes
            rb.AddForce(10f * speed * moveDirection);
        } else if (inAir)
        {
            float gravity = 9.81f;
            rb.useGravity = false;
            rb.AddForce(10f * speed * moveDirection);
            rb.AddForce(gravity * inAirGravityMultiplier * Vector3.down, ForceMode.Acceleration); // additional gravity to tighten jump
        } else // normal ground
        {
            rb.useGravity = true;
            rb.AddForce(10f * speed * moveDirection);
        }
        Debug.DrawRay(transform.position, moveDirection, Color.blue);
    }


    bool OnSlope()
    {
        if (grounded)
        {
            float slopeAngle = Vector3.Angle(Vector3.up, groundHit.normal);
            return slopeAngle <= maxSlopeAngle && slopeAngle != 0; // what happens if it's greater?
        }
        return false;
    }


    void ApplyJump()
    {
        if (jumpType == JumpType.None) return; // no jump to apply

        canJump = false;
        pressedJump = false; // it should only be true for the single frame it was pressed

        // Eliminate any current y velocity to prevent weird jumps on slopes
        rb.linearVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // Start a buffer where canJump cannot be reset by grounded in the few frames until the next physics update
        groundedJumpBufferTime = groundedJumpBufferLength;

        switch (jumpType)
        {
            case JumpType.Normal:
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                break;
            case JumpType.LeftWallrunning:
                CancelLeftWallrun(new InputAction.CallbackContext());
                rb.AddForce(Vector3.up * wallVerticalJumpForce + leftWallHit.normal * wallSideJumpForce, ForceMode.Impulse); 
                moveLeftInputLockTime = moveLeftInputLockLength;
                break;
            case JumpType.RightWallrunning:
                CancelRightWallrun(new InputAction.CallbackContext());
                rb.AddForce(Vector3.up * wallVerticalJumpForce + rightWallHit.normal * wallSideJumpForce, ForceMode.Impulse);
                moveRightInputLockTime = moveRightInputLockLength;
                break;
            case JumpType.Slide:
                isSliding = false;
                CancelSlide(new InputAction.CallbackContext());
                rb.AddForce(Vector3.up * jumpForce + Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * slideJumpHorizontalForce, ForceMode.Impulse);
                break;
            default:
                Debug.Log("ApplyJump switch statement defaulted");
                break;
        }
        jumpType = JumpType.None; // so future frames don't repeat the jump
    }


    void ApplySlide()
    {
        if (isSliding)
        {
            PerformSlide();
        }
        ApplySlideRotation(); // apply whether or not you are sliding so you still rotate the mesh 
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


        // Rotate slideAngle degrees when wallrunning
        else
        {
            targetRotation = Quaternion.Euler(-slideAngle, transform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, slideRotationSpeed * Time.deltaTime);
        }
    }


    // called during Update to see what physics changes should be made the next time FixedUpdate (and ApplyMovement) are called
    void CheckMovement()
    {
        CheckIfJumpShouldBeEnabled();
        CheckIfSprintShouldBeEnabled();
        CheckIfWallrunShouldBeEnabled();
        CheckIfSlideShouldBeEnabled();
    }


    void CheckIfJumpShouldBeEnabled()
    {
        // wasn't sure where else to put this logic
        if (rb.linearVelocity.y > 0) inJump = true;
        else if (grounded) inJump = false;


        jumpType = JumpType.None;
        if (!canJump) return;
        if (pressedJump)
        {
            if (isLeftWallRunning)
            {
                jumpType = JumpType.LeftWallrunning;
            } else if (isRightWallRunning)
            {
                jumpType = JumpType.RightWallrunning;
            } else if (isSliding)
            {
                jumpType = JumpType.Slide;
            } else
            {
                jumpType = JumpType.Normal;
            }
        }
    }


    void CheckIfSprintShouldBeEnabled()
    {
        if (pressedSprint && grounded && !isSliding)
        {
            isSprinting = true;
        } else if (!pressedSprint || isSliding)
        {
            isSprinting = false;
        }
    }


    void CheckIfWallrunShouldBeEnabled()
    {
        if (!isLeftWallRunning && pressedLeftWallrun && wallToLeft && !grounded)
        {
            rb.constraints |= RigidbodyConstraints.FreezePositionY;
            rb.useGravity = false;
            isLeftWallRunning = true;
        } else if (!isRightWallRunning && pressedRightWallrun && wallToRight && !grounded)
        {
            rb.constraints |= RigidbodyConstraints.FreezePositionY;
            rb.useGravity = false;
            isRightWallRunning = true;
        }
        inAir = !(wallToLeft || wallToRight || grounded);
    }


    void CheckIfSlideShouldBeEnabled()
    {
        if (pressedSlide && grounded)
        {
            isSliding = true;
        } else if (!pressedSlide) // this makes it so you have to be grounded to start, but not during the slide since the raycast wouldn't work.
        // this isn't ideal, how can we change it so that when you jump or are in the air, sliding stops?
        {
            isSliding = false;
        }
    }


    void UpdateCooldowns()
    {
        moveRightLocked = TickTimer(ref moveRightInputLockTime);
        moveLeftLocked = TickTimer(ref moveLeftInputLockTime);
        slideSlowdownLocked = TickTimer(ref slideSlowdownLockTime);
        groundedJumpBufferActive = TickTimer(ref groundedJumpBufferTime);
    }


    bool TickTimer(ref float timer)
    {
        timer -= Time.deltaTime;
        return timer >= 0;
    }


    void DrawRaycasts()
    {
        // actual raycasts (not debug ones)
        int layerMask = ~LayerMask.GetMask("IgnoreRaycast"); // selects everything EXCEPT IgnoreRaycast layer, thus ignoring those objects
        grounded = Physics.Raycast(rb.transform.position, Vector3.down, out groundHit, verticalRaycastDist);

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
                } else if (isRightSlice)
                {
                    wallToRight = true;
                    rightWallHit = wallHit;
                    rightRay = ray;
                }
                // break;
            } 
        }

        // grounded raycasts
        if (grounded) {
            lastSurface = LastSurface.Ground;
            Debug.DrawRay(rb.transform.position, Vector3.down * verticalRaycastDist, Color.green);
            if (!groundedJumpBufferActive) canJump = true; // to prevent it turning back on in the few frames between a jump press and a physics update
        } else
        {
            Debug.DrawRay(rb.transform.position, Vector3.down * verticalRaycastDist, Color.red);
        }

        // left wall raycasts
        if (wallToLeft) {  
            Debug.DrawRay(leftRay.origin, leftRay.direction, Color.green);
        } else
        {
            Debug.DrawRay(leftRay.origin, leftRay.direction, Color.red);
        }

        // right wall raycasts
        if (wallToRight) {  
            Debug.DrawRay(rightRay.origin, rightRay.direction, Color.green);
        } else
        {
            Debug.DrawRay(rightRay.origin, rightRay.direction, Color.red);
        }
    }


    // Action Event Handlers
    void StartRightWallrun(InputAction.CallbackContext ctx)
    {
        pressedRightWallrun = true;
    }


    void PerformWallrun()
    {
        canJump = true;
        // If player turns too much, wallforward doesn't work since the hit.normal is invalid, so we end the wallrun if they turn too far
        if (isLeftWallRunning && !wallToLeft) 
        {
            CancelLeftWallrun(new InputAction.CallbackContext());
            return;
        } else if (isRightWallRunning && !wallToRight)
        {
            CancelRightWallrun(new InputAction.CallbackContext());
            return;
        }

        // Add force to move alongside the wall
        RaycastHit hit = isLeftWallRunning ? leftWallHit : rightWallHit;
        Vector3 wallForward = Vector3.Cross(hit.normal, Vector3.up);
        if (isRightWallRunning) wallForward = -wallForward; // reversed due to right hand rule
        rb.AddForce(10f * normalSpeed * wallForward.normalized);
    }


    void CancelRightWallrun(InputAction.CallbackContext ctx)
    {
        pressedRightWallrun = false;
        if (isRightWallRunning)
        {
            lastSurface = LastSurface.RightWall;
            isRightWallRunning = false;
            rb.useGravity = true;
            rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
        }
    }


    void StartLeftWallrun(InputAction.CallbackContext ctx)
    {
        pressedLeftWallrun = true;
    }


    void CancelLeftWallrun(InputAction.CallbackContext ctx)
    {
        pressedLeftWallrun = false;
        if (isLeftWallRunning)
        {
            lastSurface = LastSurface.LeftWall;
            isLeftWallRunning = false;
            rb.useGravity = true;
            rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
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


    void StartSlide(InputAction.CallbackContext ctx)
    {
        pressedSlide = true;
        slideSlowdownLockTime = slideSlowdownLockLength;
    }


    void PerformSlide()
    {
        // The longer you slide, the more the factor goes down, eventually to 0. 
        // This factor multiplies with our desiredFlatVelocity in ApplyHorizontalMovement to slow down the player when they are sliding
        // You get an initial boost from the slideSlowdownFactor (ex. it may be 5), but then it starts to slow down after a moment
        if (!slideSlowdownLocked && grounded) slideSlowdownFactor -= Time.deltaTime * slideSlowdownSpeed;
        if (slideSlowdownFactor < minSlideSlowdownFactor) // end sliding after a bit
        {
            isSliding = false;
            CancelSlide(new InputAction.CallbackContext());
        }
    }


    void CancelSlide(InputAction.CallbackContext ctx)
    {
        pressedSlide = false;
        slideSlowdownFactor = baseSlideSlowdownFactor; // reset
    }
}
