using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorActor : MonoBehaviour
{
    public string anchorName;
    public TextMesh tagMesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tagMesh.text = anchorName;
    }

    public void Delete()
    {
        AnchorableObjectsManager.Instance.DeleteAnchor(anchorName);
        // GameObject.Destroy(this.gameObject);
    }
}
