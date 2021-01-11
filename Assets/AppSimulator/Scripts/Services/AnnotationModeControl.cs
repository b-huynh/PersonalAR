using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnotationModeControl : MonoBehaviour
{

     /// <summary>
    /// Class that exposes methods to toggle Annotation Mode, allowing manual placement of objects
    /// </summary>
    [AddComponentMenu("AppSimulator/Services/AnnotationModeControl")]

    public void ToggleAnnotationMode()
    {
        // AnnotationModeManager.Instance.ShowAnnotationModeContent =
            // !AnnotationModeManager.Instance.ShowAnnotationModeContent;
        AnnotationModeManager.Instance.gameObject.SetActive(!AnnotationModeManager.Instance.gameObject.activeSelf);
    }

    public void SetAnnotationModeActive(bool isActive)
    {
        // AnnotationModeManager.Instance.ShowAnnotationModeContent = isVisible;
        AnnotationModeManager.Instance.gameObject.SetActive(isActive);
    }

    public void CreateAnnotation()
    {
        AnnotationModeManager.Instance.Prime();
    }

    public void CreateTwoPointAnnotation()
    {
        AnnotationModeManager.Instance.ReadyTwoPointAnnotation();
    }
}
