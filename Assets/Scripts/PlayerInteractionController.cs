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
    private GameObject _heldObject = null;

    // Private Values
    private List<InputActionReference> _actionReferences;
    private bool _pressedThrowObject;


    void Awake()
    {
        _actionReferences = new() {pickup, throwObject};
    }


    void LateUpdate()
    {
        if (_heldObject)
        {
            _heldObject.transform.SetPositionAndRotation(holdPoint.position, holdPoint.rotation);
        }
    }


    void OnEnable()
    {
        foreach (InputActionReference actionReference in _actionReferences)
        {
            actionReference.action.Enable();
        }
        pickup.action.started += StartPickup;
        throwObject.action.started += StartThrowObject;
        throwObject.action.performed += PerformThrowObject;
    }


    void OnDisable()
    {
        foreach (InputActionReference actionReference in _actionReferences)
        {
            actionReference.action.Disable();
        }
        pickup.action.started -= StartPickup;
        throwObject.action.started -= StartThrowObject;
        throwObject.action.performed -= PerformThrowObject;
    }


    void StartPickup(InputAction.CallbackContext ctx)
    {
        if (_heldObject) // initiate drop
        {
            PerformDrop();
            _heldObject = null;
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
        _heldObject = other; 
    }


    void PerformDrop()
    {
        _heldObject.GetComponent<BoxCollider>().enabled = true;
        _heldObject.GetComponent<Rigidbody>().useGravity = true;
        // _heldObject.GetComponent<Rigidbody>().isKinematic = false;
    }
    

    void StartThrowObject(InputAction.CallbackContext ctx)
    {
        _pressedThrowObject = true;
    }


    void PerformThrowObject(InputAction.CallbackContext ctx)
    {
        if (_heldObject)
        {
            PerformDrop();
            _heldObject.GetComponent<Rigidbody>().AddForce(cam.transform.forward * 15f, ForceMode.Impulse);
            _heldObject = null;
            _pressedThrowObject = false;
        }
    }
}
