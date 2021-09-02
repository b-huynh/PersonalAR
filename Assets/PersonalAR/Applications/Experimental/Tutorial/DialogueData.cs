using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea(6, 6)] public string text;
    public AudioClip audioClip;
}

[CreateAssetMenu(menuName = "DialogueData")]
public class DialogueData : ScriptableObject
{
    [HideInInspector]
    public List<Dialogue> Dialogues;
}
