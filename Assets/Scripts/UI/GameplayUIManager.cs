using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIManager : UIManger
{
    private Label _currentTime;
    private Label _bestTime;
    private float _currentTimeFloat = 0;
    private float _bestTimeFloat = 0;


    protected override void Awake()
    {
        base.Awake();
        _currentTime = GetElement<Label>("CurrentTime");
        _bestTime = GetElement<Label>("BestTime");
    }


    private void Update()
    {
        _currentTimeFloat += Time.deltaTime;
        SetCurrentTime();
    }


    private void SetCurrentTime()
    {
        _currentTime.text = Mathf.Round(_currentTimeFloat) + "s";
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
