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


public static class Const
{
    public const int FRAME_RATE = 60;
    public const float TICK_RATE = 6f;
}

[System.Serializable]
public class RecordStudy
{
    public StudyObject obj;

}

//Gaze and Head pos frame
[System.Serializable]
public struct StudyFrame
{
    public long frameNum;
    public long timestamp;
    public string systemTime;
    public long unixTime;

    public bool gazeValid;

    public Vector3 hPos;
    public Vector3 hDir;
    public Quaternion hRot;
    public Vector3 hAngl;

    public Vector3 gazeOrigin;
    public Vector3 gazeDirection;

    public JointTracking hand;
    public Vector3 rightHandRay;
    public Vector3 leftHandRay;
    public List<string> openApps;
    public List<AppEvent> appEvents;
    public List<string> placedObjects;
    public List<AnchorEvent> anchorEvents;
    public List<CodeEvent> codesEvents;

    public List<GestureEvent> gestureEvents;

    public bool IsLayerableMode;

    public List<ExperimentEventData> experimentEvents;

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

public struct HandRay
{
    public Vector3 startPoint;
    public Vector3 endPoint;
    public bool hitObject;
    public string hitObjectName;
}

//Session recording which is written to json
[System.Serializable]
public class SessionRecording
{
    public long numFrames;

    public long recordNumber;
    public float tickRate = Const.TICK_RATE;
    public List<StudyFrame> frames;

    public SessionRecording(float t = Const.TICK_RATE)
    {
        tickRate = t;
        frames = new List<StudyFrame>();
    }
}

//Json entry for a single user
[System.Serializable]
public class StudyObject
{

    public string _valid = "null";
    public float tickRate = Const.TICK_RATE;
    public int sessionID = 1;
    public long startTime;
    public string userID = "abc";
    public SessionRecording sessionRecording = new SessionRecording(1);
}

public class SceneStudyManager : MonoBehaviour
{

    //objects to write to json
    public StudyObject obj;
    public JointTracking hand;

    public List<string> openAppsList;
    public List<string> placedObjectsList;


    float logTimer = 0f;
    float studyTimer = 0f;
    public StudyFrame currentFrame;
    
    [SerializeField] private RecordStudy _RecordStudy = new RecordStudy();

    public long startTime;
    public string filename;

    public int recordNumber = 0;

    MixedRealityPose pose;

    #region Public methods

    public void SaveIntoJson()
    {
        filename = "/RecordStudy_Session_" + recordNumber + "_" + startTime + ".json";
        if(recordNumber == 0){
            string data = JsonUtility.ToJson(_RecordStudy);
            System.IO.File.WriteAllText(Application.persistentDataPath + filename, data);
        }else{
            string data = JsonUtility.ToJson(_RecordStudy.obj.sessionRecording);
            System.IO.File.WriteAllText(Application.persistentDataPath + filename, data);
        }
    }

    //Record data (called every 10 ticks)
    public void SaveStudy()
    {
        currentFrame.frameNum++;
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

        currentFrame.rightHandRay = getRightHandRay();
        currentFrame.leftHandRay = getLeftHandRay();
        
        currentFrame.codesEvents = NumberDisplay.getCodeEvents();
        currentFrame.codesEvents.ForEach(delegate(CodeEvent e){
            e.eventTime = e.unixTime - startTime;
        });

        currentFrame.gestureEvents = GestureListener.getGestureEvents();
        currentFrame.gestureEvents.ForEach(delegate(GestureEvent gesture){
            //ARDebug.Log(gesture.eventType + " " + gesture.action + " pos:" + gesture.position);
            gesture.eventTime = gesture.unixTime - startTime;
        });

        currentFrame.appEvents = AppState.getAppEvents();
        currentFrame.appEvents.ForEach(delegate(AppEvent app){
            app.eventTime = app.unixTime - startTime;
            //ARDebug.Log("DATACOLLECTION - activity: " + app.activity + " activityID: " + app.activityID + " name: " + app.name + " activity type: " + app.activityType + " "+ app.systemTime);
            //Debug.Log("DATACOLLECTION - activity: " + app.activity + " activityID: " + app.activityID + " name: " + app.name + " activity type: " + app.activityType + " "+ app.systemTime);
        });

        currentFrame.anchorEvents = AnchorService.getAnchorEvent();
        currentFrame.anchorEvents.ForEach(delegate(AnchorEvent anchor){
            anchor.placedTime = anchor.unixTime - startTime;
            //ARDebug.Log("Anchor Event: " + anchor.objectName + " " + anchor.activity + " at " + anchor.placedTime);
            //Debug.Log("Anchor Event: " + anchor.objectName + " " + anchor.activity + " at " + anchor.placedTime);
        });

        List<string> apps = AppState.getOpenApps();
        List<string> objs = AnchorService.getPlacedObjects();
        openAppsList = new List<string>(apps);
        placedObjectsList = new List<string>(objs);

        ExperimentEventData EED = ExperimentManager.GetExperimentEventData();
        if (EED != null)
        {
            currentFrame.experimentEvents = new List<ExperimentEventData>();
            currentFrame.experimentEvents.Add(EED);

        }
        else
        {
            currentFrame.experimentEvents = null;
        }

        currentFrame.openApps = openAppsList;
        currentFrame.placedObjects = placedObjectsList;
        currentFrame.IsLayerableMode = ImmersiveModeController.IsLayerableMode;
        
        obj.sessionRecording.frames.Add(currentFrame);
    }

    //Write other saved data too (every second)
    public void LogStudy()
    {
        obj.sessionRecording.numFrames = obj.sessionRecording.frames.Count;
        obj.sessionRecording.recordNumber = recordNumber;
        _RecordStudy.obj = obj;
        SaveIntoJson();

        if(obj.sessionRecording.frames.Count >= Const.TICK_RATE * 30){
            recordNumber++;
            obj.sessionRecording.frames.Clear();
        }
   
    }

    public Vector3 getRightHandRay()
    {
        Vector3 rightEndPoint;
        if (PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out rightEndPoint))
        {
            return rightEndPoint;
        }

        return new Vector3(0,0,0);
    }

      public Vector3 getLeftHandRay()
    {
        Vector3 leftEndPoint;
        if (PointerUtils.TryGetHandRayEndPoint(Handedness.Left, out leftEndPoint))
        {
            return leftEndPoint;
        }

        return new Vector3(0,0,0);
    }

    public void getJoints()
    {

        int i = 0;
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

    #endregion

    #region Unity methods

    GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject;
        Application.targetFrameRate = Const.FRAME_RATE;
        startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        obj = new StudyObject();

        obj.tickRate = Const.TICK_RATE;
        obj.startTime = startTime;
        currentFrame = new StudyFrame();
        currentFrame.appEvents = new List<AppEvent>();
        currentFrame.gestureEvents = new List<GestureEvent>();
        currentFrame.codesEvents = new List<CodeEvent>();
        currentFrame.frameNum = 0;

    }

    // Update is called once per frame
    void Update()
    {
        long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
        //Log the data every second
        logTimer += Time.deltaTime;

        //Record data every 10 ticks
        studyTimer += 1f;
        if (studyTimer == Const.TICK_RATE)
        {
            studyTimer = 0f;
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