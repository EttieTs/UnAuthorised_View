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
    int lastMovieWatched = 0;

    // ------------------------------ TiltBrush Spheres -------------------------------
    Transform selectedHierarchy;
    Transform unSelectedHierarchy;
    Transform watchedHierarchy;
    GameObject tiltBrushSpheres;

    public List<GameObject> interactionTargetSelectedArr = new List<GameObject>();
    public List<GameObject> interactionTargetUnSelectedArr = new List<GameObject>();
    public List<GameObject> ineractionTargetWatchedArr = new List<GameObject>();

    bool[] isSelected = new bool[6];        // is it selected
    bool[] isWatched = new bool[6];         // is it hidden
     
    float timeSelected = 0;
    const float SelectionTime = 5;          // 5 seconds

    string StartMovie = "et-ls-startspace_2_1.mp4";
    string[] MovieNames = new string[] { "et-change-sub.mp4", // 0
                                         "et-morning-sub.mp4", // 1
                                         "et-muses-sub.mp4", // 2
                                         "et-fire-sub.mp4", // 3
                                         "et-memory-sub.mp4", // 4
                                         "et-discovery-sub.mp4" };  // 5

    // ----------------------------- Look Around You ---------------------------------

    GameObject lookAroundYou;
    GameObject lookAroundYouText;
    Vector3 lastEuler1, lastEuler2;
    int lookAroundYouCount = 0;
    const int LookAroundAmount = 50;

    // ---------------------------------- State -------------------------------------
    enum ExperienceState
    {
        ResetExperience,
        WaitingForHeadsetToBeWorn,
        LookAround,
        StartPlayingMovies,
        PlayingMovies,
    };

    ExperienceState experienceState;

    // --------------------------------------------------------------------------------

    void SetExperienceState(ExperienceState newState)
    {
        experienceState = newState;
    }
    // --------------------------------------------------------------------------------


    // Start is called before the first frame update
    void Start()
    {
        tiltBrushSpheres = GameObject.Find("TiltBrushSpheres");

        // Three types of interaction target
        selectedHierarchy   = GameObject.Find("Selected").transform;
        unSelectedHierarchy = GameObject.Find("Unselected").transform;
        watchedHierarchy     = GameObject.Find("Hidden").transform;

        // Find the children
        for (int i = 0; i < 6; i++)
        {
            interactionTargetSelectedArr.Add(selectedHierarchy.GetChild(i).gameObject);
            interactionTargetUnSelectedArr.Add(unSelectedHierarchy.GetChild(i).gameObject);
            ineractionTargetWatchedArr.Add(watchedHierarchy.GetChild(i).gameObject);
        }

        // Setup the video target and callback
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.targetTexture = renderTexture;
        // Each time we reach the end, we slow down the playback by a factor of 10.

        videoPlayer.loopPointReached += EndReached;

        // Find the look around you text
        lookAroundYou = GameObject.Find("LookAroundYou");
        lookAroundYouText = GameObject.Find("LookAroundYouText");

        // Start the experience as Resetted
        SetExperienceState(ExperienceState.ResetExperience);
    }

    public void Reset()
    {
        // ---------------------------- Interaction Targets --------------------------
        SetNothingWatched();
        HideButtons();
        UnselectButtons();

        // ------------------------------- Look Around You -----------------------------
        lookAroundYou.SetActive(true);
        lookAroundYouCount = 0;

        // ---------------------------------- Movies --------------------------------------
        videoPlayer.Stop();
        ClearOutRenderTexture(renderTexture);
        PlayStartMovie();
    }

    private void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        // Check if it's a normal movie that's stopped - or the start movie
        if (vp.url.Contains(StartMovie) == false)
        {
            ClearOutRenderTexture(renderTexture);

            // Flag the movie watched
            isWatched[lastMovieWatched] = true;

            ShowInteractionTargets();
        }

        // you can put code here to play the start video
        PlayStartMovie();
    }

    private void ShowInteractionTargets()
    {
        tiltBrushSpheres.SetActive(true);
    }

    void HideButtons()
    {
        tiltBrushSpheres.SetActive(false);
    }

    void SetButtonStates()
    {
        for (int i = 0; i < 6; i++)
        {
            // Choice between selected and unselected
            interactionTargetSelectedArr[i].SetActive(isSelected[i]);
            interactionTargetUnSelectedArr[i].SetActive(!isSelected[i]);

            if (isWatched[i] == true)
            {
                interactionTargetSelectedArr[i].SetActive(false);
                interactionTargetUnSelectedArr[i].SetActive(false);
            }

            // Make the hidden buttons visible or invisible
            ineractionTargetWatchedArr[i].SetActive( isWatched[i]);
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
    bool IsHeadsetWorn()
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

        if (BWKeyboard.GetKeyEscapeDown())
        {
            Debug.Log("Escape");
            SetExperienceState(ExperienceState.ResetExperience);
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

            //Debug.Log("HIT index: " + index + " name " + hit.transform.name);

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


    void Update_WatchingMovies()
    {
        UnselectButtons();
        RaycastGaze();
        DebugKeys();
        SetButtonStates();

        Transform camera = GameObject.Find("Main Camera").transform;

        Vector3 yawPitchRoll = camera.eulerAngles;

        // Debug.Log(yawPitchRoll.x);

        if ((yawPitchRoll.x > 45.0f) && (yawPitchRoll.x < 60.0f))
        {
            Debug.Log("Looking down");
            ShowInteractionTargets();
        }
    }

    private void LookAroundYou1()
    {
        Vector3 euler = GameObject.Find("Camera Controller").transform.rotation.eulerAngles;  // When we're look around in the keyboard

        float diffx = Mathf.Abs(euler.x - lastEuler1.x);
        float diffy = Mathf.Abs(euler.y - lastEuler1.y);
        float diffz = Mathf.Abs(euler.z - lastEuler1.z);

        // Log the gaze direction

        if (diffx > 30 || diffy > 30 || diffz > 30)
        {
            ++lookAroundYouCount;
            lastEuler1 = euler;
        }
    }

    private void LookAroundYou2()
    {
        Vector3 euler = GameObject.Find("Main Camera").transform.rotation.eulerAngles; // When we're wearing the headset

        float diffx = Mathf.Abs(euler.x - lastEuler2.x);
        float diffy = Mathf.Abs(euler.y - lastEuler2.y);
        float diffz = Mathf.Abs(euler.z - lastEuler2.z);

        // Log the gaze direction

        if (diffx > 30 || diffy > 30 || diffz > 30)
        {
            ++lookAroundYouCount;
            lastEuler2 = euler;
        }
    }

    private bool LookAroundYou()
    {
        LookAroundYou1();
        LookAroundYou2();

#if UNITY_EDITOR
        float percent = (float)lookAroundYouCount / (float)LookAroundAmount * 100.0f;
        int percentInt = (int)percent;
        lookAroundYouText.GetComponent<Text>().text = "Look Around You: " + percentInt.ToString();
#endif

        if (lookAroundYouCount > LookAroundAmount)
        {
            return false; // end this state
        }
        //Debug.Log(lookAroundYouCount);
        return true;
    }

    void Update()
    {
        switch (experienceState)
        {
            // Resets the experience
            case ExperienceState.ResetExperience:
                Debug.Log("ExperienceState.Reset");
                Reset();
                SetExperienceState(ExperienceState.WaitingForHeadsetToBeWorn);
                break;
            
            case ExperienceState.WaitingForHeadsetToBeWorn:
                if (IsHeadsetWorn())
                {
                    SetExperienceState(ExperienceState.LookAround);
                }
                break;
            
            // User looking around
            case ExperienceState.LookAround:
                if (LookAroundYou() == false)
                {
                    SetExperienceState(ExperienceState.StartPlayingMovies);
                }
                break;

            // Reveals the Tiltbrush spheres
            case ExperienceState.StartPlayingMovies:
                Debug.Log("ExperienceState.StartPlayingMovies");
                lookAroundYou.SetActive(false);
                ShowInteractionTargets();
                SetExperienceState(ExperienceState.PlayingMovies);
                break;

            // Main Experience
            case ExperienceState.PlayingMovies:
                Debug.Log("ExperienceState.PlayingMovies");
                Update_WatchingMovies();

                if ( !IsHeadsetWorn())
                {
                    SetExperienceState(ExperienceState.ResetExperience);
                }
                break;
        }
    }
}
