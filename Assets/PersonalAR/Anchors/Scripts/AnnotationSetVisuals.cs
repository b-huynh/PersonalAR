using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

public class AnnotationSetVisuals : MonoBehaviour
{
    // public List<string> choices;

    [SerializeField] private AnnotationSet _annotationSet;
    [SerializeField] private TextMeshProUGUI _setNameTextMesh;
    [SerializeField] private GameObject _buttonTemplate;

    private IAnchorService _anchorService;
    private Dictionary<string, Button> _buttons;

    void OnValidate()
    {
        if (_annotationSet != null && _setNameTextMesh != null)
        {
            _setNameTextMesh.text = _annotationSet.SetName;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!MixedRealityServiceRegistry.TryGetService<IAnchorService>(out _anchorService))
        {
            ARDebug.LogError($"Failed to get AnchorService");
        }
        _anchorService.OnRegistered += OnRegisteredHandler;
        _anchorService.OnRemoved += OnRemovedHandler;

        if (_buttonTemplate == null)
        {
            ARDebug.LogError($"{gameObject.name}: _buttonTemplate variable not set");
        }

        RepopulateButtons();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnRegisteredHandler(AnchorableObject anchor) => ForceRefreshInteractableVisuals();
    public void OnRemovedHandler(string anchorName) => ForceRefreshInteractableVisuals();
    public void ForceRefreshInteractableVisuals()
    {
        foreach(var kv in _buttons)
        {
            // Set button to no longer be interactable if it's been stored in anchor manager
            // Should use anchor service events instead
            if (_anchorService.ContainsAnchor(kv.Key) ||
                _anchorService.ContainsAnchor(kv.Key.ToLower()))
            {
                kv.Value.interactable = false;
            }
            else
            {
                kv.Value.interactable = true;
            }
        }
    }

    public void RepopulateButtons()
    {
        // Hide button template since we only use it for cloning
        _buttonTemplate.SetActive(false);

        // Delete existing children that are buttons, create new children buttons
        DeleteExistingButtons();
        CreateNewButtons(_buttonTemplate, true);

        // Set internal button map (used to update interactable status)
        _buttons = FindButtons();
        BindButtonsToAnchorPlacement(_buttons);
    }

    private void DeleteExistingButtons()
    {
        List<GameObject> existingButtons = new List<GameObject>();
        foreach(Transform child in transform)
        {
            if (child.name.StartsWith("gen_"))
            {
                existingButtons.Add(child.gameObject);
            }
        }

        for(int i = 0; i < existingButtons.Count; ++i)
        {
            DestroyImmediate(existingButtons.ElementAt(i));
        }
    }

    private void CreateNewButtons(GameObject buttonTemplate, bool activeState = true)
    {
        // Populate choice buttons
        List<string> choices = new List<string>(_annotationSet.AnnotationNames);
        choices.Sort();

        foreach(string choiceName in choices)
        {
            var newButton = Object.Instantiate(buttonTemplate, transform);
            newButton.name = $"gen_{choiceName}_button";
            newButton.GetComponentInChildren<UnityEngine.UI.Text>().text = choiceName;
            newButton.SetActive(activeState);
        }
    }

    private Dictionary<string, Button> FindButtons()
    {
        Dictionary<string, Button> buttons = new Dictionary<string, Button>();
        foreach(Transform child in transform)
        {
            if (child.name.StartsWith("gen_"))
            {
                string choiceName = child.gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text;
                var buttonComponent = child.gameObject.GetComponent<UnityEngine.UI.Button>();
                buttons.Add(choiceName, buttonComponent);
            }
        }
        return buttons;
    }

    private void BindButtonsToAnchorPlacement(Dictionary<string, Button> buttons)
    {
        AnchorPlacement anchorPlacement = GameObject.FindObjectOfType<AnchorPlacement>();
        foreach(var kv in buttons)
        {
            // TODO: Be consistent with Lower and Uppercase anchor names...
            kv.Value.onClick.AddListener(
                delegate
                {
                    anchorPlacement.SetNextObject(kv.Key.ToLower());
                }
            );
        }
    }
}
