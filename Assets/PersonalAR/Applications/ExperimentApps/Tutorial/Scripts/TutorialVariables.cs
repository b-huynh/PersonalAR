using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Applications/App Variables/Tutorial Variables")]
public class TutorialVariables : AppVariables
{
    public bool StartFlag;
    public bool HandMenuTriggered { get; set; }
    public bool AppTriggered;
    public bool EndFlag;
    public int CompletionCounter;
}
