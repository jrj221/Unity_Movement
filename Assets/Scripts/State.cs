using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected StateMachine Controller { get; private set; }
    protected Rigidbody Rb { get; private set; }
    
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void Apply();

    public void Initialize(StateMachine stateMachine)
    {
        Controller = stateMachine;
        Rb = stateMachine.rb;
    }
}