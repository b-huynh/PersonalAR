using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

[Serializable]
public class Code
{
    public int Value;
    public string Label;
    public List<CodePiece> Pieces = new List<CodePiece>();

    // Mark is code has already been used and completed
    public bool Used { get; private set; }
    public UnityEvent OnCodeEntryComplete;

    private int codeLength;
    private int numPieces;

    public Code(string label, int code, int codeLength, int numPieces)
    {
        this.Value = code;
        this.Label = label;

        this.codeLength = codeLength;
        this.numPieces = numPieces;

        this.Pieces = SplitIntoPieces(code, codeLength, numPieces);

        this.Used = false;
        this.OnCodeEntryComplete = new UnityEvent();
    }

    public override string ToString()
    {
        return $"{Label}-{Value}";
    }

    public void SetUsed(bool value)
    {
        if (Used == false && value == true)
        {
            Used = true;
            OnCodeEntryComplete.Invoke();
        }
    }

    private List<CodePiece> SplitIntoPieces(int code, int codeLength, int numPieces)
    {
        List<CodePiece> codePieces = new List<CodePiece>();

        // Calculate intermediate values
        // int codeMin = Int`2.Parse("1" + new String('0', codeLength - 1));
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

            codePieces.Add(codePiece);
        }

        return codePieces;
    }

    public bool CompareEntry(int entry)
    {
        string entryStr = entry.ToString();
        if (entryStr.Length != codeLength)
        {
            return false;
        }

        List<CodePiece> entryPieces = SplitIntoPieces(entry, codeLength, numPieces);
        
        // Remove each match from remaining pieces
        List<string> remainingPieceValues = new List<string>();
        Pieces.ForEach(piece => remainingPieceValues.Add(piece.Value));

        bool isEquivalent = (Pieces.Count == entryPieces.Count);
        foreach(CodePiece entryPiece in entryPieces)
        {
            if (remainingPieceValues.Contains(entryPiece.Value))
            {
                remainingPieceValues.Remove(entryPiece.Value);
                isEquivalent = (isEquivalent && true);
            }
            else
            {
                isEquivalent = (isEquivalent && false);
            }
        }
        return isEquivalent;
    }
}

/*
Example Code:
Code: A-123456
Label: A
Order: 2
Value: 34
*/
[Serializable]
public class CodePiece
{
    [System.NonSerialized]
    public Code Code;
    public UnityEvent OnCodeEntryComplete => Code.OnCodeEntryComplete;

    public string Label;
    public int Order;
    public string Value;
    public bool Assigned; // Bit to flag for assignment
}

public class CodeEntryCompleteEventArgs : EventArgs
{
    public Code CompletedCode { get; set; }
}

[CreateAssetMenu(menuName = "Experiment/Random Pin Codes")]
public class RandomPinCodes : ScriptableObject
{
    [Header("Code Set")]
    [SerializeField] public int randomSeed;
    [Range(1, 676)]
    [SerializeField] public int numCodes;

    [Header("Properties")]
    [Range(4, 9)]
    [SerializeField] public int codeLength;
    [Range(1, 4)]
    [SerializeField] public int numPieces;


    [ReadOnly] public List<Code> Codes = new List<Code>();

    List<KeyValuePair<System.Object, CodePiece>> AssignmentHistory = new List<KeyValuePair<object, CodePiece>>();
 
    public UnityEvent<CodeEntryCompleteEventArgs> OnCodeEntryComplete;

    public int CodesCompleted
    {
        get
        {
            return Codes.Where(code => code.Used == true).Count();
        }
    }

    public void Reset()
    {
        randomSeed = new System.Random().Next(1000, 9999);
        AssignmentHistory.Clear();
        Codes.Clear();
        // numCodes = 10;
        // codeLength = 6;
        // numPieces = 2;
    }

    void OnValidate()
    {
        Assert.IsTrue(numPieces < codeLength);
    }

    public void Awake()
    {
        Reset();
        Generate();
    }

    public void OnEnable()
    {
        Reset();
        Generate();
    }

    public void Generate()
    {
        Codes.Clear();

        // Generate random codes and labels
        System.Random rand = new System.Random(randomSeed);
        const string letters1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string letters2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        List<string> labels = new List<string>();
        foreach(char letter1 in letters1)
        {
            foreach(char letter2 in letters2)
            {
                string newLabel = letter1.ToString() + letter2.ToString();
                labels.Add(newLabel);
            }
        }
        Shuffle(labels, randomSeed);

        for(int i = 0; i < numCodes; ++i)
        {
            int randCode;
            string label = labels[i].ToString();
            do
            {
                // Generate random code
                randCode = NextRandomCode(rand);
            }
            while(Codes.Any(code => code.CompareEntry(randCode) == true));

            Code code = new Code(label, randCode, codeLength, numPieces);
            Codes.Add(code);
        }
    }

    public bool HasLeadingZeroPieces(int randCode)
    {
        string codeString = randCode.ToString();
        return codeString[0] == '0' || codeString[2] == '0' || codeString[4] == '0';
    }

    public int NextRandomCode(System.Random rand)
    {
        int randCode;
        do
        {
            randCode = rand.Next(100000, 999999);
        }
        while(HasLeadingZeroPieces(randCode));
        return randCode;
    }

    private CodePiece AssignNewCodePiece<T> (T assignable, int pieceIndex)
    {
        CodePiece newCodePiece = null;
        foreach(Code code in Codes)
        {
            CodePiece piece = code.Pieces[pieceIndex];
            if (piece.Assigned == false)
            {
                piece.Assigned = true;
                AssignmentHistory.Add(new KeyValuePair<object, CodePiece>(assignable, piece));
                newCodePiece = piece;
                break;
            }
        }

        if (newCodePiece == null)
        {
            Debug.LogWarning("Ran out of code pieces to assign");
        }

        return newCodePiece;
    }

    private Dictionary<T, CodePiece> AssignNewCodePiece<T>(List<T> assignables, int pieceIndex)
    {
        if (assignables.Count > Codes.Count)
        {
            Debug.LogWarning($"Not enough codes ({Codes.Count}) for assignables ({assignables.Count}) of type {typeof(T)}.");
        }

        Dictionary<T, CodePiece> assigned = assignables.ToDictionary(x => x, x => AssignNewCodePiece(x, pieceIndex));
        return assigned;
    }

    private CodePiece GetExistingAssignment<T>(T queryObject)
    {
        if (HasExistingAssignment(queryObject))
        {
            return AssignmentHistory.Where(kv => kv.Key == (object)queryObject && kv.Value.Code.Used == false).First().Value;
        }
        else
        {
            return null;
        }
    }

    private bool HasExistingAssignment<T>(T queryObject)
    {
        var currentAssignments = AssignmentHistory.Where(kv => kv.Key == (object)queryObject && kv.Value.Code.Used == false);
        return currentAssignments.Count() > 0;
    }

    public CodePiece GetAssignment<T>(T assignable, int pieceIndex)
    {
        CodePiece assignment = GetExistingAssignment(assignable);
        if (assignment != null)
        {
            return assignment;
        }
        else
        {
            return AssignNewCodePiece(assignable, pieceIndex);
        }
    }

    public Dictionary<T, CodePiece> GetAssignment<T>(List<T> assignables, int pieceIndex)
    {
        if (assignables.Count > Codes.Count)
        {
            Debug.LogWarning($"Not enough codes ({Codes.Count}) for assignables ({assignables.Count}) of type {typeof(T)}.");
        }

        return assignables.ToDictionary(x => x, x => GetAssignment(x, pieceIndex));
    }

    public void MarkCodeEntryComplete(int codeEntry)
    {
        foreach(Code code in Codes)
        {
            if (code.Used == false && code.CompareEntry(codeEntry))
            {
                code.SetUsed(true);

                var eventArgs = new CodeEntryCompleteEventArgs();
                eventArgs.CompletedCode = code;

                OnCodeEntryComplete.Invoke(eventArgs);
                return;
            }
        }
    }

    // ONLY checks among codes that have all pieces Assigned"
    public bool Contains(int codeNum)
    {
        return Codes.Any(code => code.CompareEntry(codeNum));
    }

    public static void Shuffle<T>(IList<T> list, int seed)
    {
        var rng = new System.Random(seed);
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
