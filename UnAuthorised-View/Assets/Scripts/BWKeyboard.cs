using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BWKeyboard : MonoBehaviour
{
    public static bool GetKeyLeftShift()
    {
        return Keyboard.current.leftShiftKey.isPressed;
    }

    public static bool GetKeyLeft()
    {
        return Keyboard.current.leftArrowKey.isPressed;
    }
    public static bool GetKeyRight()
    {
        return Keyboard.current.rightArrowKey.isPressed;
    }


    public static bool GetKey1Down()
    {
        return Keyboard.current.digit1Key.wasPressedThisFrame;
    }
    public static bool GetKey2Down()
    {
        return Keyboard.current.digit2Key.wasPressedThisFrame;
    }
    public static bool GetKey3Down()
    {
        return Keyboard.current.digit3Key.wasPressedThisFrame;
    }
    public static bool GetKey4Down()
    {
        return Keyboard.current.digit4Key.wasPressedThisFrame;
    }
    public static bool GetKey5Down()
    {
        return Keyboard.current.digit5Key.wasPressedThisFrame;
    }
    public static bool GetKey6Down()
    {
        return Keyboard.current.digit6Key.wasPressedThisFrame;
    }

    // Function keys
    public static bool GetKeyW()
    {
        return Keyboard.current.wKey.isPressed;
    }

    public static bool GetKeyF1()
    {
        return Keyboard.current.f1Key.isPressed;
    }
    public static bool GetKeyF2()
    {
        return Keyboard.current.f2Key.isPressed;
    }
    public static bool GetKeyF3()
    {
        return Keyboard.current.f3Key.isPressed;
    }
    public static bool GetKeyF4()
    {
        return Keyboard.current.f4Key.isPressed;
    }
    public static bool GetKeyF5()
    {
        return Keyboard.current.f5Key.isPressed;
    }
    public static bool GetKeyF6()
    {
        return Keyboard.current.f6Key.isPressed;
    }

    public static bool GetKeyEscapeDown()
    {
        return Keyboard.current.escapeKey.isPressed;
    }
}
