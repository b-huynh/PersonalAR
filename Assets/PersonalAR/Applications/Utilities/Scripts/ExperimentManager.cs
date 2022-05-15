using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ExperimentConditions
{
    Layerable = 1,
    Immersive = 2
}

public class ExperimentManager : Singleton<ExperimentManager>
{
    public SceneStudyManager sceneStudyManager;

    public RandomPinCodes codeSet;

    public BoolVariable immersiveModeState;

    public string UserID => sceneStudyManager.obj.userID;

    public string SessionType
    {
        get
        {
            // return immersiveModeState.GetValue() == true ? "immersive" : "layerable";
            return immersiveModeState.GetValue() == true ? "Mode B" : "Mode A";

        }
    }

    public int SecondsAllotted;

    [ReadOnly] public int Score;

    [ReadOnly] public float TotalSecondsLeft;

    public int MinutesLeft
    {
        get { return (int)(TotalSecondsLeft / 60.0f); }
    }

    public int SecondsLeft
    {
        get { return (int)(TotalSecondsLeft % 60.0f); }
    }

    public string Status
    {
        get
        {
            return string.Format(
@"<b>Participant ID:</b>
<color=#FFFFFF>{0}</color>
<b>Task Session:</b>
<color=#FFFFFF>{1}</color>
<b>Score:</b>
<color=#FFFFFF>{2}</color>
<b>Time Left:</b>
<color=#FFFFFF>{3:D2}:{4:D2}</color>",
            UserID, SessionType, Score, MinutesLeft, SecondsLeft);
        }
    }

    public UnityEvent OnTaskStart;
    public UnityEvent OnTaskComplete;

    private static ExperimentEventData currentEventData;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TotalSecondsLeft > 0)
        {
            TotalSecondsLeft -= Time.deltaTime;
            if (TotalSecondsLeft <= 0)
            {
                // Set event data
                currentEventData = new ExperimentEventData
                {
                    unixTime = Utils.UnixTimestampMilliseconds(),
                    systemTime = System.DateTime.Now.ToString("HH-mm-ss-ff"),
                    userID = this.UserID,
                    sessionType = this.SessionType,
                    eventName = "EndTask",
                    pinRandomSeed = codeSet.randomSeed,
                    pinNumCodes = codeSet.numCodes,
                    pinCodeLength = codeSet.codeLength,
                    pinNumPieces = codeSet.numPieces,
                    score = this.Score
                };

                // Invoke callbacks
                codeSet.OnCodeEntryComplete.RemoveListener(this.UpdateScore);
                OnTaskComplete.Invoke();
            }
        }
    }

    public void StartTask()
    {
        Score = 0;
        TotalSecondsLeft = SecondsAllotted;
        //UserID = SceneStudyManager.getUserID();

        // codeSet.OnCodeEntryComplete.RemoveAllListeners();
        codeSet.OnCodeEntryComplete.AddListener(this.UpdateScore);
        // codeSet.Reset();
        // codeSet.Generate();
    
        // Set event data
        currentEventData = new ExperimentEventData
        {
            unixTime = Utils.UnixTimestampMilliseconds(),
            systemTime = System.DateTime.Now.ToString("HH-mm-ss-ff"),
            userID = this.UserID,
            sessionType = this.SessionType,
            eventName = "StartTask",
            pinRandomSeed = codeSet.randomSeed,
            pinNumCodes = codeSet.numCodes,
            pinCodeLength = codeSet.codeLength,
            pinNumPieces = codeSet.numPieces,
            pinCodes = codeSet.Codes,
            score = 0
        };

        // Invoke callbacks
        OnTaskStart.Invoke();
    }

    public void UpdateScore(CodeEntryCompleteEventArgs eventArgs)
    {
        Score += 1;
    }

    public static ExperimentEventData GetExperimentEventData()
    {
        ExperimentEventData eventDataCopy = currentEventData;
        currentEventData = null;
        return eventDataCopy;
    }
}

[System.Serializable]
public class ExperimentEventData
{
    public long unixTime;
    public string systemTime;
    public string userID;
    public string sessionType;
    public string eventName;

    // Experiment RandomPinCodes params
    public int pinRandomSeed;
    public int pinNumCodes;
    public int pinCodeLength;
    public int pinNumPieces;
    public List<Code> pinCodes; 

    // Optional
    public int score;
}
