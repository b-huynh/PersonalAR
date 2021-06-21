using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(Anchorable))]
public class SmartInfoMenu : BaseEntity
{
    [SerializeField] private TextMeshPro itemNameTM;
    [SerializeField] private TextMeshPro serialNumTM;

    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Anchorable>().OnAnchorSet.AddListener(AnchorSetHandler);
        GetComponent<Anchorable>().OnAnchorRemoved.AddListener(AnchorRemovedHandler);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AnchorSetHandler(AnchorableObject anchor)
    {
        itemNameTM.text = anchor.WorldAnchorName;
    }

    private void AnchorRemovedHandler(AnchorableObject anchor)
    {
        itemNameTM.text = string.Empty;
    }

    public void SetSerialNumber(string serialNum)
    {
        serialNumTM.text = serialNum;
    }
}
