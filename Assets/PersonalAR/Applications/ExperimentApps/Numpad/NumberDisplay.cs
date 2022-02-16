using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class NumberDisplay : MonoBehaviour
{
    // Display properties
    public int maxLength;

    // Assign in editor
    [SerializeField] private TextMeshPro textMesh;

    // Assign at runtime
    private System.Guid activityID;
    [ReadOnly] public string activityIDDebug;

    // Initialize in scripts on Instantiate()
    public RandomPinCodes pinCodes { get; set; }

    public static CodeEvent codeEntered = new CodeEvent();

    void OnEnable()
    {
        Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        textMesh.color = Color.white;
        activityID = GetComponentInParent<BaseAppActivity>().activityID;
        activityIDDebug = activityID.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enter(string str)
    {
        if (textMesh.text.Length < maxLength)
        {
            textMesh.text += str;
        }
        textMesh.color = Color.white;
    }

    public void Delete()
    {
        textMesh.text = textMesh.text.Remove(textMesh.text.Length - 1);
        textMesh.color = Color.white;
    }

    public void Clear()
    {
        textMesh.text = string.Empty;
        textMesh.color = Color.white;
    }

    public void Validate()
    {
        int entered = System.Int32.Parse(textMesh.text); 
        
        codeEntered.unixTime = Utils.UnixTimestampMilliseconds();
        codeEntered.systemTime = System.DateTime.Now.ToString("HH-mm-ss-ff");
        codeEntered.codeEntered = entered;

        if (pinCodes.Contains(entered))
        {
            textMesh.color = Color.green;
            textMesh.text = "SUCCESS";
            codeEntered.success = true;
        }
        else
        {
            textMesh.color = Color.red;
            textMesh.text = "INVALID";
            codeEntered.success = false;
        }
    }

    public void CloseActivity()
    {
        var ec = new ExecutionContext(this.gameObject);
        GetComponentInParent<BaseAppActivity>().appState.StopActivity(activityID, ec);   
    }

    public static CodeEvent GetCodeEvent(){
        CodeEvent returnVal = codeEntered;
        codeEntered = new CodeEvent();
        return returnVal;
    }

}

[System.Serializable]
public class CodeEvent
{
    public long unixTime;
    public string systemTime;
    public long eventTime;
    public int codeEntered;
    public bool success;
}
