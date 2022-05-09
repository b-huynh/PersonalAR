using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;

public class ButtonStatusController : MonoBehaviour
{
    // Button visuals and behaviour references
    [SerializeField] private TextMeshPro buttonTextMesh;
    [SerializeField] private Color textDisabledColor;
    private Color textOriginalColor;
    [SerializeField] private Renderer iconRenderer;
    [SerializeField] private Material iconDisabledMaterial;
    private Material iconOriginalMaterial;
    [SerializeField] private Transform compressableButtonVisuals;

    private List<MonoBehaviour> buttonBehaviours;
    private bool statusInitialized = false;

    void OnEnable()
    {
        if (!statusInitialized)
        {
            // Determine initial status
            textOriginalColor = buttonTextMesh.color;
            iconOriginalMaterial = iconRenderer.material;
            buttonBehaviours = GetComponents<MonoBehaviour>().ToList();

            statusInitialized = true;

            Debug.Log("Tutorial Enabled");
            Debug.Log($"Bevhaiours count: {buttonBehaviours.Count()}");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStatus(bool active)
    {
        foreach(var behaviour in buttonBehaviours.Where(b => b != this))
        {
            behaviour.enabled = active;
        }
        compressableButtonVisuals.gameObject.SetActive(active);
        buttonTextMesh.color = active ? textOriginalColor : textDisabledColor;
        iconRenderer.material = active ? iconOriginalMaterial : iconDisabledMaterial;
    }
}
