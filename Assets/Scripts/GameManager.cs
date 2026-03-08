using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Object References
    // Should UI managers be singletons or references here?
    [SerializeField] private PauseMenuUIManager _pauseMenuUIManager;
    [SerializeField] private CameraController _cameraController;
    #endregion
    
    public static GameManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }
    
    
    private void Start()
    {
        InputManager.Instance.DisableInput();
    }
    

    public void StartGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _cameraController.StartGameCameraAnimation(3f);
        Helpers.Instance.Delay(5f, () =>
        {
            InputManager.Instance.EnableInput();
        });
    }
}
