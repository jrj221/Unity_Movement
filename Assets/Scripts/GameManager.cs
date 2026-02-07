using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameplayState gameplayState;
    private MainMenuState mainMenuState;
    private PauseMenuState pauseMenuState;


    private void Awake()
    {
        gameplayState = new GameplayState();
        mainMenuState = new MainMenuState();
        pauseMenuState = new PauseMenuState();
    }
}
