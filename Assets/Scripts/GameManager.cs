using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Object References
    // Should UI managers be singletons or references here?
    [SerializeField] private PauseMenuUIManager pauseMenuUIManager;
    #endregion
    
    public static GameManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }
    
    
    private void Start()
    {
        Time.timeScale = 0f;
    }
    

    public void StartGame()
    {
        pauseMenuUIManager.ShowUI();
        InputManager.Instance.EnableInput();
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}
