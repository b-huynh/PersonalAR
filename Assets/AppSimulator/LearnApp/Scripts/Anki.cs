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
public class Anki : MonoBehaviour
{
    public App owner = null;

    private bool showingAnswer = false; // If true, back side content is shown

    private Vector3 bigFocusIndicator = new Vector3(2f, 0.6f, 0.05f);
    private Vector3 smallFocusIndicator = new Vector3(0.6f, 0.6f, 0.05f);

    private ObjectRegistration objectRegistration;
    private string frontDialogue = "Question";
    private string backDialogue = "Answer";

    // Start is called before the first frame update
    void Start()
    {
        // SetDialogue(frontDialogue);
        // transform.Find("Buttons/ShowAnswer").gameObject.GetComponent<UIFocusable>().OnSelect += delegate {
        //     ShowAnswer();
        // };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(string anchorName, GameObject target)
    {
        frontDialogue = Translator.Translate(
            anchorName, GameManager.Instance.config.Experiment.TargetLanguage);
        backDialogue = anchorName;

        transform.Find("SplineToolTip").GetComponent<ToolTip>().ToolTipText = frontDialogue;
        transform.Find("SplineToolTip").GetComponent<ToolTipConnector>().Target = target;
    }

    public void SetContent(ObjectRegistration registration)
    {
        objectRegistration = registration;
        frontDialogue = Translator.Translate(
            registration.className, GameManager.Instance.config.Experiment.TargetLanguage);
        backDialogue = registration.className;
        
        transform.Find("SplineToolTip").GetComponent<ToolTip>().ToolTipText = frontDialogue;
        transform.Find("SplineToolTip").GetComponent<ToolTipConnector>().Target = registration.gameObject;

        // transform.Find("SplineToolTip/Pivot/ContentParent/Label").GetComponent<TextMeshPro>().text = frontDialogue;
    }

    public void ShowAnswer()
    {
        transform.Find("SplineToolTip/Pivot/ShowAnswerButton/IconAndText/TextMeshPro").GetComponent<TextMeshPro>().text = backDialogue;

        // // TODO: Change content
        // SetDialogue(backDialogue);

        // transform.Find("Buttons/FocusIndicator").transform.localScale = smallFocusIndicator;

        // transform.Find("Buttons/ShowAnswer").gameObject.SetActive(false);
        // List<GameObject> responses = new List<GameObject> {
        //     transform.Find("Buttons/Hard").gameObject,
        //     transform.Find("Buttons/Good").gameObject,
        //     transform.Find("Buttons/Easy").gameObject
        // };

        // foreach(GameObject response in responses)
        // {
        //     response.SetActive(true);
        //     response.GetComponent<UIFocusable>().OnSelect += delegate {
        //         string item = objectRegistration.name;
        //         string responseValue = response.GetComponent<TextMesh>().text;

        //         Debug.LogFormat("Item {0} Selected {1}", item, responseValue);
        //         StatsTracker.Instance.LogActivity(
        //             item, owner.appName, responseValue);

        //         objectRegistration.gameObject.GetComponentInChildren<ContextMenu>().RemoveApp(owner.appID);
        //         GetComponent<MenuElement>().CloseMenu();
        //     };
        // }
        // showingAnswer = true;
    }

}
