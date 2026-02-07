using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameplayState gameplayState;
    private MainMenuState mainMenuState;
    private PauseMenuState pauseMenuState;
    private IState currentState;
    private IState nextState;


    private void Awake()
    {
        gameplayState = new GameplayState();
        mainMenuState = new MainMenuState();
        pauseMenuState = new PauseMenuState();
    }


    private void Start()
    {
        currentState = mainMenuState;
    }


    private void Update()
    {
        nextState = DetermineNextState();
        if (nextState != currentState) ChangeState(nextState);
    }


    

}
