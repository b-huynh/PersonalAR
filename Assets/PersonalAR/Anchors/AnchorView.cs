using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Anchorable))]
[RequireComponent(typeof(AppEntity))]
public class AnchorView : MonoBehaviour
{
    public GameObject label;
    public TextMesh text;

    void OnEnable()
    {
        GetComponent<Anchorable>().OnAnchorSet += OnAnchorSet;
    }

    void OnDisable()
    {
        GetComponent<Anchorable>().OnAnchorSet -= OnAnchorSet;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAnchorSet(AnchorableObject anchor)
    {
        transform.parent = anchor.transform;
        transform.localPosition = Vector3.zero;

        text.text = anchor.WorldAnchorName;
        label.SetActive(false);
    }
}
