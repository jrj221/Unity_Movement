using System;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public GameObject firstCheckpoint;
    private Transform _latestCheckpoint;
    public bool finishedCourse;
    public GameObject player;
    public static CheckpointManager Instance {get ; private set;}

    private void Awake()
    {
        Instance = this;
    }

    public void ResetCheckpoints()
    {
        finishedCourse = false;
        player.transform.SetPositionAndRotation(firstCheckpoint.transform.position, firstCheckpoint.transform.rotation);
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
            GameplayUIManager.Instance.RestartTime();
        }
        player.transform.SetPositionAndRotation(_latestCheckpoint.position, _latestCheckpoint.rotation);
    }
}
