using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class TutorialItem : AnimatedMenu, ITutorialItem
{
    // // Start is called before the first frame update
    // protected override void Start()
    // {
    //     base.Start();
    // }

    // // Update is called once per frame
    // protected override void Update()
    // {
    //     base.Update();
    // }

    [Header("Visual Components")]
    [SerializeField] private TextMeshPro bodyTextMesh;
    [TextArea(5, 8)] public string bodyText;

    [SerializeField] private TextMeshPro stepTextMesh;




    [Header("Tutorial Data")]
    [SerializeField] private DialogueData tutorialData;
    public Dialogue dialogue { get; private set; }

    // *** ITutorialItem Implementation ***
    [SerializeField] private int m_itemOrder;
    public int ItemOrder
    {
        get => m_itemOrder;
    }
    // *** End ITutorialItem ***
    public BoolVariable CloseCondition;

    public static List<TutorialItem> GetTutorialItems()
    {
        return Resources.FindObjectsOfTypeAll<TutorialItem>()
            .Where(item => item.gameObject.scene.IsValid())
            .OrderBy(item => item.ItemOrder)
            .ToList();
    }

    public void OnValidate()
    {
        if (tutorialData == null || tutorialData.Dialogues.Count <= m_itemOrder)
        {
            Debug.Log("Invalid Tutorial Data");
            return;
        }
        else
        {
            dialogue = tutorialData.Dialogues[m_itemOrder];
        }

        if (bodyTextMesh != null)
        {
            bodyTextMesh.text = dialogue.text;
        }

        if (stepTextMesh != null)
        {
            int totalItems = GetTutorialItems().Count;
            string stepStr = $"{m_itemOrder.ToString()} / {totalItems.ToString()}";
            stepTextMesh.text = stepStr;
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
