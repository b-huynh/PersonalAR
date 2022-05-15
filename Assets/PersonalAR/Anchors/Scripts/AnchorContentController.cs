using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;

public class AnchorContentController : MonoBehaviour
{
    private bool IsOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.GetComponent<ScaleTween>().TweenIn();
        }

        IsOpen = true;
    }

    public void Close()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.GetComponent<ScaleTween>().TweenOut();
        }

        IsOpen = false;
    }

    public void Toggle()
    {
        if (IsOpen) { Close(); }
        else { Open(); }
    }

    public void AddEntity(BaseEntity entity)
    {
        Anchorable anchorable;
        if (entity.TryGetComponent<Anchorable>(out anchorable))
        {
            anchorable.Anchor = this.GetComponentInParent<AnchorableObject>();
            anchorable.transform.parent = this.transform;
            anchorable.transform.position = Vector3.zero;
            anchorable.transform.rotation = Quaternion.identity;

            GetComponent<GridObjectCollection>().UpdateCollection();

            // Add small bias towards camera
            Vector3 toCamDir = (Camera.main.transform.position - anchorable.transform.position).normalized;
            anchorable.transform.position += toCamDir * 0.2f;
        }
    }
}