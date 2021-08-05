using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorialItem : IMenu
{
    int ItemOrder { get; }
}
