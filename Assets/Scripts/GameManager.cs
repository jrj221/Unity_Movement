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
    private bool gameStarted;
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
                if (gameStarted) return gameplayState;
                else return mainMenuState;
            case PauseMenuState:
                break;
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
        gameStarted = true;
    }
}
