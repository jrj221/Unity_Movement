using UnityEngine;

public class AirborneState : IState
{
    private readonly StateMachine controller;
    private readonly Rigidbody rb;


    public AirborneState(StateMachine controller)
    {
        this.controller = controller;
        rb = controller.rb;
    }


    public void Apply()
    {
        if (controller.isMoving)
        {
            float speed = controller.isSprinting ? controller.sprintSpeed : controller.normalSpeed;
            speed *= controller.airMovementMultiplier;
            rb.AddForce(10f * speed * controller.moveDirection);
        }
    }

    public void OnEnter()
    {
        rb.linearDamping = 0;
        controller.inAir = true;
        controller.jumpApplied = false; // reset if you jumped to get airborne
    }

    public void OnExit()
    {
        controller.inAir = false; // for when you wallrun, otherwise grounded will set this to false when you return to the ground
    }
}