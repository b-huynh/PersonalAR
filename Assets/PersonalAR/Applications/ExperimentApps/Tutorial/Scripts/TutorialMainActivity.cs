using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TutorialMainActivity : BaseAppActivity
{
    private List<TutorialItem> tutorialItems;
    private TutorialItem currentItem;

    [SerializeField] private TutorialVariables vars;
    [SerializeField] private AppState _app;

    // Start is called before the first frame update
    void Start()
    {
        ARDebug.logToUnityConsole = true;
        ARDebug.Log($"TutorialMainActivity Start");
        ARDebug.logToUnityConsole = false;
        // tutorialItems = TutorialItem.GetTutorialItems();
        // foreach(TutorialItem item in tutorialItems)
        // {
        //     item.OnTweenInComplete.AddListener(
        //         delegate
        //         {

        //         }
        //     );

        //     ARDebug.logToUnityConsole = true;
        //     ARDebug.Log($"[Tutorial Item {item.ItemOrder}] TweenIn Callbacks: {item.OnTweenInComplete.GetPersistentEventCount()}");
        //     ARDebug.logToUnityConsole = false;

        //     item.OnTweenOutComplete.AddListener(DoTutorialItemExit);
        // }
    }

    // Update is called once per frame
    void Update() {}

    public override void StartActivity(ExecutionContext executionContext)
    {
        tutorialItems = TutorialItem.GetTutorialItems();
        if (!vars.StartFlag)
        {
            tutorialItems.RemoveAt(0);
        }
        currentItem = tutorialItems[0];
        currentItem.Open();
        currentItem.OnTutorialEnter();
    }

    public override void StopActivity(ExecutionContext executionContext)
    {
        tutorialItems = TutorialItem.GetTutorialItems();
        foreach(var tutorialItem in tutorialItems)
        {
            tutorialItem.Close();
            tutorialItem.OnTutorialExit();
            currentItem = null;
        }
        vars.Save();
    }

    public void NextItem()
    {
        // Close current step
        currentItem.Close();
        currentItem.OnTutorialExit();
        tutorialItems.Remove(currentItem);

        // Start next step
        if (tutorialItems.Count > 0)
        {
            currentItem = tutorialItems[0];
            if (currentItem.BoolCloseCondition != null)
            {
                currentItem.BoolCloseCondition.OnValueChanged += CloseConditionHandler;
            }
            if (currentItem.IntCloseCondition != null)
            {
                currentItem.IntCloseCondition.OnValueChanged += IntCloseConditionHandler;
            }
            currentItem.Open();
            currentItem.OnTutorialEnter();
        } else
        {
            _app.StopTutorial();
        }
    }

    public void CloseConditionHandler(bool newValue)
    {
        if (newValue == true)
        {
            currentItem.BoolCloseCondition.OnValueChanged -= CloseConditionHandler;
            NextItem();
        }
    }

    public void IntCloseConditionHandler(int newValue)
    {
        if (newValue >= 12)
        {
            currentItem.closeEvent?.Invoke();
            currentItem.IntCloseCondition.OnValueChanged -= IntCloseConditionHandler;
            NextItem();
        }
    }

    // public void OnItemOpen()
    // {
    //     AudioSource audio = currentItem.gameObject.GetComponent<AudioSource>();
    //     audio.clip = currentItem.dialogue.audioClip;
    //     audio.Play();
    // }

    // public void OnItemClose()
    // {
    //     AudioSource audio = currentItem.gameObject.GetComponent<AudioSource>();
    //     audio.Stop();
    // }
}
