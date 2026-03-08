using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]

public class MainMenuUIManager : UIManger
{
    private Button _startButton;
    public static MainMenuUIManager Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        _startButton = GetElement<Button>("StartGameButton");
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
        ShowUI();
    }

    
    private void OnStartGameClick(ClickEvent e)
    {
        HideUI();
        GameManager.Instance.StartGame();
    }
}
