using UnityEngine;
public class GroundedMovingState : IState
{
    private readonly StateMachine controller;
    private readonly Rigidbody rb;
    private Vector3 stairUpPosition;
    private Vector3 stairDownPosition;
    private float forwardNudge = 0.01f; // when you movePosition up a stair, Unity pushes back since you slightly collide 
                                        // with the corner of the stair this allows you to counter that push, staying put
                                        // on the edge of the stair
    private readonly float howFarInFrontToCheck = 0.05f;


    public GroundedMovingState(StateMachine controller)
    {
        this.controller = controller;
        rb = controller.rb;
    }


    public void Apply()
    {
        if (UpwardsStep())
        {
            controller.inAirBufferTime = controller.inAirBufferTimeLength;
            rb.MovePosition(stairUpPosition);
            // so grounded stays true even when you are still trying to clear the step edge, this way you can treat it like a slope
        }
        else if (DownwardsStep())
        {
            controller.inAirBufferTime = controller.inAirBufferTimeLength;
            rb.MovePosition(stairDownPosition);
        }    
        float speed = controller.isSprinting ? controller.sprintSpeed : controller.normalSpeed;
        rb.AddForce(10f * speed * controller.moveDirection);
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


    bool DownwardsStep()
    {
        if (!ValidDownwardStep(out RaycastHit downHit)) return false; // a step vs a drop below you (like walking off a cliff)
        if (StepIsGround(downHit)) return false;
        if (!ValidStepSlopeClearance(downHit)) return false;
        Debug.Log("down");

        // Success! You can move down a step
        Vector3 amountToMoveVertically = Vector3.down * (controller.feet.position.y - downHit.point.y);
        Vector3 amountToMoveHorizontally = controller.moveDirection * howFarInFrontToCheck;
        stairDownPosition = controller.transform.position + amountToMoveVertically + amountToMoveHorizontally;
        return true;
    }


    bool ValidDownwardStep(out RaycastHit downHit)
    {
        // feet are slightly elevated above capsule, so this is slightly off of maxStepHeight
        Vector3 howFarInFrontToCheckDirection = controller.moveDirection * howFarInFrontToCheck;
        // Raycast a tiny bit ahead to see if theres an upcoming step
        Debug.DrawRay(controller.feet.position + howFarInFrontToCheckDirection, Vector3.down * controller.maxStepHeight, Color.orange);
        return Physics.Raycast(controller.feet.position + howFarInFrontToCheckDirection, Vector3.down, out downHit, controller.maxStepHeight);
    }


    bool StepIsGround(RaycastHit downHit)
    {
        return Mathf.Round(downHit.point.y) == (controller.transform.position.y - 0.5 * controller.playerHeight);
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