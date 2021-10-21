using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AnchorableEvent : UnityEvent<AnchorableObject> {}

public class Anchorable : MonoBehaviour, IAnchorable
{
    [SerializeField]
    private AnchorableObject _anchor;
    public AnchorableObject Anchor
    {
        get => _anchor;
        set
        {
            // Check if we are removing old anchor.
            if (_anchor != null && _anchor != value)
            {
                OnAnchorRemoved.Invoke(_anchor);
            }

            // Set new anchor.
            _anchor = value;

            if (_anchor != null)
            {
                OnAnchorSet.Invoke(_anchor);
            }
        }
    }
    public void SetAnchor(AnchorableObject anchor) => Anchor = anchor;
    public void RemoveAnchor() => Anchor = null;

    public bool Anchored
    {
        get => _anchor != null;
    }

    public AnchorableEvent OnAnchorSet;
    public AnchorableEvent OnAnchorRemoved;

    void OnEnable()
    {
        if (_anchor != null)
        {
            // SetAnchor(_anchor);
            OnAnchorSet.Invoke(_anchor);
        }
    }

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {}
}
