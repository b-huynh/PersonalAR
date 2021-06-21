using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;

public class AnnotationModeController : MonoBehaviour
{
    public BoolVariable IsEnabled;

    [SerializeField]
    private GameObject annotationMenu;
    [SerializeField]
    private AnchorPlacement anchorPlacement;

    // Start is called before the first frame update
    void Start()
    {
        OnValueChanged(IsEnabled.RuntimeValue);
        IsEnabled.OnValueChanged += OnValueChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            IsEnabled.ToggleValue();
        }
    }

    void OnValueChanged(bool newValue)
    {
        anchorPlacement.enabled = newValue;
        if (newValue == true)
        {
            annotationMenu.GetComponent<ScaleTween>().TweenIn();
            annotationMenu.GetComponent<FollowMeToggle>().SetFollowMeBehavior(true);
        }
        else
        {
            annotationMenu.GetComponent<ScaleTween>().TweenOut();
        }
    }
}
