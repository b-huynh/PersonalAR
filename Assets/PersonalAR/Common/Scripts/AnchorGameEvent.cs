using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game Event/Anchor Game Event")]
public class AnchorGameEvent : GameEvent<AnchorableObject> {}

public class AnchorEventListener : IGameEventListener<AnchorableObject>
{
    public void OnEventRaised(AnchorableObject arg0)
    {

    }
}