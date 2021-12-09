using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmarthubObjectActivity : AnchorActivity
{
    // Shared amongst all activities
    public static List<int> unassignedLabelIndices;
    public static Dictionary<AnchorableObject, string> assignedCodeSetLabels;
    public static Dictionary<AnchorableObject, string> assignedCodePieces;

    [Header("Smarthub Object Activity")]
    [SerializeField] public RandomPinCodes codeSet;
    [ReadOnly] public AnchorableObject anchor;
    [ReadOnly] public CodePiece codePiece;

    // [ReadOnly] public int labelIndex;
    // [ReadOnly] public string codeSetLabel;
    // [ReadOnly] public string codePiece;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public override void StartActivity(ExecutionContext executionContext)
    {
        base.StartActivity(executionContext);

        // Get assigned anchor 
        anchor = executionContext.Anchor;

        // Get and code piece
        // var assignment = codeSet.Assignment;

        var assignableObjects = new List<AnchorableObject>() { anchor };
        Dictionary<AnchorableObject, CodePiece> assignment = codeSet.AssignCodePieces(assignableObjects, 0);
        codePiece = assignment[anchor];

        // Update visual elements
        cachedEntity.GetComponent<SmartInfoMenu>().SetSerialNumber($"(CODE) {codePiece.Label}-{codePiece.Value}");

        // Add smart info entity anchor content
        cachedEntity.GetComponentInChildren<IAnchorable>().Anchor = executionContext.Anchor;
        executionContext.Anchor.GetComponentInChildren<AnchorContentController>().AddEntity(
            cachedEntity.GetComponent<SmartInfoMenu>());


        // ************* OLD ASSIGNMENT CODE. TO BE DELETED. ***********************
        // // Create unassigned label indexes if it doesn't yet exist (e.g. this is first activity)
        // if (unassignedLabelIndices == null)
        // {
        //     var indices = Enumerable.Range(0, codeSet.labeledCodePieces.Count).ToList();
        //     unassignedLabelIndices = indices.OrderBy(a => System.Guid.NewGuid()).ToList();

        //     // Assigned dictionaries keep track of already assigned code pieces to reuse.
        //     assignedCodeSetLabels = new Dictionary<AnchorableObject, string>();
        //     assignedCodePieces = new Dictionary<AnchorableObject, string>();
        // }

        // // First check if we already have this assigned.
        // if (assignedCodePieces.ContainsKey(executionContext.Anchor))
        // {
        //     codeSetLabel = assignedCodeSetLabels[executionContext.Anchor];
        //     codePiece = assignedCodePieces[executionContext.Anchor];
        // }
        // // Else, assign valid code piece (if available)
        // else if (unassignedLabelIndices.Count > 0)
        // {
        //     // Grab first available index
        //     labelIndex = unassignedLabelIndices[0];
        //     unassignedLabelIndices.RemoveAt(0);

        //     // Assign label and code piece
        //     string[] codeLabels = new string[codeSet.labeledCodePieces.Count];
        //     string[] codePieces = new string[codeSet.labeledCodePieces.Count];
        //     codeSet.labeledCodePieces.Keys.CopyTo(codeLabels, 0);
        //     codeSet.labeledCodePieces.Values.CopyTo(codePieces, 0);
        //     codeSetLabel = codeLabels[labelIndex];
        //     codePiece = codePieces[labelIndex];

        //     assignedCodeSetLabels.Add(executionContext.Anchor, codeSetLabel);
        //     assignedCodePieces.Add(executionContext.Anchor, codePiece);
        // }
        // *****************************************************************************
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
