using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VariableListener<T> : MonoBehaviour
{
    public Variable<T> variable;

    private void OnEnable()
    {
        // Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        // Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        // Response.Invoke(Variable.RuntimeValue);
    }
}
