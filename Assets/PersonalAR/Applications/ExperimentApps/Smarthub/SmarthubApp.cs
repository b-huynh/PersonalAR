using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions; 

public class SmarthubApp : BaseApp
{
    public RandomPinCodes randomPinCodes;
    public int maxObjectsWithCode;

    private AnchorService anchorService;
    public ObservableCollection<AnchorableObject> anchorsWithCode = new ObservableCollection<AnchorableObject>();
    public Dictionary<AnchorableObject, CodePiece> codeAssignments = new Dictionary<AnchorableObject, CodePiece>();
    private System.Random rnd = new System.Random();


    // Start is called before the first frame update
    void Start()
    {
        // Register for new object events
        IAnchorService serviceInterface;
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out serviceInterface))
        {
            anchorService = (AnchorService)serviceInterface;
            anchorService.OnAfterRegistered += OnObjectRegistered;
            anchorService.OnAfterRemoved += OnObjectRemoved;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void BeforeFirstActivityStart()
    {
        // Randomly choose maxObjectsWithCode from available anchors
        List<AnchorableObject> shuffledAnchors =
            anchorService.AnchoredObjects.Values.OrderBy((item) => rnd.Next())
                                                .Take(maxObjectsWithCode)
                                                .ToList();

        // Debug.Log($"Shuffled anchors count: {shuffledAnchors.Count}");

        foreach(AnchorableObject randomAnchor in shuffledAnchors)
        {
            // Get assignment
            CodePiece assignedPiece = randomPinCodes.GetAssignment(randomAnchor, 0);

            // Add to collections and add service handler
            // Debug.Log($"Adding anchor: {randomAnchor.WorldAnchorName}");
            codeAssignments.Add(randomAnchor, assignedPiece);
            anchorsWithCode.Add(randomAnchor);
            anchorService.AddHandler(randomAnchor, this.appState);
        }

        randomPinCodes.OnCodeEntryComplete.AddListener(OnCodeEntryComplete);
    }

    void OnCodeEntryComplete(CodeEntryCompleteEventArgs eventArgs)
    {
        // Find anchor that has been assigned this code
        List<AnchorableObject> toRemove =
            codeAssignments.Where(pair => pair.Value.Code == eventArgs.CompletedCode)
                           .Select(pair => pair.Key)
                           .ToList();

        // Remove as handler and from containers
        foreach(AnchorableObject anchorToRemove in toRemove)
        {
            codeAssignments.Remove(anchorToRemove); // ALWAYS DO CODEASSIGNMENTS BEFORE ANCHORSWITHCODE
            anchorsWithCode.Remove(anchorToRemove);
            anchorService.RemoveHandler(anchorToRemove, this.appState);
        }
        
        // Choose new random anchor and get assignment
        int nextAnchorIdx = rnd.Next(anchorService.AnchoredObjects.Count);
        AnchorableObject randomAnchor = anchorService.AnchoredObjects.Values.ElementAt(nextAnchorIdx);
        CodePiece assignedPiece = randomPinCodes.GetAssignment(randomAnchor, 0);

        // Add new random anchor to codeAssignments and anchorsWithCode and service handler
        codeAssignments.Add(randomAnchor, assignedPiece);
        anchorsWithCode.Add(randomAnchor);
        anchorService.AddHandler(randomAnchor, this.appState);
    }

    void OnObjectRegistered(AnchorableObject anchor)
    {
        // // Register events from first numSmartObjects encountered.
        // if (numAssigned < numSmartObjects)
        // {
        //     anchorService.AddHandler(anchor, appState);
        //     numAssigned += 1;
        // }
    }

    void OnObjectRemoved(String name)
    {
        // numAssigned -= 1;
    }
}
