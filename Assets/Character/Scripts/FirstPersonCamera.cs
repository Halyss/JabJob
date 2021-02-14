using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FirstPersonCamera : MonoBehaviour
{
    [Range(0, 1)]
    public float mouseSensitivity = 0.5f;
    public bool lockMouse = true;
    [Range(0, 90)]
    public float TopAngleLimit = 70;
    [Range(0, 90)]
    public float BottomAngleLimit = 70;

    private Transform movementController;
    private float xRotation;
    private float yRotation;

    private float mouseSpeed = 2000;
    private float mouseSmoothing = 1;
    private Vector2 mouseDelta;
    private Vector2 mouseScaling;

    public Camera Camera { get; private set; }

    void Start()
    {
        Camera = GetComponent<Camera>();

        movementController = transform.parent;
        mouseScaling = new Vector2(mouseSensitivity * mouseSpeed, mouseSensitivity * mouseSpeed);

        TopAngleLimit *= -1;

        if(lockMouse)
            LockMouse();
    }

    private void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        HandleMouseRotation();
        HandleMouseLockState();
    }

    private void HandleMouseLockState()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.visible)
                LockMouse();
            else
                UnlockMouse();
        }
    }

    private void HandleMouseRotation()
    {
        mouseDelta.x = Input.GetAxisRaw("Mouse X") * Time.deltaTime;
        mouseDelta.y = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * -1;
        mouseDelta = Vector2.Scale(mouseDelta, mouseScaling);

        xRotation = Mathf.Lerp(xRotation, xRotation + mouseDelta.y, mouseSmoothing);
        xRotation = Mathf.Clamp(xRotation, TopAngleLimit, BottomAngleLimit);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        yRotation = Mathf.Lerp(yRotation, yRotation + mouseDelta.x, mouseSmoothing);
        movementController.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }
}
