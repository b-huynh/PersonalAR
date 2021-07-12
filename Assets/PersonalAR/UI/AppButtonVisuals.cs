using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Microsoft.MixedReality.Toolkit.UI;

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
            ForceRefresh();
        }
    }
    public ActivityType activityType;
    public GameObject activeVisuals;

    private System.Guid cachedActivityID = System.Guid.Empty;
    private ButtonConfigHelper buttonConfig;

    void OnValidate()
    {
        ForceRefresh();
    }

    // Start is called before the first frame update
    void Start()
    {
        ForceRefresh();

        // Set Active Visuals
        // activeVisuals.GetComponent<AppStateListener>()
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ForceRefresh()
    {
        if (_app != null)
        {
            buttonConfig = GetComponent<ButtonConfigHelper>();
            buttonConfig.MainLabelText = _app.appName;

            // Probably have to remove existing listeners?...
            buttonConfig.OnClick.RemoveListener(OnClickDelegate);
            buttonConfig.OnClick.AddListener(OnClickDelegate);


            buttonConfig.SetQuadIcon(_app.appLogo.mainTexture);
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
            _app.StopActivity(cachedActivityID, ec);
            cachedActivityID = System.Guid.Empty;
        }
    }
}
