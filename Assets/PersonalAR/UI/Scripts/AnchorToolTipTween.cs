using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;

public class AnchorToolTipTween : MonoBehaviour
{
    private ToolTip _toolTip;
    private ToolTipConnector _connector;

    public LeanTweenType inTween;
    public float inTweenTo;
    public LeanTweenType outTween;
    public float outTweenTo;

    public float duration;

    void OnEnable()
    {
        _toolTip = GetComponent<ToolTip>();
        _connector = GetComponent<ToolTipConnector>();

        _toolTip.ShowBackground = false;
        _toolTip.ShowConnector = false;
        _connector.PivotDistance = outTweenTo;

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TweenIn()
    {
        _toolTip.ShowBackground = true;
        _toolTip.ShowConnector = true;
        LeanTween.value(outTweenTo, inTweenTo, duration)
            .setEase(inTween)
            .setOnUpdate(
                (float val) =>
                {
                    _connector.PivotDistance = val;
                }
            );
    }

    public void TweenOut()
    {
        _toolTip.ShowBackground = false;
        _toolTip.ShowConnector = false;
        LeanTween.value(inTweenTo, outTweenTo, duration)
            .setEase(outTween)
            .setOnUpdate(
                (float val) =>
                {
                    _connector.PivotDistance = val;
                }
            );
    }
}
