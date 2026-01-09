using UnityEngine;

public class SlideState : IState
{
    private readonly StateMachine controller;
    private readonly Rigidbody rb;


    public SlideState(StateMachine controller)
    {
        this.controller = controller;
        rb = controller.rb;
    }


    public void Apply()
    {
        rb.AddForce(10f * controller.slideSpeed * controller.moveDirection);
    }


    public void OnEnter()
    {
        controller.slideTime = controller.maxSlideTime;
        // controller.slideStartTriggered = false;
        controller.isSliding = true;
    }


    public void OnExit()
    {
        controller.slideStopTriggered = false;
        controller.pressedSlide = false; // means you must repress the button to initate a new slide
        controller.isSliding = false;
    }
}