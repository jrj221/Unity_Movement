using UnityEngine;

public class PlayerInfoManger : MonoBehaviour
{
    public CheckpointManager checkpointManager;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            checkpointManager.UpdateCheckpoint(other.transform);
        }
        else if (other.CompareTag("Death"))
        {
            checkpointManager.TeleportPlayer();
        }
    }
}
