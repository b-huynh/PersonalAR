using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStartup : MonoBehaviour
{
    [SerializeField] private AppState _app;
    public ActivityType activityType;

    private System.Guid cachedActivityID = System.Guid.Empty;

    // Start is called before the first frame update
    void Start()
    {
        // vars.Load();
        TutorialVariables tutorialVariables = (TutorialVariables)_app.Variables;
        if (tutorialVariables.StartFlag)
        {
            // Create execution context
            ExecutionContext ec = new ExecutionContext(gameObject);
            IAnchorable anchorable = GetComponentInParent<IAnchorable>();
            if (anchorable != null)
            {
                ec.Anchor = anchorable.Anchor;
            }

            _app.StartActivity(activityType, ec);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
