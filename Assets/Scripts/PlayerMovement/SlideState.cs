using UnityEngine;

public class SlideState : IState
{
    private readonly StateMachine _controller;
    private readonly Rigidbody _rb;


    public SlideState(StateMachine controller)
    {
        this._controller = controller;
        _rb = _controller.rb;
    }


    public void Apply()
    {
        _rb.AddForce(10f * _controller.slideSpeed * _controller.moveDirection);
    }


    public void OnEnter()
    {
        _controller.slideTime = _controller.maxSlideTime;
        _controller.isSliding = true;
    }


    public void OnExit()
    {
        _controller.slideStopTriggered = false;
        _controller.inputManager.CancelSlide(); // means you must repress the button to initiate a new slide
        _controller.isSliding = false;
    }
}