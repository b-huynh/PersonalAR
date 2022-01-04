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

public static class Const
{
    public const int NUM_TOTAL = 10;
    public const int FRAME_RATE = 60;
    public const float TICK_RATE = 6f;
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
}

//Session recording which is written to json
[System.Serializable]
public class SessionRecording
{
    public int sessionID;
    public long startTime;
    public long length;
    public long numFrames;
    public float tickRate;
    public List<StudyFrame> frames;

    public SessionRecording(int id, float t)
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
    public float tickRate;
    public SessionRecording sessionRecording;

    // public StudyObject(string path, float tickRate = Const.TICK_RATE)
    // {
    //     this.tickRate = tickRate;
    //     sessionRecording = new SessionRecording[Const.NUM_TOTAL];
    //     for (int i = 0; i < Const.NUM_TOTAL; i++)
    //     {
    //         sessionRecording[i] = new SessionRecording(i, this.tickRate);
    //     }
    // }

}

//Recording status log frame
[System.Serializable]
public struct RecordingFrame
{
    public long frameIndex;
    public long timeStamp;
    public bool status;

    public RecordingFrame(bool s, long t, long f)
    {
        status = s;
        timeStamp = t;
        frameIndex = f;
    }
}

public class SceneStudyManager : MonoBehaviour
{
    //General
    string userID = "1234";
    static System.Random random = new System.Random();
    //objects to write to json

    public StudyLog log;
    public StudyObject obj;

    float logTimer = 0f;
    int studyTimer = 0;
    public int currentSession = 0;
    public StudyFrame currentFrame;

    [SerializeField] private RecordStudy _RecordStudy = new RecordStudy();

    public long startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

    public string filename = "/RecordStudy_Session_" +  System.DateTime.Now.ToString("yyyy-MM-dd-tt--HH-mm-ss") + ".json";

    //Logging counters
    long frameNum = 0;

    //Logging objects
    public List<StudyFrame> newFrames;
    public List<RecordingFrame> newRec;

    #region Public methods

    //Generate random string
    public static string GenerateHexString(int digits)
    {
        return string.Concat(Enumerable.Range(0, digits).Select(_ => random.Next(16).ToString("X")));
    }


    //Initialize the user's data
    public void InitializeUserAllData()
    {
       //initalize local frame
        newFrames = new List<StudyFrame>();
        newRec = new List<RecordingFrame>();
    }

    public void SaveIntoJson()
    {
        string data = JsonUtility.ToJson(_RecordStudy);
        ARDebug.Log(Application.persistentDataPath + filename);
        System.IO.File.WriteAllText(Application.persistentDataPath + filename, data);
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

        obj.sessionRecording.frames.Add(currentFrame);
        
        newFrames.Add(currentFrame);

        _RecordStudy.obj = obj;

        SaveIntoJson();
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

            SaveIntoJson();

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

    private async Task WaitOneSecondAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    #endregion

    #region Unity methods

    // Start is called before the first frame update
    void Start()
    {
        ARDebug.Log("START");
        Application.targetFrameRate = Const.FRAME_RATE;
    }

    // Update is called once per frame
    void Update()
    {
        ARDebug.Log("UPDATE");
        long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
        //Log the data every second
        logTimer += Time.deltaTime;

        //Record data every 10 ticks
        studyTimer += 1;
        if (studyTimer == 5)
        {
            ARDebug.Log("SAVESTUDY");
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