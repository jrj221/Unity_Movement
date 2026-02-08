using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region States
    private GameplayState gameplayState;
    private MainMenuState mainMenuState;
    private PauseMenuState pauseMenuState;
    private IState exitingState;
    private IState currentState;
    private IState nextState;
    #endregion

    #region Object References
    public StateMachine playerMovement;
    public CameraController cameraMovement;
    public MainMenuEvents mainMenuEvents;
    #endregion

    #region Class Variables
    private bool gameInMenu;
    private bool gamePaused;
    #endregion


    private void Awake()
    {
        gameplayState = new GameplayState(this);
        mainMenuState = new MainMenuState();
        pauseMenuState = new PauseMenuState();

        playerMovement.enabled = false;
        cameraMovement.enabled = false;
    }


    private void Start()
    {
        currentState = mainMenuState;
    }


    private void Update()
    {
        nextState = DetermineNextState();
        if (nextState != currentState) ChangeState(nextState);
        // Debug.Log(currentState);
    }


    private IState DetermineNextState()
    {
        switch (currentState)
        {
            case MainMenuState:
                if (!gameInMenu) return gameplayState;
                else return mainMenuState;
            case PauseMenuState:
                if (!gamePaused) return gameplayState;
                else if (gameInMenu) return mainMenuState;
                else return pauseMenuState;
            case GameplayState:
                return gameplayState;
        }
        return null; // won't logically happen but it wanted a return path
    }


    private void ChangeState(IState nextState)
    {
        exitingState = currentState;
        exitingState.OnExit();
        currentState = nextState;
        currentState.OnEnter();
    }


    public void OnGameStarted()
    {
        gameInMenu = false;
    }


    public void OnGameUnpaused()
    {
        gamePaused = false;
    }


    public void OnBackToMenu()
    {
        gameInMenu = true;
    }
}
