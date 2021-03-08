using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

using Recaug;

/*
  Manages Anki-style content for language learning app
  Front side card: Foreign language word
  Back side card: Foreign word AND translation. 3 buttons (confidene levels)
*/
[RequireComponent(typeof(Anchorable))]
[RequireComponent(typeof(AppEntity))]
public class LearnObjectMenu : MonoBehaviour
{
    private string frontDialogue = "Question";
    private string backDialogue = "Answer";
    private Translator.TargetLanguage targetLanguage;
    void OnEnable()
    {
        GetComponent<Anchorable>().OnAnchorSet += OnAnchorSet;
    }

    void OnDisable()
    {
        GetComponent<Anchorable>().OnAnchorSet -= OnAnchorSet;
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
        PervasiveLearnApp parentApp = (PervasiveLearnApp)GetComponent<AppEntity>().ParentApp;

        frontDialogue = Translator.Translate(anchorName, parentApp.language);
        backDialogue = anchorName;

        transform.position = target.transform.position;

        transform.Find("SplineToolTip").GetComponent<ToolTip>().ToolTipText = frontDialogue;
        transform.Find("SplineToolTip").GetComponent<ToolTipConnector>().Target = target;
    }

    public void ShowAnswer()
    {
        transform.Find("SplineToolTip/Pivot/ShowAnswerButton/IconAndText/TextMeshPro").GetComponent<TextMeshPro>().text = backDialogue;
    }

}
