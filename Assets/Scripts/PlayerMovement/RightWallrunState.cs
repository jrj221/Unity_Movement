using UnityEngine;

public class RightWallrunState : IState
{
    private readonly StateMachine controller;
    private readonly Rigidbody rb;


    public RightWallrunState(StateMachine controller)
    {
        this.controller = controller;
        rb = controller.rb;
    }

    public void Apply()
    {
        // Add force to move alongside the wall
        RaycastHit hit = controller.rightWallHit;
        Vector3 wallForward = -Vector3.Cross(hit.normal, Vector3.up); // reversed due to right hand rule

        // move alongside wall
        controller.rb.AddForce(15f * controller.normalSpeed * wallForward.normalized);

        // push into wall for concave surfaces
        controller.rb.AddForce(controller.pushIntoWallForce * -hit.normal);
    }

    public void OnEnter()
    {
        rb.linearDamping = controller.groundDrag;
        // rb.constraints |= RigidbodyConstraints.FreezePositionY;
        controller.useCustomGravity = false;
        controller.isRightWallrunning = true;
    }

    public void OnExit()
    {
        controller.useCustomGravity = true;
        controller.isRightWallrunning = false;
        controller.rightWallrunStopTriggered = false;
    }
}