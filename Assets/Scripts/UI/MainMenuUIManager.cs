using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]

public class MainMenuUIManager : UIManger
{
    private Button _startButton;

    protected override void Awake()
    {
        base.Awake();
        _startButton = GetElement<Button>("StartGameButton");
        Helpers.CheckNull(_startButton, "_startButton");
    }
    

    private void OnEnable()
    {
        _startButton.RegisterCallback<ClickEvent>(OnStartGameClick);
    }
    

    private void OnDisable()
    {
        // good practice to unregister callbacks
        _startButton.UnregisterCallback<ClickEvent>(OnStartGameClick);
    }

    private void Start()
    {
        InputManager.Instance.DisableInput();
    }


    private void OnStartGameClick(ClickEvent e)
    {
        HideUI();
        GameManager.Instance.StartGame();
    }
}
