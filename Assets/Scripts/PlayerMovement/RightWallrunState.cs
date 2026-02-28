using UnityEngine;

public class RightWallrunState : State
{
    public override void Apply()
    {
        // Add force to move alongside the wall
        RaycastHit hit = Controller.rightWallHit;
        Vector3 wallForward = -Vector3.Cross(hit.normal, Vector3.up); // reversed due to right hand rule

        // move alongside wall
        Controller.rb.AddForce(15f * Controller.normalSpeed * wallForward.normalized);

        // push into wall for concave surfaces
        Controller.rb.AddForce(Controller.pushIntoWallForce * -hit.normal);
    }

    public override void OnEnter()
    {
        Rb.linearDamping = Controller.groundDrag;
        // Rb.constraints |= RigidbodyConstraints.FreezePositionY;
        Controller.useCustomGravity = false;
        Controller.isRightWallrunning = true;
    }

    public override void OnExit()
    {
        Controller.useCustomGravity = true;
        Controller.isRightWallrunning = false;
        Controller.rightWallrunStopTriggered = false;
    }
}