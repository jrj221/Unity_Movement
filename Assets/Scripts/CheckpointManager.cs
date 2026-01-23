using System;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public GameObject firstCheckpoint;
    private Transform latestCheckpoint;
    public GameObject player;


    void Awake()
    {
        latestCheckpoint = firstCheckpoint.transform;
    }


    public void UpdateCheckpoint(Transform checkpoint)
    {

        latestCheckpoint = checkpoint;
    }


    public void TeleportPlayer()
    {
        player.transform.position = latestCheckpoint.position;
        player.transform.rotation = latestCheckpoint.rotation;
    }
}
