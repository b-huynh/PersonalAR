using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Int Variable")]
public class IntVariable : Variable<int> {
    public void incrementValue()
    {
        RuntimeValue++;
        Debug.Log(RuntimeValue);
        OnValueChanged?.Invoke(RuntimeValue);
    }

    public void decrementValue()
    {
        RuntimeValue--;
        Debug.Log(RuntimeValue);
        OnValueChanged?.Invoke(RuntimeValue);
    }
}
