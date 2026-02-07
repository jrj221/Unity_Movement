using UnityEngine;

public class GameplayState : IState
{
    private readonly GameManager gameManager;

    public GameplayState(GameManager GM) { gameManager = GM; }

    public void Apply()
    {
        
    }

    public void OnEnter()
    {
        // Disable the menu and enable player controls
        gameManager.playerMovement.enabled = true;
        gameManager.cameraMovement.enabled = true;
        gameManager.mainMenuEvents.DisableMainMenu();
        Cursor.visible = false;
    }

    public void OnExit()
    {
        
    }
}
