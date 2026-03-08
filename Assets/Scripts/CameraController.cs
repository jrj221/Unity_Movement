using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{   
    public GameObject player;
    public StateMachine controller;

    public float sensitivity; // 1 works good. Higher values are more sensitive
    public float cameraMovementSmoothingSpeed;
    private float _pitch;
    private float _yaw;
    public InputManager inputManager;


    private void LateUpdate()
    {
        MoveCamera();
    }


    private void MoveCamera()
    {
        // cam position
        if (controller.cameraSmoothingEnabled) transform.position = Vector3.Lerp(transform.position, player.transform.position + new Vector3(0f, 0.4f, 0f), cameraMovementSmoothingSpeed);
        else transform.position = player.transform.position + new Vector3(0f, 0.4f, 0f);

        // player _yaw
        _yaw += inputManager.DeltaCameraMovement.x * sensitivity;
        Vector3 currPlayerRotation = player.transform.eulerAngles;
        player.transform.eulerAngles = new Vector3(currPlayerRotation.x, _yaw, currPlayerRotation.z);

        // cam roll and _yaw
        Vector3 currRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currRotation.x, player.transform.eulerAngles.y, player.transform.eulerAngles.z);
        
        // cam _pitch
        float currPitch = _pitch;
        _pitch += inputManager.DeltaCameraMovement.y * sensitivity;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);
        transform.Rotate(currPitch - _pitch, 0f, 0f);
    }

    public void StartGameCameraAnimation(float duration)
    {
        StartCoroutine(StartGameCameraAnimationRoutine(duration));
    }

    private IEnumerator StartGameCameraAnimationRoutine(float duration)
    {
        for (var i = 0; i < 180f; i++)
        {
            Debug.Log("in loop");
            transform.Rotate(0.5f, 0, 0, Space.World);
            yield return new WaitForSeconds(duration / 180f);
        }
    }
}