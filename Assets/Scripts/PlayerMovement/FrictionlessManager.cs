using UnityEngine;

public class FrictionlessManager : MonoBehaviour
{
    public Rigidbody rb;
    public PhysicsMaterial frictionless;

    private void OnCollisionEnter(Collision collision)
    {
        int ignoreRaycastLayerMask = ~LayerMask.GetMask("Ignore Raycast"); // selects everything EXCEPT IgnoreRaycast layer, thus ignoring those objects
        Physics.Raycast(rb.transform.position, Vector3.down, out RaycastHit groundHit, 2f, ignoreRaycastLayerMask);

        if (groundHit.collider == collision.collider) return; // don't apply to ground
        
        Debug.Log("Frictionless");
        collision.collider.material = frictionless;
    }

    private void OnCollisionExit(Collision collision)
    {
        collision.collider.material = null; // reset
    }
}
