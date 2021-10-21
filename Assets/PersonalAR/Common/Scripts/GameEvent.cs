using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Event/Game Event")]
public class GameEvent : ScriptableObject
{
    protected List<IGameEventListener> listeners = new List<IGameEventListener>();

    public void AddListener(IGameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(IGameEventListener listener)
    {
        listeners.Remove(listener);
    }

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }
}

public class GameEvent<T0> : ScriptableObject
{
    private List<IGameEventListener<T0>> listeners = new List<IGameEventListener<T0>>();

    public void AddListener(IGameEventListener<T0> listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(IGameEventListener<T0> listener)
    {
        listeners.Remove(listener);
    }

    public void Raise(T0 arg0)
    {
        for(int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised(arg0);
    }
}