using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    // Serialized References
    public InputActionReference throwObject;
    public InputActionReference pickup;
    public GameObject cam;
    public Transform holdPoint;

    // Private References
    private GameObject heldObject = null;

    // Private Values
    private List<InputActionReference> actionReferences;
    private bool pressedThrowObject;


    void Awake()
    {
        actionReferences = new() {pickup, throwObject};
    }


    void LateUpdate()
    {
        if (heldObject)
        {
            heldObject.transform.SetPositionAndRotation(holdPoint.position, holdPoint.rotation);
        }
    }


    void OnEnable()
    {
        foreach (InputActionReference actionReference in actionReferences)
        {
            actionReference.action.Enable();
        }
        pickup.action.started += StartPickup;
        throwObject.action.started += StartThrowObject;
        throwObject.action.performed += PerformThrowObject;
    }


    void OnDisable()
    {
        foreach (InputActionReference actionReference in actionReferences)
        {
            actionReference.action.Disable();
        }
        pickup.action.started -= StartPickup;
        throwObject.action.started -= StartThrowObject;
        throwObject.action.performed -= PerformThrowObject;
    }


    void StartPickup(InputAction.CallbackContext ctx)
    {
        if (heldObject) // initiate drop
        {
            PerformDrop();
            heldObject = null;
        } else // initiate pickup
        {
            bool lookingAtObject = Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 3);
            Debug.DrawRay(cam.transform.position, cam.transform.forward * 3f, Color.green);
            if (lookingAtObject && hit.transform.gameObject.CompareTag("Pickupable"))
            {
                PerformPickup(hit);
            } 
        }
    }


    void PerformPickup(RaycastHit hit)
    {
        GameObject other = hit.transform.gameObject;
        other.GetComponent<BoxCollider>().enabled = false;
        other.GetComponent<Rigidbody>().useGravity = false;
        // other.GetComponent<Rigidbody>().isKinematic = true;
        heldObject = other; 
    }


    void PerformDrop()
    {
        heldObject.GetComponent<BoxCollider>().enabled = true;
        heldObject.GetComponent<Rigidbody>().useGravity = true;
        // heldObject.GetComponent<Rigidbody>().isKinematic = false;
    }
    

    void StartThrowObject(InputAction.CallbackContext ctx)
    {
        pressedThrowObject = true;
    }


    void PerformThrowObject(InputAction.CallbackContext ctx)
    {
        if (heldObject)
        {
            PerformDrop();
            heldObject.GetComponent<Rigidbody>().AddForce(cam.transform.forward * 15f, ForceMode.Impulse);
            heldObject = null;
            pressedThrowObject = false;
        }
    }
}
