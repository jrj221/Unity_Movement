using UnityEngine;

public class GroundedMovingState : IState
{
    private readonly StateMachine _controller;
    private readonly Rigidbody _rb;
    private Vector3 _stairUpPosition;
    private readonly float _forwardNudge = 0.01f; // when you movePosition up a stair, Unity pushes back since you slightly collide 
                                        // with the corner of the stair this allows you to counter that push, staying put
                                        // on the edge of the stair

    public GroundedMovingState(StateMachine controller)
    {
        this._controller = controller;
        _rb = _controller.rb;
    }


    public void Apply()
    {
        float speed = _controller.isSprinting ? _controller.sprintSpeed : _controller.normalSpeed;
        if (UpwardsStep())
        {
            _rb.MovePosition(_stairUpPosition);
            _controller.justSteppedUp = true;
            _controller.wallrunBufferTime = _controller.wallrunBufferLength;
            _controller.cameraSmoothingEnableTime = _controller.cameraSmoothingEnableTimeLength;
        }

        if (OnSlope())
        {
            _controller.moveDirection = Vector3.ProjectOnPlane(_controller.moveDirection, _controller.groundHit.normal);
            // _controller.usePlayerGravity = false; // why did the tutorial want this?
            if (_rb.linearVelocity.y > 0) _rb.AddForce(Vector3.down * _controller.stickToSlopeForce); // if going up slopes
        }

        Debug.DrawRay(_controller.transform.position, _controller.moveDirection, Color.blue);
        _rb.AddForce(10f * speed * _controller.moveDirection);
    }


    bool OnSlope()
    {
        Physics.Raycast(_controller.feet.position, Vector3.down, out RaycastHit groundHit, 0.1f);
        float slopeAngle = Vector3.Angle(Vector3.up, groundHit.normal);
        return slopeAngle <= _controller.maxSlopeAngle && slopeAngle != 0; // what happens if it's greater?
    }


    bool UpwardsStep()
    {
        if (!_controller.wallInSomeDirection) return false;
        if (!IsMovingTowardsStair()) return false; // (otherwise it might trigger when going down stairs)
        // Get information about the height of the step
        Vector3 horizontalStepOffset = _controller.wallDirection * (_controller.minStepLength + _controller.playerRadius); // how far from player to check
        Vector3 verticalStepOffset = new(0, _controller.maxStepHeight + _controller.playerHeight, 0); // how far to check above step, making sure there's room for the capsule after snapping
        Debug.DrawRay(_controller.feet.position + horizontalStepOffset + verticalStepOffset, Vector3.down * (_controller.playerHeight + _controller.maxStepHeight), Color.purple);
        Physics.Raycast(_controller.feet.position + horizontalStepOffset + verticalStepOffset, Vector3.down, out RaycastHit heightHit, _controller.playerHeight + _controller.maxStepHeight);
        
        if (!ValidStepSlopeClearance(heightHit)) return false;
        float stepHeight = heightHit.point.y - _controller.feet.position.y;
        if (!ValidStepHeight(stepHeight)) return false;
        if (!ValidStepLength(stepHeight)) return false;

        // Success! You can go up the step
        Physics.Raycast(_controller.feet.position, _controller.transform.forward, out RaycastHit wallHit);
        float distToStep = Vector3.ProjectOnPlane(wallHit.point - _controller.feet.position, Vector3.up).magnitude;
        Vector3 amountToMoveHorizontally = _controller.moveDirection * _forwardNudge;
        Vector3 amountToMoveVertically = Vector3.up * stepHeight;
        _stairUpPosition = _controller.transform.position + amountToMoveVertically + amountToMoveHorizontally;
        return true;
    }


    bool IsMovingTowardsStair()
    {
        return Vector3.Dot(_controller.wallDirection, _controller.moveDirection) > 0;
    }

    
    bool ValidStepSlopeClearance(RaycastHit raycastHit)
    {
        return raycastHit.normal.y > _controller.maxStepSlope; // otherwise too sloped to be a step, perhaps it's a ramp
    }


    bool ValidStepHeight(float stepHeight)
    {
        // if there was a low ceiling, stepHeight would appear to be bigger that it actually is, which is okay to fail since we wouldn't want to make the step anyway
        return stepHeight <= _controller.maxStepHeight; // valid step
    }


    bool ValidStepLength(float stepHeight)
    {
        // raycast forward to see if the step is long enough. We want it to be false since that means the step is long enough
        Debug.DrawRay(_controller.feet.position + new Vector3(0, stepHeight + 0.01f, 0), _controller.wallDirection * _controller.minStepLength, Color.purple);
        return !Physics.Raycast(_controller.feet.position + new Vector3(0, stepHeight + 0.01f, 0), _controller.wallDirection, _controller.minStepLength);
    }



    public void OnEnter()
    {
        _rb.linearDamping = _controller.groundDrag;
        _controller.useWallJumpGravity = false;
    }

    public void OnExit()
    {
        
    }
}