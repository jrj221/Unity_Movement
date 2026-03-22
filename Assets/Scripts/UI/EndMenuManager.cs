using System;
using UnityEngine;
using UnityEngine.UIElements;

public class EndMenuManager : UIManger
{
    public static EndMenuManager Instance { get; private set; }
    private Button _restartButton;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        
        _restartButton = GetElement<Button>("RestartButton");
        HideUI();
    }

    private void OnEnable()
    {
        _restartButton.RegisterCallback<ClickEvent>(OnRestartButtonClicked);
    }
    
    private void OnDisable()
    {
        _restartButton.UnregisterCallback<ClickEvent>(OnRestartButtonClicked);
    }

    public void ShowEndMenu()
    {
        ShowUI();
        ElementFadeIn(_restartButton, 3f);
    }
    

    private void OnRestartButtonClicked(ClickEvent e)
    {
        HideUI();
        GameManager.Instance.RestartGame();
    }
}
