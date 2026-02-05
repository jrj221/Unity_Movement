using UnityEngine;

public class JumpState : IState
{
    private readonly StateMachine controller;
    private readonly Rigidbody rb;
    private enum JumpType
    {
        normalJump,
        leftWallrunJump,
        rightWallrunJump,
        slideJump,
        None,
    }
    private JumpType jumpType = JumpType.None;


    public JumpState(StateMachine controller)
    {
        this.controller = controller;
        rb = controller.rb;
    }


    public void Apply()
    {
        switch (jumpType)
        {
            case JumpType.normalJump:
                if (controller.exitingState == controller.groundedMovingState) rb.position += Vector3.up * 0.1f;
                rb.AddForce(Vector3.up * controller.jumpForce, ForceMode.Impulse);
                break;
            case JumpType.leftWallrunJump:
                rb.AddForce(Vector3.up * controller.wallVerticalJumpForce + controller.leftWallHit.normal * controller.wallSideJumpForce, ForceMode.Impulse);
                controller.moveLeftInputLockTime = controller.moveLeftInputLockLength;
                controller.useWallJumpGravity = true;
                break;
            case JumpType.rightWallrunJump:
                rb.AddForce(Vector3.up * controller.wallVerticalJumpForce + controller.rightWallHit.normal * controller.wallSideJumpForce, ForceMode.Impulse);
                controller.moveRightInputLockTime = controller.moveRightInputLockLength;
                controller.useWallJumpGravity = true;
                break;
            case JumpType.slideJump:
                rb.AddForce(Vector3.up * controller.slideJumpVerticalForce + controller.moveDirection * controller.slideJumpHorizontalForce, ForceMode.Impulse);
                break;
        }
        controller.jumpApplied = true;
    }

    public void OnEnter()
    {
        controller.pressedJump = false; // prevents continous bouncing

        // NOTE: Be aware that exitingState is something different in OnEnter vs OnExit, so we assign to bools to keep it consistent
        if (controller.exitingState == controller.leftWallrunState) jumpType = JumpType.leftWallrunJump;
        else if (controller.exitingState == controller.rightWallrunState) jumpType = JumpType.rightWallrunJump;
        else if (controller.exitingState == controller.slideState) jumpType = JumpType.slideJump;
        else jumpType = JumpType.normalJump;
    }

    public void OnExit()
    {
        controller.jumpBuffered = false;
        controller.jumpBufferTime = 0;

        if (jumpType == JumpType.leftWallrunJump) controller.isLeftWallrunningBufferTime = controller.isLeftWallrunningBufferLength;
        else if (jumpType == JumpType.rightWallrunJump) controller.isRightWallrunningBufferTime = controller.isRightWallrunningBufferLength;
        jumpType = JumpType.None; // reset
        controller.jumpApplied = false;
    }
}