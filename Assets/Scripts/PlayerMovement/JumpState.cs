using UnityEngine;

public class JumpState : IState
{
    private readonly StateMachine _controller;
    private readonly Rigidbody _rb;
    private enum JumpType
    {
        NormalJump,
        LeftWallrunJump,
        RightWallrunJump,
        SlideJump,
        None,
    }
    private JumpType _jumpType = JumpType.None;


    public JumpState(StateMachine controller)
    {
        _controller = controller;
        _rb = _controller.rb;
    }


    public void Apply()
    {
        switch (_jumpType)
        {
            case JumpType.NormalJump:
                if (_controller.exitingState == _controller.groundedMovingState) _rb.position += Vector3.up * 0.1f;
                _rb.AddForce(Vector3.up * _controller.jumpForce, ForceMode.Impulse);
                break;
            case JumpType.LeftWallrunJump:
                _rb.AddForce(Vector3.up * _controller.wallVerticalJumpForce + _controller.leftWallHit.normal * _controller.wallSideJumpForce, ForceMode.Impulse);
                _controller.moveLeftInputLockTime = _controller.moveLeftInputLockLength;
                _controller.useWallJumpGravity = true;
                break;
            case JumpType.RightWallrunJump:
                _rb.AddForce(Vector3.up * _controller.wallVerticalJumpForce + _controller.rightWallHit.normal * _controller.wallSideJumpForce, ForceMode.Impulse);
                _controller.moveRightInputLockTime = _controller.moveRightInputLockLength;
                _controller.useWallJumpGravity = true;
                break;
            case JumpType.SlideJump:
                _rb.AddForce(Vector3.up * _controller.slideJumpVerticalForce + _controller.moveDirection * _controller.slideJumpHorizontalForce, ForceMode.Impulse);
                break;
        }
        _controller.jumpApplied = true;
    }

    public void OnEnter()
    {
        InputManager.Instance.CancelJump();
        // NOTE: Be aware that exitingState is something different in OnEnter vs OnExit, so we assign to bools to keep it consistent
        if (_controller.exitingState == _controller.leftWallrunState) _jumpType = JumpType.LeftWallrunJump;
        else if (_controller.exitingState == _controller.rightWallrunState) _jumpType = JumpType.RightWallrunJump;
        else if (_controller.exitingState == _controller.slideState) _jumpType = JumpType.SlideJump;
        else _jumpType = JumpType.NormalJump;
    }

    public void OnExit()
    {
        _controller.jumpBuffered = false;
        _controller.jumpBufferTime = 0;

        if (_jumpType == JumpType.LeftWallrunJump) _controller.isLeftWallrunningBufferTime = _controller.isLeftWallrunningBufferLength;
        else if (_jumpType == JumpType.RightWallrunJump) _controller.isRightWallrunningBufferTime = _controller.isRightWallrunningBufferLength;
        _jumpType = JumpType.None; // reset
        _controller.jumpApplied = false;
    }
}