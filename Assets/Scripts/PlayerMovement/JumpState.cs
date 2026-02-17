using UnityEngine;

public class JumpState : IState
{
    private readonly StateMachine _controller;
    private readonly Rigidbody _rb;
    private enum JumpType
    {
        normalJump,
        leftWallrunJump,
        rightWallrunJump,
        slideJump,
        None,
    }
    private JumpType _jumpType = JumpType.None;


    public JumpState(StateMachine controller)
    {
        this._controller = controller;
        _rb = _controller.rb;
    }


    public void Apply()
    {
        switch (_jumpType)
        {
            case JumpType.normalJump:
                if (_controller.exitingState == _controller.groundedMovingState) _rb.position += Vector3.up * 0.1f;
                _rb.AddForce(Vector3.up * _controller.jumpForce, ForceMode.Impulse);
                break;
            case JumpType.leftWallrunJump:
                _rb.AddForce(Vector3.up * _controller.wallVerticalJumpForce + _controller.leftWallHit.normal * _controller.wallSideJumpForce, ForceMode.Impulse);
                _controller.moveLeftInputLockTime = _controller.moveLeftInputLockLength;
                _controller.useWallJumpGravity = true;
                break;
            case JumpType.rightWallrunJump:
                _rb.AddForce(Vector3.up * _controller.wallVerticalJumpForce + _controller.rightWallHit.normal * _controller.wallSideJumpForce, ForceMode.Impulse);
                _controller.moveRightInputLockTime = _controller.moveRightInputLockLength;
                _controller.useWallJumpGravity = true;
                break;
            case JumpType.slideJump:
                _rb.AddForce(Vector3.up * _controller.slideJumpVerticalForce + _controller.moveDirection * _controller.slideJumpHorizontalForce, ForceMode.Impulse);
                break;
        }
        _controller.jumpApplied = true;
    }

    public void OnEnter()
    {
        _controller.pressedJump = false; // prevents continous bouncing

        // NOTE: Be aware that exitingState is something different in OnEnter vs OnExit, so we assign to bools to keep it consistent
        if (_controller.exitingState == _controller.leftWallrunState) _jumpType = JumpType.leftWallrunJump;
        else if (_controller.exitingState == _controller.rightWallrunState) _jumpType = JumpType.rightWallrunJump;
        else if (_controller.exitingState == _controller.slideState) _jumpType = JumpType.slideJump;
        else _jumpType = JumpType.normalJump;
    }

    public void OnExit()
    {
        _controller.jumpBuffered = false;
        _controller.jumpBufferTime = 0;

        if (_jumpType == JumpType.leftWallrunJump) _controller.isLeftWallrunningBufferTime = _controller.isLeftWallrunningBufferLength;
        else if (_jumpType == JumpType.rightWallrunJump) _controller.isRightWallrunningBufferTime = _controller.isRightWallrunningBufferLength;
        _jumpType = JumpType.None; // reset
        _controller.jumpApplied = false;
    }
}