using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Only 1 BaseEntity can exist within a GameObject hierarchy.
public class BaseEntity : MonoBehaviour
{
    void Start() {}
    void Update() {}
    protected virtual void Reset() => EnforceHierarchy();
    protected virtual void OnValidate() => EnforceHierarchy();
    private void EnforceHierarchy()
    {
        BaseEntity[] parentEntities = GetComponentsInParent<BaseEntity>();
        BaseEntity[] childEntities = GetComponentsInChildren<BaseEntity>();

        if (parentEntities.Length != 1 || childEntities.Length != 1)
        {
            Debug.LogError($"Only one BaseEntity allowed in hierarchy!");
        }
    }
}
