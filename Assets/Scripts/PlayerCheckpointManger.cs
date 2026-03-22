using UnityEngine;

public class PlayerInfoManger : MonoBehaviour
{
    // References
    public CheckpointManager checkpointManager;
    public GameplayUIManager gameplayUIManager;


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
            checkpointManager.Death();
            SFXManager.Instance.PlayLaserDeath();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            checkpointManager.UpdateCheckpoint(other.transform);

            if (other.gameObject.name == "Final Checkpoint")
            {
                checkpointManager.finishedCourse = true;
                gameplayUIManager.UpdateBestTime();
            }
        }
    }
}
