using System;
using UnityEngine;

public class AirborneState : State
{
    private Vector3 _stairDownPosition;
    private bool _stepDownScheduled;

    public override void Apply()
    {
        if (Controller.isMoving)
        {
            if (_stepDownScheduled)
            {
                Rb.MovePosition(_stairDownPosition);
                _stepDownScheduled = false;
                Controller.wallrunBufferTime = Controller.wallrunBufferLength;
                Controller.cameraSmoothingEnableTime = Controller.cameraSmoothingEnableTimeLength;
            }

            float speed = Controller.isSprinting ? Controller.sprintSpeed : Controller.normalSpeed;
            speed *= Controller.airMovementMultiplier;
            Debug.Log("Speed: " + Math.Round(speed));
            Rb.AddForce(10f * speed * Controller.moveDirection);
        }
    }


    private bool DownwardsStep()
    {
        if (!ValidDownwardStep(out RaycastHit downHit)) return false; // a step vs a drop below you (like walking off a cliff)
        if (!ValidStepSlopeClearance(downHit)) return false;

        // Success! You can move down a step
        Vector3 amountToMoveVertically = Vector3.down * (Controller.feet.position.y - downHit.point.y);
        _stairDownPosition = Controller.transform.position + amountToMoveVertically;
        return true;
    }


    private bool ValidDownwardStep(out RaycastHit downHit)
    {
        // feet are slightly elevated above capsule, so this is slightly off of maxStepHeight
        return Physics.Raycast(Controller.feet.position, Vector3.down, out downHit, Controller.maxStepHeight);
    }

    
    private bool ValidStepSlopeClearance(RaycastHit raycastHit)
    {
        return raycastHit.normal.y > Controller.maxStepSlope; // otherwise too sloped to be a step, perhaps it's a ramp
    }


    public override void OnEnter()
    {
        Controller.inAir = true;
        Rb.linearDamping = 0;

        // check for a step and apply it next frame. If we check every physics frame, we'd get a ton of false positives
        if (!Controller.justSteppedUp && Controller.exitingState == Controller.groundedMovingState && DownwardsStep())
        {
            _stepDownScheduled = true;
        }
        Controller.justSteppedUp = false;
    }


    public override void OnExit()
    {
        Controller.inAir = false; // for when you wallrun, otherwise grounded will set this to false when you return to the ground
    }
}