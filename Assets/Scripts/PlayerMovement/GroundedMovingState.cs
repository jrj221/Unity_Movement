using UnityEngine;

public class GroundedMovingState : IState
{
    private readonly StateMachine controller;
    private readonly Rigidbody rb;
    private Vector3 stairUpPosition;
    private readonly float forwardNudge = 0.01f; // when you movePosition up a stair, Unity pushes back since you slightly collide 
                                        // with the corner of the stair this allows you to counter that push, staying put
                                        // on the edge of the stair

    public GroundedMovingState(StateMachine controller)
    {
        this.controller = controller;
        rb = controller.rb;
    }


    public void Apply()
    {
        float speed = controller.isSprinting ? controller.sprintSpeed : controller.normalSpeed;
        if (UpwardsStep())
        {
            rb.MovePosition(stairUpPosition);
            controller.justSteppedUp = true;
            controller.wallrunBufferTime = controller.wallrunBufferLength;
            controller.cameraSmoothingEnableTime = controller.cameraSmoothingEnableTimeLength;
        }

        if (OnSlope())
        {
            controller.moveDirection = Vector3.ProjectOnPlane(controller.moveDirection, controller.groundHit.normal);
            // controller.usePlayerGravity = false; // why did the tutorial want this?
            if (rb.linearVelocity.y > 0) rb.AddForce(Vector3.down * controller.stickToSlopeForce); // if going up slopes
        }

        Debug.DrawRay(controller.transform.position, controller.moveDirection, Color.blue);
        rb.AddForce(10f * speed * controller.moveDirection);
    }


    bool OnSlope()
    {
        Physics.Raycast(controller.feet.position, Vector3.down, out RaycastHit groundHit, 0.1f);
        float slopeAngle = Vector3.Angle(Vector3.up, groundHit.normal);
        return slopeAngle <= controller.maxSlopeAngle && slopeAngle != 0; // what happens if it's greater?
    }


    bool UpwardsStep()
    {
        if (!controller.wallInSomeDirection) return false;
        if (!IsMovingTowardsStair()) return false; // (otherwise it might trigger when going down stairs)
        // Get information about the height of the step
        Vector3 horizontalStepOffset = controller.wallDirection * (controller.minStepLength + controller.playerRadius); // how far from player to check
        Vector3 verticalStepOffset = new(0, controller.maxStepHeight + controller.playerHeight, 0); // how far to check above step, making sure there's room for the capsule after snapping
        Debug.DrawRay(controller.feet.position + horizontalStepOffset + verticalStepOffset, Vector3.down * (controller.playerHeight + controller.maxStepHeight), Color.purple);
        Physics.Raycast(controller.feet.position + horizontalStepOffset + verticalStepOffset, Vector3.down, out RaycastHit heightHit, controller.playerHeight + controller.maxStepHeight);
        
        if (!ValidStepSlopeClearance(heightHit)) return false;
        float stepHeight = heightHit.point.y - controller.feet.position.y;
        if (!ValidStepHeight(stepHeight)) return false;
        if (!ValidStepLength(stepHeight)) return false;

        // Success! You can go up the step
        Physics.Raycast(controller.feet.position, controller.transform.forward, out RaycastHit wallHit);
        float distToStep = Vector3.ProjectOnPlane(wallHit.point - controller.feet.position, Vector3.up).magnitude;
        Vector3 amountToMoveHorizontally = controller.moveDirection * forwardNudge;
        Vector3 amountToMoveVertically = Vector3.up * stepHeight;
        stairUpPosition = controller.transform.position + amountToMoveVertically + amountToMoveHorizontally;
        return true;
    }


    bool IsMovingTowardsStair()
    {
        return Vector3.Dot(controller.wallDirection, controller.moveDirection) > 0;
    }

    
    bool ValidStepSlopeClearance(RaycastHit raycastHit)
    {
        return raycastHit.normal.y > controller.maxStepSlope; // otherwise too sloped to be a step, perhaps it's a ramp
    }


    bool ValidStepHeight(float stepHeight)
    {
        // if there was a low ceiling, stepHeight would appear to be bigger that it actually is, which is okay to fail since we wouldn't want to make the step anyway
        return stepHeight <= controller.maxStepHeight; // valid step
    }


    bool ValidStepLength(float stepHeight)
    {
        // raycast forward to see if the step is long enough. We want it to be false since that means the step is long enough
        Debug.DrawRay(controller.feet.position + new Vector3(0, stepHeight + 0.01f, 0), controller.wallDirection * controller.minStepLength, Color.purple);
        return !Physics.Raycast(controller.feet.position + new Vector3(0, stepHeight + 0.01f, 0), controller.wallDirection, controller.minStepLength);
    }



    public void OnEnter()
    {
        rb.linearDamping = controller.groundDrag;
        controller.useWallJumpGravity = false;
    }

    public void OnExit()
    {
        
    }
}