using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugModeController : MonoBehaviour
{
    public BoolVariable IsEnabled;
    public bool LaunchOnStartup;

    [SerializeField]
    private GameObject debugMenu;

    void Awake()
    {
        if (LaunchOnStartup)
        {
            IsEnabled.SetValue(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        OnValueChanged(IsEnabled.RuntimeValue);
        IsEnabled.OnValueChanged += OnValueChanged;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnValueChanged(bool newValue)
    {
        if (newValue == true)
            debugMenu.GetComponent<ScaleTween>().TweenIn();
        else
            debugMenu.GetComponent<ScaleTween>().TweenOut();
    }
}
