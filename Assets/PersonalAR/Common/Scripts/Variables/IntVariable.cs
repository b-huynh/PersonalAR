using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Int Variable")]
public class IntVariable : Variable<int> {
    public void incrementValue()
    {
        RuntimeValue++;
        OnValueChanged?.Invoke(RuntimeValue);
    }

    public void decrementValue()
    {
        RuntimeValue--;
        OnValueChanged?.Invoke(RuntimeValue);
    }
}
