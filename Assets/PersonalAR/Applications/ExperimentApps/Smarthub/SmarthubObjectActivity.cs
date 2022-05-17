using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class SmarthubObjectActivity : AnchorActivity
{
    [Header("Smarthub Object Activity")]
    [SerializeField] public RandomPinCodes codeSet;
    [ReadOnly] public AnchorableObject anchor;
    [ReadOnly] public CodePiece codePiece;

    private SmarthubVariables AppVariables;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public override void StartActivity(ExecutionContext executionContext)
    {
        // Debug.Log("SmarthubObjectActivity StartActivity");
        bool originalInitState = initialized;

        base.StartActivity(executionContext); // This can change original initialization state
        
        if (originalInitState == false)
        {
            AppVariables = (SmarthubVariables)appState.Variables;

            // Get assigned anchor 
            anchor = executionContext.Anchor;

            // Check initial code assignments in parent app
            SmarthubApp smarthubApp = (SmarthubApp)this.appRuntime;
            codePiece = 
                smarthubApp.anchorsWithCode.Contains(this.anchor) ?
                smarthubApp.codeAssignments[this.anchor] :
                null;

            // Update visuals to match
            UpdateVisuals();

            // Subscribe to future changes to assignments
            smarthubApp.anchorsWithCode.CollectionChanged += OnCollectionChanged;

            // Add smart info entity anchor content
            cachedEntity.GetComponentInChildren<IAnchorable>().Anchor = anchor;
            anchor.GetComponentInChildren<AnchorContentController>().AddEntity(
                cachedEntity.GetComponent<SmartInfoMenu>());

            initialized = true;
        }
        
        // Set rendering layer
        cachedEntity.SetLayerInChildren(LayerMask.NameToLayer("Default"));
    }

    public override void StopActivity(ExecutionContext ec)
    {
        // Debug.Log("SmarthubObjectActivity StopActivity");
        base.StopActivity(ec);
        cachedEntity.SetLayerInChildren(LayerMask.NameToLayer("SuspendRendering"));
    }

    // Update is called once per frame
    public void Update()
    {

    }

    private void UpdateVisuals()
    {
        // Update visual elements
        if (codePiece != null)
        {
            cachedEntity.GetComponent<SmartInfoMenu>().SetSerialNumber($"(CODE) <color=#FF7F7F>{codePiece.Label}-{codePiece.Value}</color>");
        }
        else
        {
            // cachedEntity.GetComponent<SmartInfoMenu>().SetSerialNumber($"SERIAL NUMBER UNKNOWN");
            cachedEntity.GetComponent<SmartInfoMenu>().SetSerialNumber($"");
        }
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        if (eventArgs.Action == NotifyCollectionChangedAction.Remove &&
            eventArgs.OldItems.Contains(this.anchor))
        {
            codePiece = null;
            UpdateVisuals();
        }

        if (eventArgs.Action == NotifyCollectionChangedAction.Add &&
            eventArgs.NewItems.Contains(this.anchor))
        {
            codePiece = ((SmarthubApp)this.appRuntime).codeAssignments[this.anchor];
            UpdateVisuals();
        }
    }
}
