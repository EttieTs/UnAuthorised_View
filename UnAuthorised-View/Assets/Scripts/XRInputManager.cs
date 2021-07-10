using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRInputManager : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    private InputDevice left;
    private InputDevice right;
    List<InputDevice> devices = new List<InputDevice>();

    void RegisterDevices(InputDevice device)
    {
        if ((device.characteristics & InputDeviceCharacteristics.Left) != 0)
        {
            Debug.Log("Registered left hand");

            left = device;
        }
        if ((device.characteristics & InputDeviceCharacteristics.Right) != 0)
        {
            Debug.Log("Registered right hand");

            right = device;
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        InputDevices.deviceConnected += RegisterDevices;
        InputDevices.GetDevices(devices);

        foreach (InputDevice device in devices)
        {
            RegisterDevices(device);
        }
    }
}
