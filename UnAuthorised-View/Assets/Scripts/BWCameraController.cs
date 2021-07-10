using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(BWFlyCamera))]
public class BWCameraController : MonoBehaviour
{
    // The different camera types available
    public enum CameraType
    {
        Standard,
        OpenVR,
        Oculus,
    }

    public bool SupportXR = false;

    public CameraType cameraType;

    static public Camera currentCamera;

    static public BWCameraController instance;

    static public bool isCameraMoving = false;

    static NavMeshAgent navMeshAgent;

    // Available cameras
    public static BWFlyCamera flyCamera;

    // Start is called before the first frame update
    static public string GetCameraFilename(string cameraName)
    {
        return Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + cameraName;
    }

    void Start()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-virtualreality")
            {
                SupportXR = false;
            }
        }

        flyCamera = GetComponent<BWFlyCamera>();
        
        if (!SupportXR)
        {
            Debug.Log("XR Supported");
            UnityEngine.XR.XRSettings.enabled = false;
        }
        else
        {
            Debug.Log("XR device name: " + UnityEngine.XR.XRSettings.loadedDeviceName );
        }

        // Disable the cameras we are not using
        if(UnityEngine.XR.XRSettings.loadedDeviceName == "OpenVR")
        {
            cameraType = CameraType.OpenVR;
            currentCamera = GetComponentInChildren<Camera>();
            gameObject.SetActive(true);
        }

        Debug.Log("Setup camera for: " + System.Enum.GetName(typeof(CameraType), cameraType));

        instance = this;
    }

    ////////////////////////////////////////////////////////////////////////////////
    // RecalibrateVirtualRealityPose

    System.Collections.IEnumerator RecalibrateVirtualRealityPose()
    {
        // Recenter the pose
        //UnityEngine.XR.XRDevice.SetTrackingSpaceType(UnityEngine.XR.TrackingSpaceType.Stationary);
        //OpenVR.System.ResetSeatedZeroPose();

        yield return new WaitForSeconds(0.5f);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        BWEventManager.TriggerEvent("ResetView");
    }

    ////////////////////////////////////////////////////////////////////////////////
    // RecalibrateStandardCamera

    void RecalibrateStandardCamera()
    {
        BWEventManager.TriggerEvent("ResetView");
    }

    ////////////////////////////////////////////////////////////////////////////////
    // ResetVR

    void ResetCamera()
    {
        Debug.Log("ResetVR");

        if (cameraType == CameraType.OpenVR)
        {
            StartCoroutine(RecalibrateVirtualRealityPose());
        }
        else
        {
            RecalibrateStandardCamera();
        }
    }

    // Update is called once per frame
    void Update()
    {
        isCameraMoving = false;

#if ENABLE_INPUT_SYSTEM

        Mouse mouse = Mouse.current;

        if (mouse.IsPressed(1))
        {
            isCameraMoving = true;
        }

#else
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isCameraMoving = true;
        }
#endif

        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            Debug.Log("VR present");
        }
    }
}
