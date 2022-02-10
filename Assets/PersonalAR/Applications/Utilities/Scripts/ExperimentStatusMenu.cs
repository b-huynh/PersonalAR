using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ExperimentStatusMenu : MonoBehaviour
{
    public TextMeshPro statusDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        statusDisplay.text = ExperimentManager.Instance.Status;
    }
}
