using UnityEngine;

public class AirborneState : IState
{
    private readonly StateMachine controller;
    private readonly Rigidbody rb;
    private Vector3 stairDownPosition;
    private bool stepDownScheduled;


    public AirborneState(StateMachine controller)
    {
        this.controller = controller;
        rb = controller.rb;
    }


    public void Apply()
    {
        if (controller.isMoving)
        {
            if (stepDownScheduled)
            {
                rb.MovePosition(stairDownPosition);
                stepDownScheduled = false;
                controller.cameraSmoothingEnableTime = controller.cameraSmoothingEnableTimeLength;
            }

            float speed = controller.isSprinting ? controller.sprintSpeed : controller.normalSpeed;
            speed *= controller.airMovementMultiplier;
            rb.AddForce(10f * speed * controller.moveDirection);
        }
    }


    bool DownwardsStep()
    {
        if (!ValidDownwardStep(out RaycastHit downHit)) return false; // a step vs a drop below you (like walking off a cliff)
        if (!ValidStepSlopeClearance(downHit)) return false;

        // Success! You can move down a step
        Vector3 amountToMoveVertically = Vector3.down * (controller.feet.position.y - downHit.point.y);
        stairDownPosition = controller.transform.position + amountToMoveVertically;
        return true;
    }


    bool ValidDownwardStep(out RaycastHit downHit)
    {
        // feet are slightly elevated above capsule, so this is slightly off of maxStepHeight
        return Physics.Raycast(controller.feet.position, Vector3.down, out downHit, controller.maxStepHeight);
    }

    
    bool ValidStepSlopeClearance(RaycastHit raycastHit)
    {
        return raycastHit.normal.y > controller.maxStepSlope; // otherwise too sloped to be a step, perhaps it's a ramp
    }


    public void OnEnter()
    {
        rb.linearDamping = 0;
        controller.jumpApplied = false; // reset if you jumped to get airborne

        // check for a step and apply it next frame. If we check every physics frame, we'd get a ton of false positives
        if (!controller.justSteppedUp && controller.exitingState == controller.groundedMovingState && DownwardsStep())
        {
            stepDownScheduled = true;
        }
        controller.justSteppedUp = false;
    }


    public void OnExit()
    {
        controller.inAir = false; // for when you wallrun, otherwise grounded will set this to false when you return to the ground
    }
}