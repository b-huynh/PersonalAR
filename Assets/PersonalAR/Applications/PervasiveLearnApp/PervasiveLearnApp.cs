using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions; 

using Recaug;
using Recaug.Client;

public class PervasiveLearnApp : BasePervasiveApp
{
    [Header("Learn App Settings")]
    // Learning Activity Prefab
    [SerializeField]
    private AppEntity objectTagPrefab;

    // Learning State
    public Translator.TargetLanguage language;
    private List<string> knownWords = new List<string>
    {
        "chair",
        "tv",
        "keyboard",
        "mouse",
        "sofa",
        "dining table",
        "microwave",
    };

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        // Register for new object events
        IAnchorService anchorService;
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out anchorService))
        {
            anchorService.OnRegistered += OnObjectRegistered;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnObjectRegistered(AnchorableObject anchor)
    {
        AppEntity menu = AppEntity.Instantiate(objectTagPrefab, this);
        menu.GetComponent<IAnchorable>().SetAnchor(anchor);
    }
}
