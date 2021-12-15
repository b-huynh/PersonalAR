using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using UnityEngine;

/*
 * Data to add: (how often do you want this information saved?)
 * Apps open/closed 
 * Time Spent per App 
 * Object Annotation Positions
 * Post-Task Mesh State (will come back to, Ask Ehsan)
 * Plackback software 
 * Add position of Apps
 */

public class DataCollector : MonoBehaviour
{
    public System.DateTime startTime;
    int studyTimer = 0;
    [SerializeField] private DataCollection _DataCollection = new DataCollection();

    public void SaveIntoJson()
    {
        string data = JsonUtility.ToJson(_DataCollection);
        string filename = "/DataCollection_Session_" + startTime.ToString("yyyy-MM-dd-tt--HH-mm-ss") + ".json";
        System.IO.File.WriteAllText(Application.persistentDataPath + filename, data);
    }

    // Use this for initialization
    void Start()
    {
        //create session_folder
        startTime = System.DateTime.Now;
         _DataCollection.session_info.UnixTime = Utils.UnixTimestampMilliseconds();
        studyTimer = 0;
        _DataCollection.session_info.SystemTime = startTime.ToString("HH-mm-ss");
        _DataCollection.session_info.Date = startTime.ToString("yyyy-dd-M");
        
    }

    // Update is called once per frame
    void Update()
    {
        EyeTracking current_data = new EyeTracking();
        current_data.Position = Camera.main.transform.position;
        current_data.Direction = Camera.main.transform.forward;
        current_data.Angle = Camera.main.transform.rotation.eulerAngles;

        current_data.SystemTime = System.DateTime.Now.ToString("HH-mm-ss-ff");
        current_data.UnixTime = Utils.UnixTimestampMilliseconds();
        _DataCollection.EyeTracking.Add(current_data);

        //record data every 20 ticks
        studyTimer += 1;
        if (studyTimer == 20)
        {
            studyTimer = 0;
            SaveIntoJson();
        }
    }

}

[System.Serializable]
public class DataCollection
{
    public Session session_info = new Session();
    public List<EyeTracking> EyeTracking = new List<EyeTracking>();
}

[System.Serializable]
public class EyeTracking
{
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Angle;
    public string SystemTime;
    public long UnixTime;
}

[System.Serializable]
public class Session
{
    public string Date;
    public string SystemTime;
    public long UnixTime;
    public int Participant_ID = 123;
}
