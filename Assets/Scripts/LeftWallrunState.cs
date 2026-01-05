using UnityEngine;

public class LeftWallrunState : IState
{
    private readonly StateMachine controller;
    private readonly Rigidbody rb;


    public LeftWallrunState(StateMachine controller)
    {
        this.controller = controller;
        rb = controller.rb;
    }

    public void Apply()
    {
        // Add force to move alongside the wall
        RaycastHit hit = controller.leftWallHit;
        Vector3 wallForward = Vector3.Cross(hit.normal, Vector3.up);
        controller.rb.AddForce(10f * controller.normalSpeed * wallForward.normalized);
    }

    public void OnEnter()
    {
        rb.linearDamping = controller.groundDrag;
        // rb.constraints |= RigidbodyConstraints.FreezePositionY;
        controller.usePlayerGravity = false;
    }

    public void OnExit()
    {
        controller.usePlayerGravity = true;
    }
}