using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private Label _currentTime;
    private Label _bestTime;
    private float _currentTimeFloat = 0;
    private float _bestTimeFloat = 0;


    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _currentTime = root.Q<Label>("_currentTime");
        _bestTime = root.Q<Label>("_bestTime");
    }


    void Update()
    {
        _currentTimeFloat += Time.deltaTime;
        SetCurrentTime();
    }


    public void SetCurrentTime()
    {
        _currentTime.text = Mathf.Round(_currentTimeFloat).ToString() + "s";
    }


    public void UpdateBestTime()
    {
        if (_currentTimeFloat > _bestTimeFloat)
        {
            _bestTimeFloat = _currentTimeFloat;
            _bestTime.text = "Best Time: " + _currentTime.text;
        }
    }


    public void RestartTime()
    {
        _currentTimeFloat = 0;
    }
}
