using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Assertions;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

public class Code
{
    public int Value;
    public string Label;
    public List<CodePiece> Pieces = new List<CodePiece>();

    public Code(string label, int code, int codeLength, int numPieces)
    {
        Value = code;
        Label = label;

        // Calculate intermediate values
        // int codeMin = Int32.Parse("1" + new String('0', codeLength - 1));
        // int codeMax = Int32.Parse(new String('9', codeLength));
        int pieceLength = codeLength / numPieces;

        // Split code into pieces and label
        string codeStr = code.ToString();

        for(int j = 0; j < numPieces; ++j)
        {
            string pieceValue = (j == numPieces - 1) ? 
                codeStr.Substring(j * pieceLength) : 
                codeStr.Substring(j * pieceLength, pieceLength);

            CodePiece codePiece = new CodePiece()
            {
                Code = this,
                Label = this.Label,
                Order = j,
                Value = pieceValue
            };

            Pieces.Add(codePiece);
        }
    }

    public override string ToString()
    {
        return $"{Label}-{Value}";
    }
}

public struct CodePiece
{
    public Code Code;
    public string Label;
    public int Order;
    public string Value;
}

[CreateAssetMenu(menuName = "Experiment/Random Pin Codes")]
public class RandomPinCodes : ScriptableObject
{
    [Header("Code Set")]
    [SerializeField] public int randomSeed;
    [SerializeField] public int numCodes;

    [Header("Properties")]
    [Range(4, 9)]
    [SerializeField] public int codeLength;
    [Range(1, 4)]
    [SerializeField] public int numPieces;


    [ReadOnly] public List<Code> Codes = new List<Code>();

    void Reset()
    {
        randomSeed = new System.Random().Next(1000, 9999);
        numCodes = 10;
        codeLength = 6;
        numPieces = 2;
    }

    void OnValidate()
    {
        Assert.IsTrue(numPieces < codeLength);
    }

    public void Awake()
    {
        Generate();
    }

    public void OnEnable()
    {
        Generate();
    }

    public void Generate()
    {
        // Generate random codes and labels
        System.Random rand = new System.Random(randomSeed);
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for(int i = 0; i < numCodes; ++i)
        {
            // Generate random code
            string label = letters[i].ToString();
            int randCode = rand.Next(100000, 999999);
            Code code = new Code(label, randCode, codeLength, numPieces);
            Codes.Add(code);
        }   
    }


    // [Header("Codes")]
    [ReadOnly] public List<int> pinCodes = new List<int>();

    // Scripting-only public variables
    [NonSerialized] public OrderedDictionary labeledCodePieces = new OrderedDictionary();
    [NonSerialized] public List<string> codePieceLabels = new List<string>();
    [NonSerialized] public List<string> codePieces = new List<string>();

    // public void Generate()
    // {
    //     // Clear container variables
    //     pinCodes.Clear();
    //     labeledCodePieces.Clear();

    //     // Calculate intermediate values
    //     int codeMin = Int32.Parse("1" + new String('0', codeLength - 1));
    //     int codeMax = Int32.Parse(new String('9', codeLength));
    //     int pieceLength = codeLength / numPieces;

    //     // Generate random codes and labels
    //     System.Random rand = new System.Random(randomSeed);
    //     const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    //     for(int i = 0; i < numCodes; ++i)
    //     {
    //         // Generate random code
    //         string label = letters[i].ToString();
    //         int code = rand.Next(100000, 999999);
    //         pinCodes.Add(code);

    //         Code c = new Code(label, code, codeLength, numPieces);
            

    //         // // Split code into pieces and label
    //         // string codeStr = code.ToString();
    //         // for(int j = 0; j < numPieces; ++j)
    //         // {
    //         //     string label = letters[i] + (j + 1).ToString();
    //         //     string piece = codeStr.Substring(j * pieceLength, pieceLength);
    //         //     if (j == numPieces - 1)
    //         //     {
    //         //         piece = codeStr.Substring(j * pieceLength);
    //         //     }
    //         //     labeledCodePieces.Add(label, piece);
    //         // }
    //     }
    // }

    private Dictionary<AnchorableObject, CodePiece> _assignment = new Dictionary<AnchorableObject, CodePiece>();
    public Dictionary<AnchorableObject, CodePiece> Assignment
    {
        get
        {
            // Get anchor service and return default if it doesnt yet exist.
            AnchorService anchorService;
            if (MixedRealityServiceRegistry.TryGetService<AnchorService>(out anchorService) == false)
            {
                Debug.Log("Cant get anchorservice");
                return _assignment;
            }
            
            // Create a new assignment based on current list of anchored objects.
            _assignment = Assign(anchorService.AnchoredObjects.Values.ToList());

            // if (_assignment.Count != anchorService.AnchorCount)
            // {
            //     _assignment = Assign(anchorService.AnchoredObjects.Values.ToList());
            // }

            return _assignment;
        }
    }

    // Assign generated code pieces to objects
    public void Assign()
    {
        AnchorService anchorService;
        if (MixedRealityServiceRegistry.TryGetService<AnchorService>(out anchorService))
        {
            Debug.Log(anchorService.AnchorCount);
        }
    }

    // Assign anchors to code pieces.
    public Dictionary<AnchorableObject, CodePiece> Assign(List<AnchorableObject> anchors)
    {
        // Check that we have the same amount of anchors and pieces. Otherwise wierdness happens...
        if (anchors.Count != numCodes * numPieces)
        {
            Debug.LogWarning($"Not enough anchors placed to assign code pieces. Need {numCodes * numPieces} pieces");
        }
        
        // Gather all CodePieces
        List<CodePiece> allCodePieces = new List<CodePiece>();
        foreach(Code code in Codes)
        {
            allCodePieces = allCodePieces.Concat(code.Pieces).ToList();
        }

        // Assign a code piece to each anchor
        var assignments = new Dictionary<AnchorableObject, CodePiece>();
        // Use the same random seed used to generate codes, for consistency.
        var rand = new System.Random(randomSeed);
        foreach(AnchorableObject anchor in anchors)
        {
            int pieceIndex = rand.Next(allCodePieces.Count);
            CodePiece assignablePiece = allCodePieces[pieceIndex];
            allCodePieces.RemoveAt(pieceIndex);

            assignments.Add(anchor, assignablePiece);
            ARDebug.Log($"Assigned: {anchor.WorldAnchorName} : {assignablePiece.Label}-{assignablePiece.Value}");
        }

        return assignments;
    }

    public bool Contains(int codeNum)
    {
        return Codes.Any(code => code.Value == codeNum);
    }
}
