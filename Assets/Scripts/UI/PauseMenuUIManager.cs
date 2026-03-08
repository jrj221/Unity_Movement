using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuUIManager : UIManger
{
    private Button _resumeButton;


    protected override void Awake()
    {
        base.Awake();
        _resumeButton = GetElement<Button>("ResumeButton");
        
        HideUI(); // by default
    }

    private void OnEnable()
    {
        _resumeButton.RegisterCallback<ClickEvent>(ResumeGame);
    }


    private void OnDisable()
    {
        _resumeButton.UnregisterCallback<ClickEvent>(ResumeGame);
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
}
