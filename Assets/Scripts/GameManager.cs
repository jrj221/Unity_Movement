using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region States
    private GameplayState _gameplayState;
    private IState _exitingState;
    private IState _currentState;
    private IState _nextState;
    #endregion
    
    #region Object References
    [SerializeField] private PauseMenuEvents _pauseMenuEvents;
    #endregion
    
    public static GameManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
        _gameplayState = new GameplayState(this);
    }


    private void Start()
    {
        _currentState = _gameplayState;
    }


    private void Update()
    {
        _nextState = DetermineNextState();
        if (_nextState != _currentState) ChangeState();
        Debug.Log(_currentState);
    }


    private IState DetermineNextState()
    {
        switch (_currentState)
        {
            case GameplayState:
                return _gameplayState;
        }
        return null; // won't logically happen but it wanted a return path
    }


    private void ChangeState()
    {
        _exitingState = _currentState;
        _exitingState.OnExit();
        _currentState = _nextState;
        _currentState.OnEnter();
    }


    public void StartGame()
    {
        _pauseMenuEvents.enabled = true;
        InputManager.Instance.EnableInput();
    }
}
