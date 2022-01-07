using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Extensions;
using Microsoft.MixedReality.Toolkit.Utilities;

using Microsoft;

//when no hand in hand tracking, simplfy data
//when appEvent is null, simplfy data
//add anchors from AnchorService.cs
//positions of apps and anchors?

//list of all current anchors?
// or event when anchor placed, removed, moved?
public static class Const
{
    public const int NUM_TOTAL = 10;
    public const int FRAME_RATE = 60;
    public const float TICK_RATE = 10f;
}

[System.Serializable]
public class RecordStudy
{
    public StudyLog log;
    public StudyObject obj;
}

//Log for web app
[System.Serializable]
public class StudyLog
{
    public Vector3 headPos;
    public float angle;
    public long time;
    public bool gazeValid;
}

//Gaze and Head pos frame
[System.Serializable]
public struct StudyFrame
{
    public Vector3 hPos;
    public Vector3 hDir;
    public Quaternion hRot;
    public Vector3 hAngl;

    public Vector3 gazeOrigin;
    public Vector3 gazeDirection;
    public bool gazeValid;

    public long timestamp;
    public string systemTime;
    public long unixTime;

    public JointTracking hand;
    public List<string> openApps;
    public List<AppEvent> AppEvents;

}

[System.Serializable]
public struct JointTracking
{
    public Vector3 rWrist;
    public Vector3 rThumb;
    public Vector3 rIndex;
    public Vector3 rMiddle;
    public Vector3 rPinky;
    public Vector3 rRing;

    public Vector3 lWrist;
    public Vector3 lRing;
    public Vector3 lPinky;
    public Vector3 lMiddle;
    public Vector3 lIndex;
    public Vector3 lThumb;
}


//Session recording which is written to json
[System.Serializable]
public class SessionRecording
{
    public int sessionID;
    public long startTime;
    public long numFrames;
    public float tickRate = Const.TICK_RATE;
    public List<StudyFrame> frames;

    public SessionRecording(int id, float t = Const.TICK_RATE)
    {
        sessionID = id;
        tickRate = t;
        frames = new List<StudyFrame>();
    }
}

//Json entry for a single user
[System.Serializable]
public class StudyObject
{
    public string _valid = "null";
    public string userID;
    public float tickRate = Const.TICK_RATE;
    public SessionRecording sessionRecording = new SessionRecording(1);
}

public class SceneStudyManager : MonoBehaviour
{
    //General
    string userID = "1234";
    static System.Random random = new System.Random();
    //objects to write to json

    public StudyLog log;
    public StudyObject obj;
    public JointTracking hand;
    public List<string> openAppsList;

    float logTimer = 0f;
    int studyTimer = 0;
    public int currentSession = 0;
    public long frameNum = 0;
    public StudyFrame currentFrame;
    

    [SerializeField] private RecordStudy _RecordStudy = new RecordStudy();

    public long startTime;

    public string filename;

    //Logging objects
    public List<StudyFrame> newFrames;

    MixedRealityPose pose;

    #region Public methods

    //Generate random string
    public static string GenerateHexString(int digits)
    {
        return string.Concat(Enumerable.Range(0, digits).Select(_ => random.Next(16).ToString("X")));
    }

    //Initialize the user's data
    public void InitializeUserAllData()
    {
        newFrames = new List<StudyFrame>();
    }

    public void SaveIntoJson()
    {
        string data = JsonUtility.ToJson(_RecordStudy);
        System.IO.File.WriteAllText(Application.persistentDataPath + filename, data);
        //Debug.Log(Application.persistentDataPath + filename);
    }

    //Record data (called every 10 ticks)
    public void SaveStudy()
    {
        currentFrame.timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
        currentFrame.systemTime = System.DateTime.Now.ToString("HH-mm-ss-ff");
        currentFrame.unixTime = Utils.UnixTimestampMilliseconds();

        currentFrame.hPos = Camera.main.transform.position;
        currentFrame.hRot = Camera.main.transform.rotation;
        currentFrame.hDir = Camera.main.transform.forward;
        currentFrame.hAngl = Camera.main.transform.rotation.eulerAngles;

        currentFrame.gazeOrigin = CoreServices.InputSystem.EyeGazeProvider.GazeOrigin;
        currentFrame.gazeDirection = CoreServices.InputSystem.EyeGazeProvider.GazeDirection;
        currentFrame.gazeValid = CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingDataValid;
        hand = new JointTracking();
        getJoints();

        currentFrame.hand = hand;

        currentFrame.AppEvents = AppState.getAppEvents();

        List<string> apps = AppState.getOpenApps();
        openAppsList = new List<string>();

        for(int i = 0; i < apps.Count; i++){
            openAppsList.Add(apps[i]);
        }

        currentFrame.openApps = openAppsList;
        
        newFrames.Add(currentFrame);
    }

    //Write other saved data too (every second)
    public void LogStudy()
    {
        if (newFrames.Count > 0)
        {
            for (int i = 0; i < newFrames.Count - 1; i++)
            {
                obj.sessionRecording.frames.Add(newFrames[i]);
            }

            obj.sessionRecording.numFrames = obj.sessionRecording.frames.Count;
            _RecordStudy.obj = obj;

            newFrames = new List<StudyFrame>();
        }

        Vector3 angle = Camera.main.transform.rotation.eulerAngles;
        log.time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
        log.headPos = Camera.main.transform.position;
        log.angle = angle.y;
        log.gazeValid = currentFrame.gazeValid;
        _RecordStudy.log = log;

        SaveIntoJson();
    }

    public void getJoints()
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out pose))
        {
            hand.lThumb = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out pose))
        {
            hand.lIndex = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out pose))
        {
            hand.lMiddle = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Left, out pose))
        {
            hand.lRing = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Left, out pose))
        {
            hand.lPinky = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Wrist, Handedness.Left, out pose))
        {
            hand.lWrist = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out pose))
        {
            hand.rThumb = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out pose))
        {
            hand.rIndex = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out pose))
        {
            hand.rMiddle = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Right, out pose))
        {
            hand.rRing = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out pose))
        {
            hand.rPinky = pose.Position;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Wrist, Handedness.Right, out pose))
        {
            hand.rWrist = pose.Position;
        }
    }

    private async Task WaitOneSecondAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    #endregion

    #region Unity methods

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = Const.FRAME_RATE;
        startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        filename = "/RecordStudy_Session_" +  System.DateTime.Now.ToString("yyyy-MM-dd-tt--HH-mm-ss") + ".json";
        obj = new StudyObject();
        log = new StudyLog();

        obj.tickRate = Const.TICK_RATE;
        currentFrame = new StudyFrame();
        currentFrame.AppEvents = new List<AppEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
        //Log the data every second
        logTimer += Time.deltaTime;

        //Record data every 10 ticks
        studyTimer += 1;
        if (studyTimer == Const.TICK_RATE)
        {
            studyTimer = 0;
            SaveStudy();
        }

        //Log data every second
        if (logTimer > 1f)
        {
            logTimer = 0f;
            LogStudy();
        }
    }

    #endregion
}