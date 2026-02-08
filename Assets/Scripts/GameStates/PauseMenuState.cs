using UnityEngine;

public class PauseMenuState : IState
{
    private PauseMenuEvents pauseMenuEvents;

    public PauseMenuState(PauseMenuEvents events)
    {
        pauseMenuEvents = events;
    }

    public void Apply()
    {

    }

    public void OnEnter()
    {
        pauseMenuEvents.ShowPauseMenu();
        Cursor.visible = true;
    }

    public void OnExit()
    {
        pauseMenuEvents.HidePauseMenu();
        Cursor.visible = false;
    }
}
