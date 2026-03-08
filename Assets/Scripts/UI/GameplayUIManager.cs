using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIManager : UIManger
{
    private Label _currentTime;
    private Label _bestTime;
    private float _currentTimeFloat = 0;
    private float _bestTimeFloat = 0;

    public static GameplayUIManager Instance { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        _currentTime = GetElement<Label>("CurrentTime");
        _bestTime = GetElement<Label>("BestTime");
        
        HideUI(); // by default
        HideElement(_bestTime);
    }


    private void Update()
    {
        if (!GameManager.Instance.GameStarted || CheckpointManager.Instance.finishedCourse) return; 
        
        _currentTimeFloat += Time.deltaTime;
        SetCurrentTime();
    }


    private void SetCurrentTime()
    {
        _currentTime.text = _currentTimeFloat.ToString("F2") + "s";
    }


    public void UpdateBestTime()
    {
        if (_currentTimeFloat < _bestTimeFloat) return; // Failed to get better time
        
        _bestTimeFloat = _currentTimeFloat;
        _bestTime.text = "Best: " + _currentTime.text;
        ShowElement(_bestTime);
    }


    public void RestartTime()
    {
        _currentTimeFloat = 0;
    }
}
