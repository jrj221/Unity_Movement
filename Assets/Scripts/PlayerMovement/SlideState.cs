using UnityEngine;

public class SlideState : State
{
    public float slideSpeed;
    public float maxSlideTime;
    
    public override void Apply()
    {
        Rb.AddForce(10f * slideSpeed * Controller.moveDirection);
    }


    public override void OnEnter()
    {
        Controller.slideTime = maxSlideTime;
        Controller.isSliding = true;
    }


    public override void OnExit()
    {
        Controller.slideStopTriggered = false;
        InputManager.Instance.CancelSlide(); // means you must repress the button to initiate a new slide
        Controller.isSliding = false;
    }
}