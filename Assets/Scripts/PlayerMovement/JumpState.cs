using UnityEngine;

public class JumpState : IState
{
    private readonly StateMachine controller;
    private readonly Rigidbody rb;
    private bool normalJump;
    private bool leftWallrunJump;
    private bool rightWallrunJump;
    private bool slideJump;


    public JumpState(StateMachine controller)
    {
        this.controller = controller;
        rb = controller.rb;
    }


    public void Apply()
    {
        if (normalJump)
        {
            rb.AddForce(Vector3.up * controller.jumpForce, ForceMode.Impulse);
        }
        else if (leftWallrunJump)
        {
            rb.AddForce(Vector3.up * controller.wallVerticalJumpForce + controller.leftWallHit.normal * controller.wallSideJumpForce, ForceMode.Impulse);
            controller.moveLeftInputLockTime = controller.moveLeftInputLockLength;
            controller.useWallJumpGravity = true;
        }
        else if (rightWallrunJump)
        {
            rb.AddForce(Vector3.up * controller.wallVerticalJumpForce + controller.rightWallHit.normal * controller.wallSideJumpForce, ForceMode.Impulse);
            controller.moveRightInputLockTime = controller.moveRightInputLockLength;
            controller.useWallJumpGravity = true;
        }
        else if (slideJump)
        {
            rb.AddForce(Vector3.up * controller.slideJumpVerticalForce + controller.moveDirection * controller.slideJumpHorizontalForce, ForceMode.Impulse);
        }
        controller.jumpApplied = true;
    }

    public void OnEnter()
    {
        controller.pressedJump = false; // prevents continous bouncing

        // NOTE: Be aware that exitingState is something different in OnEnter vs OnExit, so we assign to bools to keep it consistent
        if (controller.exitingState == controller.leftWallrunState) leftWallrunJump = true;
        else if (controller.exitingState == controller.rightWallrunState) rightWallrunJump = true;
        else if (controller.exitingState == controller.slideState) slideJump = true;
        else normalJump = true;
    }

    public void OnExit()
    {
        // controller.jumpBufferTime = 0;
        
        if (leftWallrunJump) controller.isLeftWallrunningBufferTime = controller.isLeftWallrunningBufferLength;
        else if (rightWallrunJump) controller.isRightWallrunningBufferTime = controller.isRightWallrunningBufferLength;
        normalJump = false; // reset in case we used one of them
        leftWallrunJump = false;
        rightWallrunJump = false;
        slideJump = false;
    }
}