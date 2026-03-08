using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuUIManager : UIManger
{
    private Button _resumeButton;
    private Button _settingsButton;
    private Button _quitButton;


    protected override void Awake()
    {
        base.Awake();
        _resumeButton = GetElement<Button>("ResumeButton");
        _settingsButton = GetElement<Button>("SettingsButton");
        _quitButton = GetElement<Button>("QuitButton");
        
        HideUI(); // by default
    }

    private void OnEnable()
    {
        _resumeButton.RegisterCallback<ClickEvent>(ResumeGame);
        _settingsButton.RegisterCallback<ClickEvent>(OpenSettings);
        _quitButton.RegisterCallback<ClickEvent>(QuitToMenu);
    }


    private void OnDisable()
    {
        _resumeButton.UnregisterCallback<ClickEvent>(ResumeGame);
        _settingsButton.UnregisterCallback<ClickEvent>(OpenSettings);
        _quitButton.UnregisterCallback<ClickEvent>(QuitToMenu);
    }

    
    private void Update()
    {
        if (InputManager.Instance.PressedPause)
        {
            ShowUI();
            InputManager.Instance.DisableInput();
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            HideUI();
            InputManager.Instance.EnableInput();
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }


    private void ResumeGame(ClickEvent e)
    {
        InputManager.Instance.TogglePause(); // manually switch pause off if you press the button instead
    }


    private void OpenSettings(ClickEvent e)
    {
        return;
    }
    
    
    private void QuitToMenu(ClickEvent e)
    {
        return;
    }
}
