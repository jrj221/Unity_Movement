using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Object References
    // Should UI managers be singletons or references here?
    [SerializeField] private CameraController _cameraController;
    private GameObject _player;
    #endregion
    public bool GameStarted { get; private set; }
    
    public static GameManager Instance { get; private set; }


    private void Awake()
    {
        Application.targetFrameRate = 90;
        Instance = this;
        _player = GameObject.Find("PlayerRoot");
        GameObject.Find("PlayerMesh").GetComponent<MeshRenderer>().enabled = false;
    }

    private void ResetPlayer()
    {
        // Teleports the player back to the start, and rotates them and the camera into starting positions
        CheckpointManager.Instance.ResetCheckpoints();
        _cameraController.Reset();
        _player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }
    
    
    private void Start()
    {
        InputManager.Instance.DisableInput();
    }
    

    public void RestartGame()
    {
        GameStarted = false;
        InputManager.Instance.DisableInput();
        GameplayUIManager.Instance.HideUI();
        ResetPlayer();
        GameplayUIManager.Instance.Reset();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _cameraController.StartGameCameraAnimation(2f);
        Helpers.Instance.Delay(2.5f, () =>
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
