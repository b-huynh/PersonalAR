using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicTransform : MonoBehaviour
{
    public Transform Base;
    public Transform Target;

    // Start is called before the first frame update
    void Start()
    {
        if (Target == null)
        {
            Target = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Target.SetPositionAndRotation(Base.position, Base.rotation);
    }
}
