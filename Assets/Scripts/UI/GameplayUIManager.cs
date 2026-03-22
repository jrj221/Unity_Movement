using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIManager : UIManger
{
    private Label _currentTime;
    private Label _bestTime;
    private VisualElement _best;
    private float _currentTimeFloat;
    private float _bestTimeFloat;

    public static GameplayUIManager Instance { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        _currentTime = GetElement<Label>("CurrentTime");
        _bestTime = GetElement<Label>("BestTime");
        _best = GetElement<VisualElement>("Best");
        
        HideUI(); // by default
        HideElement(_best);
    }


    private void Update()
    {
        if (!GameManager.Instance.GameStarted || CheckpointManager.Instance.finishedCourse) return; 
        
        _currentTimeFloat += Time.deltaTime;
        SetCurrentTime();
    }


    private void SetCurrentTime()
    {
        _currentTime.text = (100_000 - (200 * _currentTimeFloat)).ToString("N0");
    }


    public void UpdateBestTime()
    {
        if (_currentTimeFloat < _bestTimeFloat) return; // Failed to get better time
        
        _bestTimeFloat = _currentTimeFloat;
        _bestTime.text = _currentTime.text;
        ShowElement(_best);
    }


    public void RestartTime()
    {
        _currentTimeFloat = 0;
    }
}
