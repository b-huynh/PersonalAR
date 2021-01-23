using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadFileMenu : MonoBehaviour
{
    private string _searchPattern = "anchors?.txt";

    public UnityEngine.UI.GridLayoutGroup ButtonLayoutGroup;
    public UnityEngine.UI.Button ButtonTemplate;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RepopulateMenuOptions()
    {
        // Clear past entries if any
        foreach(Transform child in ButtonLayoutGroup.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Search persistantdatapath for existing save files. Order from most recent.
        string[] saveFiles = Directory.GetFiles(Application.persistentDataPath, _searchPattern);
        System.Array.Sort(saveFiles);
        System.Array.Reverse(saveFiles);

        // No files found
        if (saveFiles.Length <= 0)
        {
            var newButton = Object.Instantiate(ButtonTemplate, ButtonLayoutGroup.transform);
            newButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            newButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "No existing save files found.";
            newButton.gameObject.SetActive(true);
        }

        // Generate new buttons based on currently known save files
        foreach (string file in saveFiles)
        {
            var newButton = Object.Instantiate(ButtonTemplate, ButtonLayoutGroup.transform);
            newButton.GetComponentInChildren<UnityEngine.UI.Text>().text = file;
            newButton.gameObject.SetActive(true);
            newButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                Debug.LogFormat("Loading save file: {0}", file);
                // GameObject.FindObjectOfType<AnchorableObjectsManager>().LoadAnchors(file);
            });
        }
    }
}
