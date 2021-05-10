using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboveCanvasConstraint : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector3 offset;

    void OnValidate() => UpdatePosition();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }
    
    void UpdatePosition()
    {
        Vector3 newPos = canvas.transform.position;
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
        Vector3 canvasScale = canvas.GetComponent<RectTransform>().localScale;
        float height = canvasSize.y * canvasScale.y;
        newPos = newPos + new Vector3(0, height/2.0f + offset.y, 0);
        transform.position = newPos;
    }
}
