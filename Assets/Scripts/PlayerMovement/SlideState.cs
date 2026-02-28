using UnityEngine;

public class SlideState : State
{
    public override void Apply()
    {
        Rb.AddForce(10f * Controller.slideSpeed * Controller.moveDirection);
    }


    public override void OnEnter()
    {
        Controller.slideTime = Controller.maxSlideTime;
        Controller.isSliding = true;
    }


    public override void OnExit()
    {
        Controller.slideStopTriggered = false;
        InputManager.Instance.CancelSlide(); // means you must repress the button to initiate a new slide
        Controller.isSliding = false;
    }
}