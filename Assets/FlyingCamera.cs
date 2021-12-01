using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCamera : MonoBehaviour
{
    public float moveSpeed;
    public float lookSensitivity;

    Transform cameraTransform;
    Vector2 lookRotation;
    Vector3 targetPosition;

    public void Start()
    {
        cameraTransform = GetComponent<Transform>();
        lookRotation = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        targetPosition = cameraTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float forwardMovement = Input.GetAxis("Vertical");
        float rightMovement = Input.GetAxis("Horizontal");

        //Note: deltaTime is NOT required for this, as GetAxis refers to the distance moved this frame
        //Update Rotation (Left/Right - Turret)
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        lookRotation.y += (mouseX);

        //Update Rotation (Up/Down - Gun)
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;
        lookRotation.x -= (mouseY);

        lookRotation.x = Mathf.Clamp(lookRotation.x, -89f, 89f);

        //Set Rotation (All)
        cameraTransform.localEulerAngles = new Vector3(lookRotation.x, lookRotation.y, 0f);

        Vector3 lastPosition = cameraTransform.position;
        targetPosition += ((forwardMovement * cameraTransform.forward) + (rightMovement * cameraTransform.right)) * moveSpeed * Time.deltaTime;
        Vector3 newPosition = targetPosition;
        cameraTransform.position = newPosition;
    }
}
