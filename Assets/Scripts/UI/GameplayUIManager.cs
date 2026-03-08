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
        if (!GameManager.Instance.GameStarted) return; 
        
        _currentTimeFloat += Time.deltaTime;
        SetCurrentTime();
    }


    private void SetCurrentTime()
    {
        _currentTime.text = Math.Round(_currentTimeFloat, 2) + "s";
    }


    public void UpdateBestTime()
    {
        if (_currentTimeFloat < _bestTimeFloat) return; // Failed to get better time
        
        _bestTimeFloat = _currentTimeFloat;
        _bestTime.text = "Best Time: " + _currentTime.text;
    }


    public void RestartTime()
    {
        _currentTimeFloat = 0;
    }
}
