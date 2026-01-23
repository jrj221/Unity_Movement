using System;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Vector3 latestCheckpoint;
    public GameObject player;


    public void UpdateCheckpoint(Vector3 checkpoint)
    {

        latestCheckpoint = checkpoint;
    }


    public void TeleportPlayer()
    {
        Debug.Log("teleported");
        player.transform.position = latestCheckpoint;
    }
}
