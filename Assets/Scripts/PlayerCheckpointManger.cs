using UnityEngine;

public class PlayerInfoManger : MonoBehaviour
{
    // References
    public CheckpointManager checkpointManager;
    public UIManager uiManager;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            checkpointManager.UpdateCheckpoint(other.transform);

            if (other.name == "Final Checkpoint")
            {
                uiManager.UpdateBestTime();
            }
        }
        else if (other.CompareTag("Death"))
        {
            checkpointManager.TeleportPlayer();
        }
    }
}
