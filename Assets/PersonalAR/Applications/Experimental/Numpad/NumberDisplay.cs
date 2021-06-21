using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class NumberDisplay : MonoBehaviour
{
    public int maxLength;
    [SerializeField] private TextMeshPro textMesh;

    public RandomPinCodes pinCodes {get; set;}

    void OnEnable()
    {
        Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        textMesh.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enter(string str)
    {
        if (textMesh.text.Length < maxLength)
        {
            textMesh.text += str;
        }
        textMesh.color = Color.white;
    }

    public void Delete()
    {
        textMesh.text = textMesh.text.Remove(textMesh.text.Length - 1);
        textMesh.color = Color.white;
    }

    public void Clear()
    {
        textMesh.text = string.Empty;
        textMesh.color = Color.white;
    }

    public void Validate()
    {
        int entered = System.Int32.Parse(textMesh.text);
        if (pinCodes.pinCodes.Contains(entered))
        {
            textMesh.color = Color.green;
        }
        else
        {
            textMesh.color = Color.red;
        }
    }
}
