using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]

public class MainMenuEvents : MonoBehaviour
{
    public GameManager gameManager;
    private UIDocument _document;
    private Button _startButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _startButton = _document.rootVisualElement.Q("StartGame") as Button; // type casts into a Button


        _startButton.RegisterCallback<ClickEvent>(OnStartGameClick);
    }

    private void OnDisable()
    {
        // good practice to unregister callbacks
        _startButton.UnregisterCallback<ClickEvent>(OnStartGameClick);
    }


    private void OnStartGameClick(ClickEvent e)
    {
        gameManager.OnGameStarted(); // seems redundant but I wanted to keep the click event disjoint from the gameManager
    }


    public void DisableMainMenu()
    {
        _document.enabled = false;
    }
}
