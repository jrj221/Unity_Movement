using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _resumeButton;
    private Button _settingsButton;
    private Button _quitButton;


    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _resumeButton = _document.rootVisualElement.Q("Buttons").Q("ResumeButton") as Button; // type casts into a Button
        _settingsButton = _document.rootVisualElement.Q("Buttons").Q("SettingsButton") as Button;
        _quitButton = _document.rootVisualElement.Q("Buttons").Q("QuitButton") as Button;

        Helpers.CheckNull(_resumeButton, "resumeButton");
        Helpers.CheckNull(_settingsButton, "settingsButton");
        Helpers.CheckNull(_quitButton, "quitButton");
        
        _document.rootVisualElement.style.display = DisplayStyle.None; // by default
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
            _document.rootVisualElement.style.display = DisplayStyle.Flex;
            InputManager.Instance.DisableInput();
        }
        else
        {
            _document.rootVisualElement.style.display = DisplayStyle.None;
            InputManager.Instance.EnableInput();
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
