using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppButtonBehavior : MonoBehaviour
{
    [SerializeField] private AppState _app;
    [SerializeField] private bool tutorial;
    [SerializeField] private TutorialVariables tutorialVariables;
    public ActivityType activityType;

    private System.Guid cachedActivityID = System.Guid.Empty;
    private ButtonConfigHelper buttonConfig;
    private void Awake()
    {
        ForceRefreshReferences();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ForceRefreshReferences()
    {
        if (_app != null)
        {
            buttonConfig = GetComponent<ButtonConfigHelper>();

            // Probably have to remove existing listeners?...
            buttonConfig.OnClick.RemoveListener(OnClickDelegate);
            buttonConfig.OnClick.AddListener(OnClickDelegate);

            GetComponent<AppStateListener>().SetAppState(_app);
            // GetComponent<AppStateListener>().appState = _app;

            // if (activeVisuals != null)
            // {
            //     // activeVisuals.GetComponent<AppStateListener>().appState = _app;
            //     activeVisuals.GetComponent<AppStateListener>().SetAppState(_app);
            // }
            tutorialVariables.Load();
            if (tutorialVariables.StartFlag)
            {
                cachedActivityID = System.Guid.NewGuid();
            }
        }
    }

    public void OnClickDelegate()
    {
        // Create execution context
        ExecutionContext ec = new ExecutionContext(gameObject);
        IAnchorable anchorable = GetComponentInParent<IAnchorable>();
        if (anchorable != null)
        {
            ec.Anchor = anchorable.Anchor;
        }

        // Toggle activity
        if (cachedActivityID == System.Guid.Empty)
        {
            // Save activity ID to toggle off later.
            cachedActivityID = _app.StartActivity(activityType, ec);
        }
        else
        {
            if (tutorial)
            {
                _app.StopTutorial();
                cachedActivityID = System.Guid.Empty;
            } else
            {
                _app.StopActivity(cachedActivityID, ec);
                cachedActivityID = System.Guid.Empty;
            }
        }
    }
}
