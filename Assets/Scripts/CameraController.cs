using System.IO.Pipes;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{   
    public GameObject player;
    public StateMachine controller;
    public InputActionReference look;
    private Vector2 deltaCameraMovement;

    public float sensitivity; // 1 works good. Higher values are more sensitive
    public float cameraMovementSmoothingSpeed;
    private float pitch;
    private float yaw;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void OnEnable()
    {
        look.action.Enable();
        look.action.performed += PerformLook;
    }


    void OnDisable()
    {
        look.action.Disable();
        look.action.performed -= PerformLook;
    }


    void LateUpdate()
    {
        MoveCamera();
    }


    void PerformLook(InputAction.CallbackContext ctx)
    {
        deltaCameraMovement = ctx.ReadValue<Vector2>();
    }


    void MoveCamera()
    {
        // cam position
        if (controller.cameraSmoothingEnabled) transform.position = Vector3.Lerp(transform.position, player.transform.position + new Vector3(0f, 0.4f, 0f), cameraMovementSmoothingSpeed);
        else transform.position = player.transform.position + new Vector3(0f, 0.4f, 0f);

        // player yaw
        yaw += deltaCameraMovement.x * sensitivity;
        Vector3 currPlayerRotation = player.transform.eulerAngles;
        player.transform.eulerAngles = new Vector3(currPlayerRotation.x, yaw, currPlayerRotation.z);

        // cam roll and yaw
        Vector3 currRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currRotation.x, player.transform.eulerAngles.y, player.transform.eulerAngles.z);
        
        // cam pitch
        float currPitch = pitch;
        pitch += deltaCameraMovement.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        transform.Rotate(currPitch - pitch, 0f, 0f);
    }
}
