using UnityEngine;

public class AirborneState : IState
{
    private readonly StateMachine _controller;
    private readonly Rigidbody _rb;
    private Vector3 _stairDownPosition;
    private bool _stepDownScheduled;


    public AirborneState(StateMachine controller)
    {
        _controller = controller;
        _rb = controller.rb;
    }


    public void Apply()
    {
        if (_controller.isMoving)
        {
            if (_stepDownScheduled)
            {
                _rb.MovePosition(_stairDownPosition);
                _stepDownScheduled = false;
                _controller.wallrunBufferTime = _controller.wallrunBufferLength;
                _controller.cameraSmoothingEnableTime = _controller.cameraSmoothingEnableTimeLength;
            }

            float speed = _controller.isSprinting ? _controller.sprintSpeed : _controller.normalSpeed;
            speed *= _controller.airMovementMultiplier;
            _rb.AddForce(10f * speed * _controller.moveDirection);
        }
    }


    bool DownwardsStep()
    {
        if (!ValidDownwardStep(out RaycastHit downHit)) return false; // a step vs a drop below you (like walking off a cliff)
        if (!ValidStepSlopeClearance(downHit)) return false;

        // Success! You can move down a step
        Vector3 amountToMoveVertically = Vector3.down * (_controller.feet.position.y - downHit.point.y);
        _stairDownPosition = _controller.transform.position + amountToMoveVertically;
        return true;
    }


    bool ValidDownwardStep(out RaycastHit downHit)
    {
        // feet are slightly elevated above capsule, so this is slightly off of maxStepHeight
        return Physics.Raycast(_controller.feet.position, Vector3.down, out downHit, _controller.maxStepHeight);
    }

    
    bool ValidStepSlopeClearance(RaycastHit raycastHit)
    {
        return raycastHit.normal.y > _controller.maxStepSlope; // otherwise too sloped to be a step, perhaps it's a ramp
    }


    public void OnEnter()
    {
        _controller.inAir = true;
        _rb.linearDamping = 0;

        // check for a step and apply it next frame. If we check every physics frame, we'd get a ton of false positives
        if (!_controller.justSteppedUp && _controller.exitingState == _controller.groundedMovingState && DownwardsStep())
        {
            _stepDownScheduled = true;
        }
        _controller.justSteppedUp = false;
    }


    public void OnExit()
    {
        _controller.inAir = false; // for when you wallrun, otherwise grounded will set this to false when you return to the ground
    }
}