using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Recaug;

public class DebugObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ObjectRegistration registration = GetComponent<ObjectRegistration>();
        TextMesh textMesh = GetComponentInChildren<TextMesh>();
        textMesh.text = registration.className;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
