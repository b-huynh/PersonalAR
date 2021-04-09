using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchorable : MonoBehaviour, IAnchorable
{
    [SerializeField]
    private AnchorableObject _anchor;
    public string AnchorName
    {
        get => _anchor.WorldAnchorName;
    }

    public event Action<AnchorableObject> OnAnchorSet;

    void OnEnable()
    {
        if (_anchor != null)
        {
            SetAnchor(_anchor);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAnchor(AnchorableObject anchor)
    {
        _anchor = anchor;
        OnAnchorSet?.Invoke(_anchor);
    }
}
