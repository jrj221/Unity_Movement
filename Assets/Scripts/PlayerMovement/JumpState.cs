using System.Collections;
using UnityEngine;

public class JumpState : State
{
    private enum JumpType
    {
        NormalJump,
        LeftWallrunJump,
        RightWallrunJump,
        SlideJump,
        None,
    }
    private JumpType _jumpType = JumpType.None;
    
    public override void Apply()
    {
        switch (_jumpType)
        {
            case JumpType.NormalJump:
                if (Controller.exitingState == Controller.groundedMovingState || Controller.exitingState == Controller.slideState) Rb.position += Vector3.up * 0.1f; // Same principle as with moving and jumping--player sinks into ground, sometimes negating effect of jump. This pushes them out a bit
                Rb.AddForce(Vector3.up * Controller.jumpForce, ForceMode.Impulse);
                break;
            case JumpType.LeftWallrunJump:
                Rb.AddForce(Vector3.up * Controller.wallVerticalJumpForce + Controller.leftWallHit.normal * Controller.wallSideJumpForce, ForceMode.Impulse);
                Controller.moveLeftInputLockTime = Controller.moveLeftInputLockLength;
                Controller.useWallJumpGravity = true;
                break;
            case JumpType.RightWallrunJump:
                Rb.AddForce(Vector3.up * Controller.wallVerticalJumpForce + Controller.rightWallHit.normal * Controller.wallSideJumpForce, ForceMode.Impulse);
                Controller.moveRightInputLockTime = Controller.moveRightInputLockLength;
                Controller.useWallJumpGravity = true;
                break;
            case JumpType.SlideJump:
                SimulateMomentum(6f, 2f);
                goto case JumpType.NormalJump; // Apply normal jump, but moving with "momentum"
        }
        Controller.jumpApplied = true;
    }

    public override void OnEnter()
    {
        InputManager.Instance.CancelJump();
        // NOTE: Be aware that exitingState is something different in OnEnter vs OnExit, so we assign to bools to keep it consistent
        if (Controller.exitingState == Controller.leftWallrunState) _jumpType = JumpType.LeftWallrunJump;
        else if (Controller.exitingState == Controller.rightWallrunState) _jumpType = JumpType.RightWallrunJump;
        else if (Controller.exitingState == Controller.slideState) _jumpType = JumpType.SlideJump;
        else _jumpType = JumpType.NormalJump;
    }

    public override void OnExit()
    {
        Controller.jumpBuffered = false;
        Controller.jumpBufferTime = 0;

        if (_jumpType == JumpType.LeftWallrunJump) Controller.isLeftWallrunningBufferTime = Controller.isLeftWallrunningBufferLength;
        else if (_jumpType == JumpType.RightWallrunJump) Controller.isRightWallrunningBufferTime = Controller.isRightWallrunningBufferLength;
        _jumpType = JumpType.None; // reset
        Controller.jumpApplied = false;
    }

    private void SimulateMomentum(float speedFactor, float momentumDuration)
    {
        Controller.airMovementMultiplier = speedFactor;
        StartCoroutine(SimulateMomentumRoutine(speedFactor, momentumDuration));
    }

    private IEnumerator SimulateMomentumRoutine(float speedFactor, float momentumDuration)
    {
        Controller.airMovementMultiplier = speedFactor;
        yield return new WaitForSeconds(momentumDuration);
        Controller.airMovementMultiplier = 1;
    }
}