using Unity.VisualScripting;
using UnityEngine;

public class IdleState : IState
{
    private readonly StateMachine _controller;


    public IdleState(StateMachine _controller)
    {
        this._controller = _controller;
    }


    public void Apply()
    {
        
    }

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }
}