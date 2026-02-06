using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _startButton;
    public StateMachine playerMovement;
    public CameraController cameraMovement;

    private void Awake()
    {
        playerMovement.enabled = false;
        cameraMovement.enabled = false;
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
        playerMovement.enabled = true;
        cameraMovement.enabled = true;
        _document.enabled = false;
    }
}
