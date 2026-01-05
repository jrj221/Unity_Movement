using Unity.VisualScripting;
using UnityEngine;

public class IdleState : IState
{
    private readonly StateMachine controller;


    public IdleState(StateMachine controller)
    {
        this.controller = controller;
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