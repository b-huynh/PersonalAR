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

    [SerializeField] private TextMeshPro bodyTextMesh;
    [TextArea(5, 8)] public string bodyText;

    [SerializeField] private TextMeshPro stepTextMesh;

    public void OnValidate()
    {
        if (bodyTextMesh != null)
        {
            bodyTextMesh.text = bodyText;
        }

        if (stepTextMesh != null)
        {
            int totalItems = GetTutorialItems().Count;
            string stepStr = $"{m_itemOrder.ToString()} / {totalItems.ToString()}";
            stepTextMesh.text = stepStr;
        }
    }

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
