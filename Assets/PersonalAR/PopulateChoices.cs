using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateChoices : MonoBehaviour
{
    public List<string> choices;
    public GameObject optionTemplate;

    private Dictionary<string, GameObject> buttons;

    private AnchorableObjectsManager anchorManager;

    // Start is called before the first frame update
    void Start()
    {
        if (optionTemplate == null)
        {
            Debug.LogErrorFormat("{0}: optionTemplate variable not set", gameObject.name);
        }

        if (choices.Count <= 0)
        {
            Debug.LogErrorFormat("{0}: choices not populated", gameObject.name);
        }

        // Populate choice buttons
        buttons = new Dictionary<string, GameObject>();
        foreach(string choiceName in choices)
        {
            var newButton = Object.Instantiate(optionTemplate, transform);
            newButton.name = choiceName + "ButtonChoice";
            newButton.GetComponentInChildren<UnityEngine.UI.Text>().text = choiceName;
            newButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                ClickHandler(choiceName);
            });
            buttons.Add(choiceName, newButton);
        }

        // Hide initial button since we only use it as template for cloning
        optionTemplate.SetActive(false);

        // Set reference to anchor manager so we can check when objects are placed
        anchorManager = GameObject.FindObjectOfType<AnchorableObjectsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var kv in buttons)
        {
            // Set button to no longer be interactable if it's been stored in anchor manager  
            if(anchorManager.anchoredObjects.ContainsKey(kv.Key))
            {
                kv.Value.GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
        }
    }

        void ClickHandler(string name)
    {
        GameObject.FindObjectOfType<AnchorableObjectsManager>().SetNextObject(name);
    }
}
