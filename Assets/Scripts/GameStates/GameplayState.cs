using UnityEngine;

public class GameplayState : IState
{
    private readonly GameManager _gameManager;

    public GameplayState(GameManager gm) { _gameManager = gm; }

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
