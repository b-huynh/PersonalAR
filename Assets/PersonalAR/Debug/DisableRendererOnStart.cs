using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererActiveOnStart : MonoBehaviour
{
    public bool ValueOnStart;

    // Start is called before the first frame update
    void Start()
    {          
        GetComponent<Renderer>().enabled = ValueOnStart;
    }
}
