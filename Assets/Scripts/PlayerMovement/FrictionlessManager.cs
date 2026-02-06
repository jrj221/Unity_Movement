using UnityEngine;

public class FrictionlessManager : MonoBehaviour
{
    public Rigidbody rb;
    public PhysicsMaterial frictionless;

    void OnCollisionEnter(Collision collision)
    {
        Physics.Raycast(rb.transform.position, Vector3.down, out RaycastHit groundHit, 1.1f);
        if (groundHit.collider != collision.collider) // don't apply to ground
        {
            collision.collider.material = frictionless;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        collision.collider.material = null; // reset
    }
}
