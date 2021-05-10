using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppCardsManager : ScriptableObject
{
    private Dictionary<AnchorableObject, BasePervasiveApp> ObjectAppLinks;
    private Dictionary<AnchorableObject, List<GameObject>> ObjectCards;
    private Dictionary<BasePervasiveApp, List<GameObject>> AppCards;


    void AddAppCard(AnchorableObject anchor, GameObject appCard)
    {
    }

    void RemoveAppCard(AnchorableObject anchor, GameObject appCard)
    {

    }
}
