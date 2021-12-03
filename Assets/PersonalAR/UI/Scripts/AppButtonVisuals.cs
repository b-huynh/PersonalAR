using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

using Microsoft.MixedReality.Toolkit.UI;

[RequireComponent(typeof(AppStateListener))]
[RequireComponent(typeof(ButtonConfigHelper))]
public class AppButtonVisuals : MonoBehaviour
{
    [SerializeField] private AppState _app;
    public AppState App
    {
        get => _app;
        set
        {
            _app = value;
            ForceRefreshVisuals();
        }
    }
    public ActivityType activityType;

    [Header("Execution State Visuals")]
    // public GameObject activeVisuals;
    public GameObject FullActiveVisuals;
    public GameObject SemiActiveVisuals;
    public TextMeshPro SemiActiveCount;

    // Private State Variables
    private ButtonConfigHelper buttonConfig;

    void OnValidate()
    {
        ForceRefreshVisuals();
    }

    void Awake()
    {

        var appListener = GetComponent<AppStateListener>();
        appListener.OnExecutionStopped.AddListener(OnExecutionStoppedDelegate);
        appListener.OnExecutionSuspended.AddListener(OnExecutionSuspendedDelegate);
        appListener.OnExecutionRunningFull.AddListener(OnExecutionRunningFullDelegate);
        appListener.OnExecutionRunningPartial.AddListener(OnExecutionRunningPartialDelegate);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ForceRefreshVisuals()
    {
        if (_app != null)
        {
            buttonConfig = GetComponent<ButtonConfigHelper>();
            buttonConfig.MainLabelText = _app.appName;
            buttonConfig.SetQuadIcon(_app.appLogo.mainTexture);
        }
    }

    public void OnExecutionStoppedDelegate()
    {
        FullActiveVisuals.SetActive(false);
        SemiActiveVisuals.SetActive(false);
        SemiActiveCount.gameObject.SetActive(false);
        SemiActiveCount.text = App.NumActivities.ToString();
    }

    public void OnExecutionSuspendedDelegate()
    {
        // Do nothing for now.
    }

    public void OnExecutionRunningFullDelegate()
    {
        FullActiveVisuals.SetActive(true);
        SemiActiveVisuals.SetActive(false);
        SemiActiveCount.gameObject.SetActive(false);
    }

    public void OnExecutionRunningPartialDelegate()
    {
        FullActiveVisuals.SetActive(false);
        SemiActiveVisuals.SetActive(true);
        SemiActiveCount.gameObject.SetActive(true);
        SemiActiveCount.text = App.NumActivities.ToString();
    }
}
