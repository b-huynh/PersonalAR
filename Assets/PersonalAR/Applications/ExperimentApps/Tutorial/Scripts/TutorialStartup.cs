using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStartup : MonoBehaviour
{
    [SerializeField] private TutorialVariables vars;
    [SerializeField] private AppState _app;
    public ActivityType activityType;

    private System.Guid cachedActivityID = System.Guid.Empty;

    // Start is called before the first frame update
    void Start()
    {
        vars.Load();
        if (vars.StartFlag)
        {
            // Create execution context
            ExecutionContext ec = new ExecutionContext(gameObject);
            IAnchorable anchorable = GetComponentInParent<IAnchorable>();
            if (anchorable != null)
            {
                ec.Anchor = anchorable.Anchor;
            }
            /*
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
            }*/
            _app.StartActivity(activityType, ec);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
