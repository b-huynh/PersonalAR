using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssignmentPair
{
    // public AnchorableObject Anchor; // FIX: Type Mismatch. cannot serialize a game object that exists in scene onto scriptableobject.
    public int InstanceID;
    public CodePiece CodePiece;  // FIX: Recursive serialization
    public AssignmentPair(int instanceID, CodePiece codePiece)
    {
        // this.Anchor = anchor;
        this.InstanceID = instanceID;
        this.CodePiece = codePiece;
    }
}

[Serializable]
[CreateAssetMenu(menuName = "Applications/App Variables/Smarthub Variables")]
public class SmarthubVariables : AppVariables
{
    public List<AssignmentPair> AssignmentPairs = new List<AssignmentPair>();
}
