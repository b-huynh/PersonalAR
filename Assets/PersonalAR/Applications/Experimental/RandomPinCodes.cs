using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Assertions;

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

    [Header("Codes")]
    [ReadOnly] public List<int> pinCodes = new List<int>();

    // Scripting-only public variables
    [NonSerialized] public OrderedDictionary labeledCodePieces = new OrderedDictionary();
    [NonSerialized] public List<string> codePieceLabels = new List<string>();
    [NonSerialized] public List<string> codePieces = new List<string>();

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
        // Clear container variables
        pinCodes.Clear();
        labeledCodePieces.Clear();

        // Calculate intermediate values
        int codeMin = Int32.Parse("1" + new String('0', codeLength - 1));
        int codeMax = Int32.Parse(new String('9', codeLength));
        int pieceLength = codeLength / numPieces;

        // Generate random codes and labels
        System.Random rand = new System.Random(randomSeed);
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for(int i = 0; i < numCodes; ++i)
        {
            // Generate random code
            int code = rand.Next(100000, 999999);
            pinCodes.Add(code);

            // Split code into pieces and label
            string codeStr = code.ToString();
            for(int j = 0; j < numPieces; ++j)
            {
                string label = letters[i] + (j + 1).ToString();
                string piece = codeStr.Substring(j * pieceLength, pieceLength);
                if (j == numPieces - 1)
                {
                    piece = codeStr.Substring(j * pieceLength);
                }
                labeledCodePieces.Add(label, piece);
            }            
        }
    }
}
