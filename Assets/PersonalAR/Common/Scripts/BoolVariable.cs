using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Bool Variable")]
public class BoolVariable : Variable<bool>
{
    public void ToggleValue()
    {
        SetValue(!RuntimeValue);
        ARDebug.Log($"BoolVariable {name} set to {RuntimeValue}");
    }
}
