using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

using Recaug;

// public interface IInitializable<T>
// {
//     T initData { set; }
// }

[System.Serializable]
public struct FlashCardData
{
    public int numResponses;
    public string dueDateSerialized;
}


/*
  Manages Anki-style content for language learning app
  Front side card: Foreign language word
  Back side card: Foreign word AND translation. 3 buttons (confidene levels)
*/
[RequireComponent(typeof(Anchorable))]
[RequireComponent(typeof(AppStateListener))]
public class LearnObjectMenu : MonoBehaviour
{
    public FlashCardData data;

    public Translator.TargetLanguage targetLanguage;

    // Component and GameObject refs to manage display behaviour
    [SerializeField]
    private ToolTip toolTip;
    [SerializeField]
    private ToolTipConnector toolTipConnector;
    [SerializeField]
    private TextMeshPro textMesh;
    [SerializeField]
    private ScaleTween cardTween;
    [SerializeField]
    private GameObject reviewButtons;
    [SerializeField]
    private GameObject showAnswerButton;

    // Internal state variables
    private string _backDialogue;

    void OnEnable()
    {
        GetComponent<Anchorable>().OnAnchorSet.AddListener(OnAnchorSet);
    }

    void OnDisable()
    {
        GetComponent<Anchorable>().OnAnchorSet.RemoveListener(OnAnchorSet);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnAnchorSet(AnchorableObject anchor)
    {
        SetDisplayProperties(anchor.WorldAnchorName, anchor.gameObject);
    }

    private void SetDisplayProperties(string anchorName, GameObject target)
    {
        string frontDialogue = Translator.Translate(anchorName, targetLanguage);
        _backDialogue = anchorName;

        transform.position = target.transform.position;

        // toolTip.ToolTipText = frontDialogue;
        textMesh.text = frontDialogue;

        toolTipConnector.Target = target;
    }

    public void ShowAnswer()
    {
        textMesh.text = _backDialogue;
        showAnswerButton.SetActive(false);
        reviewButtons.SetActive(true);
    }

    public void OnAppRenderOn()
    {
        toolTip.gameObject.SetActive(true);
        cardTween.TweenIn();
    }

    public void OnAppRenderOff()
    {
        toolTip.gameObject.SetActive(false);
        cardTween.TweenOut();
    }

    public void HandleEasyResponse()
    {
        cardTween.TweenOut();
    }

    public void HandleReviewResponse()
    {
        cardTween.TweenOut();
    }

    public void HandleHardResponse()
    {
        cardTween.TweenOut();
    }
}
