using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions; 

// public class JSONSavable<T> : ISavable
// {
//     public T deserialized { get; private set; }

//     public void Save(string file)
//     {
//         using(StreamWriter writer = new StreamWriter(file))
//         {
//             string json = JsonUtility.ToJson(this);
//             writer.Write(json);
//         }
//     }

//     public void Load(string file)
//     {
//         using(StreamReader reader = new StreamReader(file))
//         {
//             string json = reader.ReadToEnd();
//             deserialized = JsonUtility.FromJson<T>(json);
//         }
//     }
// }

[System.Serializable]
public class LearnAppData
{
    public List<FlashCardData> flashCards = new List<FlashCardData>();
}

public class PervasiveLearnApp : BasePervasiveApp
{
    [Header("Learn App Settings")]
    // Learning Activity Prefab
    [SerializeField] private LearnObjectMenu objectMenu;

    // Learning State
    public Translator.TargetLanguage language;
    private LearnAppData _appData;

    private List<string> keywords = new List<string>
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

        // Load saved app data
        _appData = appState.Load<LearnAppData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            FlashCardData fc = new FlashCardData();
            fc.numResponses = Random.Range(1, 5);
            fc.dueDateSerialized = System.DateTime.Now.ToString("o");

            _appData.flashCards.Add(fc);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            appState.Save(_appData);
        }
    }

    void OnObjectRegistered(AnchorableObject anchor)
    {
        GameObject menu = objectMenu.gameObject.Instantiate(appState);
        // AppEntity menu = AppEntity.Instantiate(objectTagPrefab, this);
        objectMenu.GetComponent<LearnObjectMenu>().targetLanguage = language;
        menu.GetComponent<IAnchorable>().Anchor = anchor;
    }
}
