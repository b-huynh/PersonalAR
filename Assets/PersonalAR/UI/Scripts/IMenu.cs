using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IMenu : IOpen, IClose, IToggle
{
    bool Visible { get; }
}
