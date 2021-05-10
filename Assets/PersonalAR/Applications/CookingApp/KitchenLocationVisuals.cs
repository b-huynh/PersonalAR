using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class KitchenLocationVisuals : MonoBehaviour
{
    [Header("Runtime Control")]
    public Recipe recipe;
    public KitchenLocation locationType;

    [Header("Visual State Control")]
    [SerializeField] private TextMeshPro titleText;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image panel;
    [SerializeField] private Color attentionColor;
    [SerializeField] private Color defaultColor;

    // Private state variables
    private Vector2 originalSize;

    void OnValidate()
    {
        if (locationText != null)
        {
            locationText.text = locationType.ToString();
        }

        UpdateVisuals();
    }

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        originalSize = rect.sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (instructionText != null)
        {
            if (recipe?.CurrentStep?.LocationType == locationType)
            {
                instructionText.text = recipe.CurrentStep.Instruction;
                panel.color = attentionColor;
            }
            else
            {
                // instructionText.text = 
                //     $"See [<color=\"red\">{recipe?.CurrentStep?.LocationType.ToString()}</color>]";
                instructionText.text = string.Empty;
                panel.color = defaultColor;
            }
        }

        if (titleText != null)
        {
            RectTransform rect = canvas.GetComponent<RectTransform>();
            titleText.text = $"Width: {rect.sizeDelta.x}, Height: {rect.sizeDelta.y}";
        }
    }

    public void AddWidth(int delta)
    {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x + delta, rect.sizeDelta.y);
    }

    public void AddHeight(int delta)
    {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + delta);
    }

    public void ResetSize()
    {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        rect.sizeDelta = originalSize;
    }
}
