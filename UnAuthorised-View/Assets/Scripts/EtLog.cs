using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EtLog : MonoBehaviour
{
    string fileName;
    Vector3 lastEuler = Vector3.zero;

    //Start is called before the first frame update
    void Start()
    {
        // Persistent - because it's in the data folder of the app
        // e.g. for saved games, tilt brush outputs etc.
        fileName = Path.Combine(Application.persistentDataPath, "log.txt");
        Debug.Log("Log file created at: " + fileName);
        Write("Log started");

        // Initialise these to get full logs if you're deploying this
        //BWEventManager.StartListening("Play Movie1", PlayMovie1);
        //BWEventManager.StartListening("Play Movie2", PlayMovie2);
        //BWEventManager.StartListening("Play Movie3", PlayMovie3);
        //BWEventManager.StartListening("Hide Buttons", HideButtons);
        //BWEventManager.StartListening("Headset On", HeadsetOn);
        //BWEventManager.StartListening("Headset Off", HeadsetOff);
    }

    void Write(string logging)
    {
        DateTime dt = DateTime.Now;
        string dateTime = dt.ToString("MM/dd/yyyy HH:mm:ss");
        StreamWriter writer = new StreamWriter(fileName, true);
        string output = dateTime + "," + logging;
        writer.WriteLine(output);
        writer.Close();

        Debug.Log(logging);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject headGameObject;

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            headGameObject = GameObject.Find("Camera Controller");
        }
        else
        {
            headGameObject = GameObject.Find("Main Camera");
        }

        Transform headTransform = headGameObject.transform;

        Vector3 euler = headGameObject.transform.rotation.eulerAngles;

        float diffx = Math.Abs( euler.x - lastEuler.x );
        float diffy = Math.Abs( euler.y - lastEuler.y );
        float diffz = Math.Abs( euler.z - lastEuler.z );

        /*
        // Log the gaze direction

        if (diffx > 30 || diffy > 30 || diffz > 30)
        {
            lastEuler = euler;

            string eulerString = string.Format("{0:0.00}", euler.x ) + "," +
                                 string.Format("{0:0.00}", euler.y) + "," +
                                 string.Format("{0:0.00}", euler.z);

            Write(eulerString);
        } 
        */
    }

    void PlayMovie1()
    {
        Write("Playing Movie 1");
    }

    void PlayMovie2()
    {
        Write("Playing Movie 2");
    }

    void PlayMovie3()
    {
        Write("Playing Movie 3");
    }

    void HideButtons()
    {
        Write("Hiding Buttons");
    }

    void HeadsetOn()
    {
        Write("On");
    }
    void HeadsetOff()
    {
        Write("Off");
    }
}
