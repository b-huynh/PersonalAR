using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Recaug;
using Recaug.Client;

public class PervasiveLearnApp : BasePervasiveApp
{
    [Header("Learn App Settings")]
    // Learning Activity Prefab
    public GameObject objectTagPrefab;

    // Learning State
    private List<string> knownWords = new List<string> {
        "chair",
        "tv",
        "keyboard",
        "mouse",
        "sofa",
        "dining table",
        "microwave",

        "zebra",
        "giraffe",
        "bottle",

        "car",
        "chair",
        "clock",

        "cup",
        "book",
        "apple",

        "scissors",
        "airplane",
        "donut",
    };
    private Dictionary<string, GameObject> toLearn;
    private List<string> learned;

    // UI State
    private bool awaitingResponse = false;

    private static bool releaseFocusAfterQuiz = false;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        
        toLearn = new Dictionary<string, GameObject>();
        learned = new List<string>();

        // Context Events
        ObjectRegistry.Instance.OnRegistered += OnObjectRegistered;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnObjectRegistered(ObjectRegistration registration)
    {
        // Do not care about words we don't know.
        // if (!knownWords.Contains(registration.className))
        // {
        //     return;
        // }

        // Register available content
        // registration.gameObject.GetComponentInChildren<ContextMenu>().AddApp(appID);

        CreateAnkiContent(registration);
    }

    void CreateAnkiContent(ObjectRegistration registration)
    {
        GameObject menu = InstantiateInApp(objectTagPrefab);

        menu.transform.position = registration.position;

        menu.GetComponent<Anki>().SetContent(registration);
        // menu.GetComponent<Anki>().owner = this;
    }

    // void CreateQuizContent(ObjectRegistration registration)
    // {
    //     // Exit if already learned
    //     if (learned.Contains(registration.className))
    //     {
    //         return;
    //     }

    //     // Create object tag
    //     var objectTag = GameObject.Instantiate(objectTagPrefab,
    //         registration.position, transform.rotation);

    //     // Create the Quiz
    //     var quiz = objectTag.GetComponent<MultipleChoiceQuiz>();
        
    //     // Instantiate some variables that shouldn't be there...
    //     quiz.registration = registration;
    //     quiz.parentApp = this;

    //     // Set quiz options and correct answer
    //     int correctResponse = Random.Range(0, 4);
    //     var junk = new List<string> { "giraffe", "motorcycle", "airplane", "zebra" };
    //     for(int i = 0; i < 4; ++i)
    //     {
    //         if (i == correctResponse)
    //         {
    //             quiz.SetOption(i, registration.className,
    //                 GameManager.Instance.config.Experiment.TargetLanguage);
    //         }
    //         else
    //         {
    //             quiz.SetOption(i, junk[i], GameManager.Instance.config.Experiment.TargetLanguage);
    //         }
    //     }
    //     quiz.SetAnswer(correctResponse);

    //     // Set response callback
    //     quiz.OnCorrectResponse += delegate {
    //         // Remove Learning Event
    //         toLearn.Remove(registration.className);

    //         // Update learned words
    //         learned.Add(registration.className);

    //         // Unlink object
    //         // TODO: Think of smarter way to do this...
    //         Unlink(objectTag);

    //         // Destroy Quiz GameObject
    //         GameObject.Destroy(objectTag);

    //         // Reset response state
    //         awaitingResponse = false;

    //         // Remove self from object's available content list.
    //         registration.gameObject.GetComponentInChildren<ContextMenu>().RemoveApp(appID);

    //         // Release App Focus
    //         // GameManager.Instance.SwitchApp(1);
    //         if (releaseFocusAfterQuiz)
    //         {
    //             GameManager.Instance.ReleaseAppFocus();
    //         }
    //     };

    //     Link(objectTag);

    //     // Update known objects cache
    //     toLearn[registration.className] = objectTag;
    // }

    // private bool InView(GameObject g)
    // {
    //     Vector3 vp = Camera.main.WorldToViewportPoint(g.transform.position);
    //     if (vp.x >= 0.0f && vp.x <= 1.0f && 
    //         vp.y >= 0.0f && vp.y <= 1.0f && 
    //         vp.z > 0.0f)
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }
}
