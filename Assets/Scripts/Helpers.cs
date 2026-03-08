
using System;
using System.Collections;
using UnityEngine;

public class Helpers : MonoBehaviour
{
    public static Helpers Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    public void CheckNull<T>(T variable, string variableName)
    {
        if (variable == null) {
            Debug.Log($"{variableName} is null");
        }
    }
    
    public void Delay(float delay, Action action)
    {
        StartCoroutine(DelayRoutine(delay, action));
    }

    private IEnumerator DelayRoutine(float time, Action callback)
    {
        yield return new WaitForSecondsRealtime(time);
        callback();
    }
}
