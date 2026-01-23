using UnityEngine;

public class PlayerInfoManger : MonoBehaviour
{
    public CheckpointManager checkpointManager;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            Debug.Log("checkpoint!");
            checkpointManager.UpdateCheckpoint(other.transform.position);
        }
        else if (other.CompareTag("Death"))
        {
            Debug.Log("you died!");
            checkpointManager.TeleportPlayer();
        }
    }
}
