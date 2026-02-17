using System;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public UIManager uiManager;
    public GameObject firstCheckpoint;
    private Transform _latestCheckpoint;
    public bool finishedCourse;
    public GameObject player;


    private void Awake()
    {
        _latestCheckpoint = firstCheckpoint.transform;
    }


    public void UpdateCheckpoint(Transform checkpoint)
    {
        _latestCheckpoint = checkpoint;
    }


    public void Death()
    {
        if (finishedCourse)
        {
            _latestCheckpoint = firstCheckpoint.transform;
            finishedCourse = false;
            uiManager.RestartTime();
        }
        // player.transform.SetPositionAndRotation(_latestCheckpoint.position, _latestCheckpoint.rotation);
        player.transform.position = _latestCheckpoint.position;
        player.transform.rotation = _latestCheckpoint.rotation;
    }
}
