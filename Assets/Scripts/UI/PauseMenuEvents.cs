using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuEvents : MonoBehaviour
{
    public InputManager inputManager;
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


        print("found resume: " + _resumeButton);
        _document.enabled = false; // by default

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



    public void ShowPauseMenu()
    {
        _document.enabled = true;
    }


    public void HidePauseMenu()
    {
        _document.enabled = false;
    }


    private void ResumeGame(ClickEvent e)
    {
        Debug.Log("ne");
        inputManager.ManualPauseToggle(); // manually switch pause off if you press the button instead
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
