using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class RecipeInfographicVisuals : MonoBehaviour
{
    [SerializeField] private Recipe recipe;
    [SerializeField] private TextMeshPro infoText;
    [SerializeField] private TextMeshPro infoSubtext;

    // Start is called before the first frame update
    void Start()
    {
        infoText.text = $"0 / 0";
        infoSubtext.text = "Steps";
    }

    // Update is called once per frame
    void Update()
    {
        if (recipe != null)
        {
            infoText.text = $"{recipe.CurrentIndex + 1} / {recipe.Steps.Count}";
        }   
    }
}
