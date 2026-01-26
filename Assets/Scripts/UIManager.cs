using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private Label currentTime;
    private Label bestTime;
    private float currentTimeFloat = 0;
    private float bestTimeFloat = 0;


    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        currentTime = root.Q<Label>("currentTime");
        bestTime = root.Q<Label>("bestTime");
    }


    void Update()
    {
        currentTimeFloat += Time.deltaTime;
        SetCurrentTime();
    }


    public void SetCurrentTime()
    {
        currentTime.text = Mathf.Round(currentTimeFloat).ToString() + "s";
    }


    public void UpdateBestTime()
    {
        if (currentTimeFloat > bestTimeFloat)
        {
            bestTimeFloat = currentTimeFloat;
            bestTime.text = "Best Time: " + currentTime.text;
        }
    }
}
