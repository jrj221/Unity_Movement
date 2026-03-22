using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Object References
    // Should UI managers be singletons or references here?
    [SerializeField] private CameraController _cameraController;
    #endregion
    public bool GameStarted { get; private set; }
    
    public static GameManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }
    
    
    private void Start()
    {
        InputManager.Instance.DisableInput();
    }
    

    public void RestartGame()
    {
        CheckpointManager.Instance.ResetCheckpoints();
        _cameraController.Reset();
        GameplayUIManager.Instance.Reset();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _cameraController.StartGameCameraAnimation(2f);
        Helpers.Instance.Delay(3f, () =>
        {
            InputManager.Instance.EnableInput();
            GameStarted = true;
            GameplayUIManager.Instance.ShowUI();
        });
    }

    
    public void EndGame()
    {
        InputManager.Instance.DisableInput();
        GameStarted = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        EndMenuManager.Instance.ShowEndMenu();
    }
}
