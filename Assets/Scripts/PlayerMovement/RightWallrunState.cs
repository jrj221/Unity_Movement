using UnityEngine;

public class RightWallrunState : IState
{
    private readonly StateMachine _controller;
    private readonly Rigidbody _rb;


    public RightWallrunState(StateMachine controller)
    {
        this._controller = controller;
        _rb = _controller.rb;
    }

    public void Apply()
    {
        // Add force to move alongside the wall
        RaycastHit hit = _controller.rightWallHit;
        Vector3 wallForward = -Vector3.Cross(hit.normal, Vector3.up); // reversed due to right hand rule

        // move alongside wall
        _controller.rb.AddForce(15f * _controller.normalSpeed * wallForward.normalized);

        // push into wall for concave surfaces
        _controller.rb.AddForce(_controller.pushIntoWallForce * -hit.normal);
    }

    public void OnEnter()
    {
        _rb.linearDamping = _controller.groundDrag;
        // _rb.constraints |= RigidbodyConstraints.FreezePositionY;
        _controller.useCustomGravity = false;
        _controller.isRightWallrunning = true;
    }

    public void OnExit()
    {
        _controller.useCustomGravity = true;
        _controller.isRightWallrunning = false;
        _controller.rightWallrunStopTriggered = false;
    }
}