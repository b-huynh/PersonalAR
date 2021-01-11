using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedScaleRatio : MonoBehaviour
{
    public enum FixedDimension {FixedX, FixedY, FixedZ, FixedAll}

    public FixedDimension fixedDimension;

    private Vector3 originalScale;
    private Transform originalParent;

    // Start is called before the first frame update
    void Start()
    {
        originalParent = transform.parent;
        transform.parent = null;
        originalScale = transform.localScale;
        transform.parent = originalParent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.parent = null;

        var scaleTmp = transform.localScale;
        switch(fixedDimension)
        {
            case FixedDimension.FixedX:
                scaleTmp.x = originalScale.x;
                break;
            case FixedDimension.FixedY:
                scaleTmp.y = originalScale.y;
                break;
            case FixedDimension.FixedZ:
                scaleTmp.z = originalScale.z;
                break;
            case FixedDimension.FixedAll:
                scaleTmp = originalScale;
                break;
        }
        transform.localScale = scaleTmp;
        
        transform.parent = originalParent;
    }
}
