using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRendererOnStart : MonoBehaviour
{
    public bool ValueOnStart;
    // Start is called before the first frame update
    void Start()
    {          
        GetComponent<Renderer>().enabled = ValueOnStart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
