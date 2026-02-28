using UnityEngine;

public class GroundedMovingState : State
{
    private Vector3 _stairUpPosition;
    private readonly float _forwardNudge = 0.01f; // when you movePosition up a stair, Unity pushes back since you slightly collide 
                                        // with the corner of the stair this allows you to counter that push, staying put
                                        // on the edge of the stair

                                        
    public override void Apply()
    {
        float speed = Controller.isSprinting ? Controller.sprintSpeed : Controller.normalSpeed;
        if (UpwardsStep())
        {
            Rb.MovePosition(_stairUpPosition);
            Controller.justSteppedUp = true;
            Controller.wallrunBufferTime = Controller.wallrunBufferLength;
            Controller.cameraSmoothingEnableTime = Controller.cameraSmoothingEnableTimeLength;
        }

        if (OnSlope())
        {
            Controller.moveDirection = Vector3.ProjectOnPlane(Controller.moveDirection, Controller.groundHit.normal);
            // Controller.usePlayerGravity = false; // why did the tutorial want this?
            if (Rb.linearVelocity.y > 0) Rb.AddForce(Vector3.down * Controller.stickToSlopeForce); // if going up slopes
        }

        Debug.DrawRay(Controller.transform.position, Controller.moveDirection, Color.blue);
        Rb.AddForce(10f * speed * Controller.moveDirection);
    }


    bool OnSlope()
    {
        Physics.Raycast(Controller.feet.position, Vector3.down, out RaycastHit groundHit, 0.1f);
        float slopeAngle = Vector3.Angle(Vector3.up, groundHit.normal);
        return slopeAngle <= Controller.maxSlopeAngle && slopeAngle != 0; // what happens if it's greater?
    }


    bool UpwardsStep()
    {
        if (!Controller.wallInSomeDirection) return false;
        if (!IsMovingTowardsStair()) return false; // (otherwise it might trigger when going down stairs)
        // Get information about the height of the step
        Vector3 horizontalStepOffset = Controller.wallDirection * (Controller.minStepLength + Controller.playerRadius); // how far from player to check
        Vector3 verticalStepOffset = new(0, Controller.maxStepHeight + Controller.playerHeight, 0); // how far to check above step, making sure there's room for the capsule after snapping
        Debug.DrawRay(Controller.feet.position + horizontalStepOffset + verticalStepOffset, Vector3.down * (Controller.playerHeight + Controller.maxStepHeight), Color.purple);
        Physics.Raycast(Controller.feet.position + horizontalStepOffset + verticalStepOffset, Vector3.down, out RaycastHit heightHit, Controller.playerHeight + Controller.maxStepHeight);
        
        if (!ValidStepSlopeClearance(heightHit)) return false;
        float stepHeight = heightHit.point.y - Controller.feet.position.y;
        if (!ValidStepHeight(stepHeight)) return false;
        if (!ValidStepLength(stepHeight)) return false;

        // Success! You can go up the step
        Physics.Raycast(Controller.feet.position, Controller.transform.forward, out RaycastHit wallHit);
        float distToStep = Vector3.ProjectOnPlane(wallHit.point - Controller.feet.position, Vector3.up).magnitude;
        Vector3 amountToMoveHorizontally = Controller.moveDirection * _forwardNudge;
        Vector3 amountToMoveVertically = Vector3.up * stepHeight;
        _stairUpPosition = Controller.transform.position + amountToMoveVertically + amountToMoveHorizontally;
        return true;
    }


    bool IsMovingTowardsStair()
    {
        return Vector3.Dot(Controller.wallDirection, Controller.moveDirection) > 0;
    }

    
    bool ValidStepSlopeClearance(RaycastHit raycastHit)
    {
        return raycastHit.normal.y > Controller.maxStepSlope; // otherwise too sloped to be a step, perhaps it's a ramp
    }


    bool ValidStepHeight(float stepHeight)
    {
        // if there was a low ceiling, stepHeight would appear to be bigger that it actually is, which is okay to fail since we wouldn't want to make the step anyway
        return stepHeight <= Controller.maxStepHeight; // valid step
    }


    bool ValidStepLength(float stepHeight)
    {
        // raycast forward to see if the step is long enough. We want it to be false since that means the step is long enough
        Debug.DrawRay(Controller.feet.position + new Vector3(0, stepHeight + 0.01f, 0), Controller.wallDirection * Controller.minStepLength, Color.purple);
        return !Physics.Raycast(Controller.feet.position + new Vector3(0, stepHeight + 0.01f, 0), Controller.wallDirection, Controller.minStepLength);
    }



    public override void OnEnter()
    {
        Rb.linearDamping = Controller.groundDrag;
        Controller.useWallJumpGravity = false;
    }

    public override void OnExit()
    {
        
    }
}