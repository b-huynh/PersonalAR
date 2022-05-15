using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

[RequireComponent(typeof(AudioSource))]
public class TutorialItem : AnimatedMenu, ITutorialItem
{
    public enum ContinueType { Button, ButtonConditional, Hand, None }

    [Header("Visual Components")]
    [SerializeField] private TextMeshPro bodyTextMesh;
    [SerializeField] private TextMeshPro stepTextMesh;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject handSymbol;
    public ContinueType continueType;
    public UnityEvent closeEvent;

    [Header("Display Location")]
    [SerializeField] Transform displayParent;

    [Header("Tutorial")]
    [SerializeField] private DialogueData tutorialData;
    public Dialogue dialogue { get; private set; }

    // *** ITutorialItem Implementation ***
    [SerializeField] private int m_itemOrder;
    public int ItemOrder
    {
        get => m_itemOrder;
    }
    // *** End ITutorialItem ***
    public BoolVariable BoolCloseCondition;
    public IntVariable IntCloseCondition;

    public static List<TutorialItem> GetTutorialItems()
    {
        return Resources.FindObjectsOfTypeAll<TutorialItem>()
            .Where(item => item.gameObject.scene.IsValid())
            .OrderBy(item => item.ItemOrder)
            .ToList();
    }

    public void OnValidate()
    {
        if (tutorialData == null) { return; }
        
        if (tutorialData.Dialogues.Count <= m_itemOrder)
        {
            Debug.Log($"Invalid Tutorial Data {gameObject.GetInstanceID()} {gameObject.GetFullName()} "); 
        }
        else
        {
            dialogue = tutorialData.Dialogues[m_itemOrder];
            GetComponent<AudioSource>().clip = dialogue.audioClip;
        }

        if (bodyTextMesh != null)
        {
            bodyTextMesh.text = dialogue.text;
        }

        if (stepTextMesh != null)
        {
            int totalItems = GetTutorialItems().Count - 1;
            // string stepStr = $"{(m_itemOrder+1).ToString()} / {totalItems.ToString()}";
            string stepStr = $"{(m_itemOrder).ToString()} / {totalItems.ToString()}";
            stepTextMesh.text = stepStr;
        }

        continueButton?.SetActive(false);
        handSymbol?.SetActive(false);
        if (BoolCloseCondition != null && IntCloseCondition != null)
        {
            Debug.Log("Invalid close condition, cannot have both bool and int close condition");
            BoolCloseCondition = null;
            IntCloseCondition = null;
        }
        if (continueType == ContinueType.Button) { continueButton?.SetActive(true); }
        else if (continueType == ContinueType.ButtonConditional)
        {
            continueButton.SetActive(true);
            continueButton.GetComponent<ButtonStatusController>().SetStatus(false);
        }
        else if (continueType == ContinueType.Hand) { handSymbol?.SetActive(true); }
    }

    protected override void Start()
    {
        base.Start();



        // OnTweenInComplete.AddListener(OnTutorialEnter);
        // OnTweenOutComplete.AddListener(OnTutorialExit);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Open()
    {
        if (displayParent == null)
        { 
            ARDebug.LogWarning($"{gameObject.name} has no displayParent");    
        }
        else
        { 
            this.transform.SetPositionAndRotation(displayParent.transform.position, displayParent.parent.rotation);
            // this.transform.parent = displayParent;
            this.transform.parent = displayParent;
            this.transform.localPosition = Vector3.zero;
        }

        base.Open();
    }

    public void OnTutorialEnter()
    {
        ARDebug.logToUnityConsole = true;
        ARDebug.Log($"[Tutorial Item {ItemOrder}]");

        if (dialogue == null)
        {
            ARDebug.Log("Dialogue is NULL");
        }
        else if (dialogue.audioClip == null)
        {
            ARDebug.Log("AudioClip is NULL");
        }
        else
        {
            ARDebug.Log($"AudioClip: {dialogue.audioClip.name} {dialogue.audioClip.GetInstanceID()}");
        }

        if (tutorialData == null)
        {
            ARDebug.Log("TutorialData is NULL");
        }
        else
        {
            ARDebug.Log($"TutorialData: {tutorialData.name} {tutorialData.GetInstanceID()}");
        }

        // ARDebug.Log($"AudioClip: {dialogue.audioClip.name} {dialogue.audioClip.GetInstanceID()}");
        // ARDebug.Log($"Text: {dialogue.text}");

        AudioSource audio = GetComponent<AudioSource>();
        // audio.clip = item.dialogue.audioClip;

        ARDebug.Log("AudioSource OK");

        if (audio.clip == null)
        {
            ARDebug.Log("AudioSource clip is NULL");
        }
        else
        {
            ARDebug.Log($"AudioSource Clip: {audio.clip.name} {audio.clip.GetInstanceID()}");
        }

        ARDebug.logToUnityConsole = false;
        audio.Play();
    }

    public void OnTutorialExit()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Stop();
    }
}
