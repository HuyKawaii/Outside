using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController: MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private PlayerController playerController;

    private float xMouse, yMouse;
    private float verticalRotation = 0.0f;
    float mouseSen = 100.0f;
    

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController.IsOwner) return;
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        xMouse = Input.GetAxis("Mouse X") * mouseSen * Time.deltaTime;
        yMouse = Input.GetAxis("Mouse Y") * mouseSen * Time.deltaTime;

        verticalRotation -= yMouse;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * xMouse);
    }
}
