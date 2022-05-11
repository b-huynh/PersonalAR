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
        base.StartActivity(executionContext);

        AppVariables = (SmarthubVariables)appState.Variables;

        // Get assigned anchor 
        anchor = executionContext.Anchor;

        // CodePiece newCodePiece = codeSet.GetAssignment(anchor, 0);
        // UpdateCode(newCodePiece);

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



        // codePiece.Code.OnCodeEntryComplete.AddListener(this.OnCodeEntryComplete);

        // if (GetExistingAssignment() == false)
        // {
        //     GetNewAssignment();
        // }
    }

    public override void StopActivity(ExecutionContext ec)
    {
        base.StopActivity(ec);

        if (codePiece != null)
        {
            codePiece.Code.OnCodeEntryComplete.RemoveListener(this.OnCodeEntryComplete);
        }
    }

    // Update is called once per frame
    public void Update()
    {

    }

    // TODO: Make templated/generalize so assignments in other apps can use.
    // private bool GetExistingAssignment()
    // {
    //     int anchorID = anchor.GetInstanceID();
    //     var unfinishedCodeAssignments =
    //         AppVariables.AssignmentPairs.Where(pair => pair.InstanceID == anchorID && pair.CodePiece.Code.Used == false);

    //     if (unfinishedCodeAssignments.Count() > 0)
    //     {
    //         codePiece = unfinishedCodeAssignments.First().CodePiece;
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    // private void GetNewAssignment()
    // {
    //     // Get code piece
    //     var assignableObjects = new List<AnchorableObject>() { anchor };
    //     Dictionary<AnchorableObject, CodePiece> assignment = codeSet.AssignCodePieces(assignableObjects, 0);
    //     codePiece = assignment[anchor];

    //     // Update app state variables
    //     SmarthubVariables variables = (SmarthubVariables)appState.Variables;
    //     // variables.AssignmentPairs.Add(new AssignmentPair(anchor, codePiece));
    //     variables.AssignmentPairs.Add(new AssignmentPair(anchor.GetInstanceID(), codePiece));

    //     // Subscribe to code completion events
    //     codePiece.Code.OnCodeEntryComplete.AddListener(this.OnCodeEntryComplete);
    // }

    private void UpdateVisuals()
    {
        // Update visual elements
        if (codePiece != null)
        {
            cachedEntity.GetComponent<SmartInfoMenu>().SetSerialNumber($"(CODE) {codePiece.Label}-{codePiece.Value}");
        }
        else
        {
            cachedEntity.GetComponent<SmartInfoMenu>().SetSerialNumber($"SERIAL NUMBER UNKNOWN");
        }
    }

    private void OnCodeEntryComplete()
    {
        // Clear completion event handlers
        codePiece.Code.OnCodeEntryComplete.RemoveListener(this.OnCodeEntryComplete);
        UpdateCode(null);

        // Get a new assignment
        // CodePiece newCodePiece = codeSet.GetAssignment(anchor, 0);
        // UpdateCode(newCodePiece);
        // codePiece.Code.OnCodeEntryComplete.AddListener(this.OnCodeEntryComplete);
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

    // Update code. Set newCodePiece to null to designate at an object without any codes.
    public void UpdateCode(CodePiece newCodePiece)
    {
        if (newCodePiece != null)
        {
            codePiece = newCodePiece;
            UpdateVisuals();
            codePiece.Code.OnCodeEntryComplete.AddListener(this.OnCodeEntryComplete);
        }
        else
        {
            codePiece = null;
            UpdateVisuals();
        }
    }
}
