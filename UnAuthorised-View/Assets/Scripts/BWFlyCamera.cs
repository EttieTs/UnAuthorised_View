using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BWFlyCamera : MonoBehaviour
{
    [System.Serializable]
    public class CameraState
    {
        public float yaw;
        public float pitch;
        public float roll;

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
            t.position = new Vector3(0, 0, 0);
        }
    }
    
    // The camera's state
    public CameraState nextCameraState = new CameraState();

    [Header("Rotation")]
    [Tooltip("Speed of rotation in yaw, pitch, roll")]
    public float rotationSpeed = 1.0f;

    [Tooltip("Invert Y axis (more like an airplane yoke)")]
    public bool invertY = false;

    void OnDestroy()
    {
        //Save();
    }

    void OnEnable()
    {
        nextCameraState.UpdateTransform(transform);
    }

    void RotateWithKeys()
    {
#if ENABLE_INPUT_SYSTEM

        if ( BWKeyboard.GetKeyLeft() )
        {
            nextCameraState.yaw -= rotationSpeed * Time.deltaTime * 20;
        }
        if (BWKeyboard.GetKeyRight())
        {
            nextCameraState.yaw += rotationSpeed * Time.deltaTime * 20;
        }

#else
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            nextCameraState.yaw -= rotationSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            nextCameraState.yaw += rotationSpeed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            nextCameraState.pitch += rotationSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            nextCameraState.pitch -= rotationSpeed;
        }
#endif
    }

    void Update()
    {
        //Debug.Log("BWFlyCamera.Update");
        if (BWCameraController.isCameraMoving)
        {
            float yaw = 0;
            float pitch = 0;

            if (BWKeyboard.GetKeyLeftShift())
            {

#if ENABLE_INPUT_SYSTEM

                Mouse mouse = Mouse.current;

                // Get the mouse movement
                float x = mouse.delta.x.ReadValue();
                float y = mouse.delta.y.ReadValue();
                Vector2 delta = new Vector2(x, y);

                yaw += delta.x * Time.deltaTime;
#else
                // Update the rotation
                yaw += Input.GetAxis("Mouse X");
                pitch -= Input.GetAxis("Mouse Y");
#endif

                if (invertY)
                {
                    pitch = pitch * -1;
                }                
            }

            nextCameraState.yaw += yaw;
            nextCameraState.pitch += pitch;

            // Calculate the movement from keys
            RotateWithKeys();

            Vector3 direction = new Vector3(0, 0, 0);

#if ENABLE_INPUT_SYSTEM
#else
            // Speed up movement when shift key held
            if (Input.GetKey(KeyCode.LeftShift))
            {
                translation *= keyboardSpeedBoost;
            }
#endif         

            // Update the camera transform
            nextCameraState.UpdateTransform(transform);
        }
    }
}