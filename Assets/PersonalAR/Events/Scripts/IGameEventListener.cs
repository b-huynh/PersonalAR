using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEventListener
{
    void OnEventRaised();   
}

public interface IGameEventListener<T0>
{
    void OnEventRaised(T0 arg0);
}
