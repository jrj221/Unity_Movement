using System;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Vector3 latestCheckpoint;

    public void UpdateCheckpoint(Vector3 checkpoint)
    {
        latestCheckpoint = checkpoint;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
