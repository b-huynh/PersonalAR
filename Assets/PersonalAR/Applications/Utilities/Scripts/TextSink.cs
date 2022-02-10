using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class TextSink : MonoBehaviour
{
    public ITextSource textSource;

    public TextMeshPro textSink;

    // Start is called before the first frame update
    void Start()
    {
        if (textSink == null)
        {
            textSink = GetComponent<TextMeshPro>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        textSink.text = textSource.TextSource;
    }
}
