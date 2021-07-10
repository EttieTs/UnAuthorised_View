using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;

public class EtMovieController : MonoBehaviour
{
    // ---------------------------------- Movies --------------------------------------
    VideoPlayer videoPlayer;
    public RenderTexture renderTexture;
    private GameObject canvas;
    static bool lastIsUserPresentNow = false;
    int lastMovieWatched = 0;

    // ---------------------------------- Buttons -------------------------------------
    Transform selectedHierarchy;
    Transform unSelectedHierarchy;
    Transform watchedHierarchy;
    GameObject tiltBrushButtons;

    public List<GameObject> butnsSelectedArr = new List<GameObject>();
    public List<GameObject> butUnSelectedArr = new List<GameObject>();
    public List<GameObject> buttonsWatchedArr = new List<GameObject>();

    bool[] isSelected = new bool[6];        // is it selected
    bool[] isWatched = new bool[6];         // is it hidden
     
    float timeSelected = 0;
    const float SelectionTime = 5;          // 5 seconds

    string StartMovie = "et-change-sub.mp4";
    string[] MovieNames = new string[] { "et-change-sub.mp4", // 0
                                         "et-morning-sub.mp4", // 1
                                         "et-muses-sub.mp4", // 2
                                         "et-fire-sub.mp4", // 3
                                         "et-memory-sub.mp4", // 4
                                         "et-discovery-sub.mp4" };  // 5
    // --------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        tiltBrushButtons    = GameObject.Find("TiltBrushButtons");

        // Three lots of button
        selectedHierarchy   = GameObject.Find("Selected").transform;
        unSelectedHierarchy = GameObject.Find("Unselected").transform;
        watchedHierarchy     = GameObject.Find("Hidden").transform;

        // Find the children
        for (int i = 0; i < 6; i++)
        {
            butnsSelectedArr.Add(selectedHierarchy.GetChild(i).gameObject);
            butUnSelectedArr.Add(unSelectedHierarchy.GetChild(i).gameObject);
            buttonsWatchedArr.Add(watchedHierarchy.GetChild(i).gameObject);
        }

        // Setup the video target and callback
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.targetTexture = renderTexture;
        // Each time we reach the end, we slow down the playback by a factor of 10.

        videoPlayer.loopPointReached += EndReached;

        // Find the canvas
        canvas = GameObject.Find("Canvases");

        Reset();
    }

    public void Reset()
    {
        // ---------------------------------- Buttons -------------------------------------
        SetNothingWatched();
        ShowButtons();
        UnselectButtons();

        // ---------------------------------- Movies --------------------------------------
        videoPlayer.Stop();
        lastIsUserPresentNow = IsHeadsetPresent();
        ClearOutRenderTexture(renderTexture);
        PlayStartMovie();
    }

    private void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        ClearOutRenderTexture(renderTexture);

        // Flag the movie watched
        isWatched[lastMovieWatched] = true;

        ShowButtons();

        // you can put code here to play the start video
        PlayStartMovie();
    }

    private void ShowButtons()
    {
        tiltBrushButtons.SetActive(true);
    }

    void HideButtons()
    {
        tiltBrushButtons.SetActive(false);
    }

    void SetButtonStates()
    {
        for (int i = 0; i < 6; i++)
        {
            // Choice between selected and unselected
            butnsSelectedArr[i].SetActive(isSelected[i]);
            butUnSelectedArr[i].SetActive(!isSelected[i]);

            if (isWatched[i] == true)
            {
                butnsSelectedArr[i].SetActive(false);
                butUnSelectedArr[i].SetActive(false);
            }

            // Make the hidden buttons visible or invisible
            buttonsWatchedArr[i].SetActive( isWatched[i]);
        }
    }

    public void ClearOutRenderTexture(RenderTexture renderTexture)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }

    void PlayMovie(string movieName)
    {
        string path;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            // normally where Unity's assets are
            path = Path.Combine(Application.dataPath, "movies");
            path = Path.Combine(path, movieName);
        }
        else
        {
            // Persistent - because it's in the data folder of the app
            // e.g. for saved games, tilt brush outputs etc.
            path = Path.Combine(Application.persistentDataPath, movieName);
        }
        videoPlayer.url = path;
        videoPlayer.Play();
    }

    void PlayStartMovie()
    {
        //TpControl.lastButtonUsed = GameObject.Find("Button 1");

        //Debug.Log("Play movie 1");

        PlayMovie(StartMovie);
    }

    void PlayMovie(int index)
    {
        lastMovieWatched = index;
        string name = MovieNames[index];
        PlayMovie(name);
        HideButtons();
    }

    void Pause()
    {
        //Debug.Log("Pause");
        videoPlayer.Pause();
    }
    bool IsHeadsetPresent()
    {
        // Stop the video when the headset is taken off
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (device.isValid)
        {
            bool isUserPresentNow = true;

            if (device.TryGetFeatureValue(CommonUsages.userPresence, out isUserPresentNow))
            {
                return isUserPresentNow;
            }
        }
        return false;
    }

    void DebugKeys()
    {
        if (BWKeyboard.GetKey1Down())
        {
            PlayMovie(0);
        }
        if (BWKeyboard.GetKey2Down())
        {
            PlayMovie(1);
        }
        if (BWKeyboard.GetKey3Down())
        {
            PlayMovie(2);
        }
        if (BWKeyboard.GetKey4Down())
        {
            PlayMovie(3);
        }
        if (BWKeyboard.GetKey5Down())
        {
            PlayMovie(4);
        }
        if (BWKeyboard.GetKey6Down())
        {
            PlayMovie(5);
        }

        if (BWKeyboard.GetKeyW())
        {
            isWatched[lastMovieWatched] = true;
        }

        if (BWKeyboard.GetKeyF1())
        {
            isSelected[0] = !isSelected[0];
        }
        if (BWKeyboard.GetKeyF2())
        {
            isSelected[1] = !isSelected[1];
        }
        if (BWKeyboard.GetKeyF3())
        {
            isSelected[2] = !isSelected[2];
        }
        if (BWKeyboard.GetKeyF4())
        {
            isSelected[3] = !isSelected[3];
        }
        if (BWKeyboard.GetKeyF5())
        {
            isSelected[4] = !isSelected[4];
        }
        if (BWKeyboard.GetKeyF6())
        {
            isSelected[5] = !isSelected[5];
        }
    }

    void RaycastGaze()
    {
        Transform cameraTransform = Camera.main.transform;

        Vector3 eyePosition = cameraTransform.position;

        RaycastHit hit;

        if (Physics.Raycast(eyePosition, cameraTransform.forward, out hit, Mathf.Infinity))
        {
            int index = hit.transform.GetSiblingIndex();

            Debug.Log("HIT index: " + index + " name " + hit.transform.name);

            isSelected[index] = true;

            // Increment the time selected
            timeSelected += Time.deltaTime;

            if (timeSelected > SelectionTime)
            {
                if (isWatched[index] == false)
                {
                    PlayMovie(index);
                }
            }
        }
        else
        {
            timeSelected = 0;
        }
    }

    void SetNothingWatched()
    {
        // The buttons are unselected here
        for (int i = 0; i < 6; i++)
        {
            isWatched[i] = false;
        }
    }

    void UnselectButtons()
    {
        // The buttons are unselected here
        for (int i = 0; i < 6; i++)
        {
            isSelected[i] = false;
        }
    }

    void Update()
    {
        UnselectButtons();
        RaycastGaze();
        DebugKeys();
        SetButtonStates();

        if (BWKeyboard.GetKeyEscapeDown())
        {
            Debug.Log("Escape");
            Reset();
        }

        Transform camera = GameObject.Find("Main Camera").transform;

        Vector3 yawPitchRoll = camera.eulerAngles;

        // Debug.Log(yawPitchRoll.x);

        if( (yawPitchRoll.x > 45.0f) && (yawPitchRoll.x < 60.0f ) )
        {
            Debug.Log("Looking down");
            ShowButtons();
        }

        // Check if headset is present
        bool isUserPresentNow = IsHeadsetPresent();

        // Has this changed since last time
        if (isUserPresentNow != lastIsUserPresentNow)
        {
            if (isUserPresentNow)
            {
                BWEventManager.TriggerEvent("Headset On");
            }
            else
            {
                BWEventManager.TriggerEvent("Headset Off");

                Reset();
            }

            // Keep track of the change
            lastIsUserPresentNow = isUserPresentNow;
        }
    }
}
