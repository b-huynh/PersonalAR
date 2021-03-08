using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IMenu
{
    bool Visible { get; }

    void Open();
    void Close();
    void Toggle();
}
