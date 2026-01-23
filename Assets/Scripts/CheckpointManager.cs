using System;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public GameObject firstCheckpoint;
    private Vector3 latestCheckpoint;
    public GameObject player;


    void Awake()
    {
        latestCheckpoint = firstCheckpoint.transform.position;
    }


    public void UpdateCheckpoint(Vector3 checkpoint)
    {

        latestCheckpoint = checkpoint;
    }


    public void TeleportPlayer()
    {
        player.transform.position = latestCheckpoint;
    }
}
