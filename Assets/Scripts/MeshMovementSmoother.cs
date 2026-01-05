using UnityEngine;

public class MeshMovementSmoother : MonoBehaviour
{
    public GameObject player;
    public PlayerMovementController playerController;
    public float movementSmoothingSpeed;


    void LateUpdate()
    {
        // NOTE: Player and arrow meshes are not rotating. Simple fix in the camera script but I haven't done it yet
        if (playerController.forceMeshSnap)
        {
            transform.position = player.transform.position;
            playerController.forceMeshSnap = false;
        } else
        {
            // transform.position = Vector3.Lerp(transform.position, player.transform.position, movementSmoothingSpeed);
            transform.position = player.transform.position;
        }
        transform.rotation = player.transform.rotation;
    }
}
