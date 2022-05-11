using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class NumberDisplay : MonoBehaviour
{
    // Display properties
    public int maxLength;
    private string clearString = "__-__-__";


    // Assign in editor
    [SerializeField] private TextMeshPro textMesh;

    // Assign at runtime
    private System.Guid activityID;
    [ReadOnly] public string activityIDDebug;

    public BoolVariable codeEntered;

    // Initialize in scripts on Instantiate()
    public RandomPinCodes pinCodes { get; set; }

    public static List<CodeEvent> codes = new List<CodeEvent>();

    void OnEnable()
    {
        // Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        textMesh.color = Color.white;
        activityID = GetComponentInParent<BaseAppActivity>().activityID;
        activityIDDebug = activityID.ToString();

        Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enter(string str)
    {
        int blankIndex = textMesh.text.IndexOf('_');
        if (blankIndex == -1)
        {
            return;
        }

        textMesh.text = textMesh.text.Insert(blankIndex, str).Remove(blankIndex + 1, str.Length);

        // if (textMesh.text.Length < maxLength)
        // {
        //     textMesh.text += str;
        // }
        // textMesh.color = Color.white;
        // 
        // for(int i = 0; i < textMesh.text.Length; ++i)
        // {
        //     if (textMesh.text[i] == '_')
        //     {
        //         textMesh.text[i] = str[0];
        //         return;
        //     }
        // }
    }

    public void Delete()
    {
        textMesh.text = textMesh.text.Remove(textMesh.text.Length - 1);
        textMesh.color = Color.white;
    }

    public void Clear()
    {
        textMesh.text = string.Empty;
        textMesh.text = clearString;
        textMesh.color = Color.white;
    }

    public void Validate()
    {
        string cleanedInput = textMesh.text.Replace("-", "").Replace("_", "");
        Debug.Log($"Cleaned Input: {cleanedInput}");
        int entered = System.Int32.Parse(cleanedInput);

        CodeEvent code = new CodeEvent();
        
        code.unixTime = Utils.UnixTimestampMilliseconds();
        code.systemTime = System.DateTime.Now.ToString("HH-mm-ss-ff");
        code.codes = entered;

        Debug.Log($"Parsed input: {entered}");
        if (pinCodes.Contains(entered))
        {
            pinCodes.MarkCodeEntryComplete(entered);

            textMesh.color = Color.green;
            textMesh.text = "SUCCESS";
            code.success = true;
            codeEntered.SetValue(true);
        }
        else
        {
            textMesh.color = Color.red;
            textMesh.text = "INVALID";
            code.success = false;
        }

        codes.Add(code);
    }

    public void CloseActivity()
    {
        var ec = new ExecutionContext(this.gameObject);
        GetComponentInParent<BaseAppActivity>().appState.StopActivity(activityID, ec);   
    }
    public static List<CodeEvent> getCodeEvents(){
        List<CodeEvent> returnVal = codes;
        codes = new List<CodeEvent>();
        return returnVal;
    }
}

[System.Serializable]
public class CodeEvent
{
    public long unixTime;
    public string systemTime;
    public long eventTime;
    public int codes;
    public bool success;
}