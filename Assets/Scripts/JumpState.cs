using UnityEngine;

public class JumpState : IState
{
    private readonly StateMachine controller;
    private readonly Rigidbody rb;


    public JumpState(StateMachine controller)
    {
        this.controller = controller;
        rb = controller.rb;
    }


    public void Apply()
    {
        if (controller.exitingState == controller.idleState || controller.exitingState == controller.groundedMovingState) 
        {
            rb.AddForce(Vector3.up * controller.jumpForce, ForceMode.Impulse);
        } else if (controller.exitingState == controller.leftWallrunState)
        {
            rb.AddForce(Vector3.up * controller.wallVerticalJumpForce + controller.leftWallHit.normal * controller.wallSideJumpForce, ForceMode.Impulse);
            controller.moveLeftInputLockTime = controller.moveLeftInputLockLength; 
            controller.useWallJumpGravity = true;
        } else if (controller.exitingState == controller.rightWallrunState)
        {
            rb.AddForce(Vector3.up * controller.wallVerticalJumpForce + controller.rightWallHit.normal * controller.wallSideJumpForce, ForceMode.Impulse); 
            controller.moveRightInputLockTime = controller.moveRightInputLockLength;
        }
        controller.jumpApplied = true;
    }

    public void OnEnter()
    {
        controller.pressedJump = false; // prevents continous bouncing
    }

    public void OnExit()
    {
       controller.inAirBufferTime = controller.inAirBufferTimeLength; // So grounded doesn't overwrite inAir 
    }
}