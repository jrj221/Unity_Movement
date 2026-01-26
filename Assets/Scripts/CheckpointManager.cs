using System;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public UIManager uiManager;
    public GameObject firstCheckpoint;
    private Transform latestCheckpoint;
    public bool finishedCourse;
    public GameObject player;


    void Awake()
    {
        latestCheckpoint = firstCheckpoint.transform;
    }


    public void UpdateCheckpoint(Transform checkpoint)
    {

        latestCheckpoint = checkpoint;
    }


    public void Death()
    {
        if (finishedCourse)
        {
            latestCheckpoint = firstCheckpoint.transform;
            finishedCourse = false;
            uiManager.RestartTime();
        }

        player.transform.position = latestCheckpoint.position;
        player.transform.rotation = latestCheckpoint.rotation;
    }
}
