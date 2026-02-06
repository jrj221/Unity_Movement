using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]

public class MainMenuEvents : MonoBehaviour
{
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
        Debug.Log("play!");
    }
}
