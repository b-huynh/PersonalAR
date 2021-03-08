using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AppUtils
{
    public static void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach(Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    public static void SetLayerRecursive(GameObject obj, string layerName)
    {
        SetLayerRecursive(obj, LayerMask.NameToLayer(layerName));
    }
}
