using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;

using Recaug;

public class GlobalSelectListener : MonoBehaviour, IMixedRealityInputActionHandler
{

    public GameObject stickyNotePrefab;

    public GameObject notesApp;

    List<string> fakeObjects = new List<string>() {
        "chair",
        "tv",
        "keyboard",
        "mouse",
        "sofa",
        "dining table",
        "potted plant"
    };

    List<string> fakeNotes = new List<string>() {
        "TODO: Send in the laptop for repair... :(",
        "It's your turn to do the dishes!!! \n - Brandon",
        "Water basil plant \n - Brandon",
        "Take a note...",
        "Take a note..."
    };

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputActionHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnActionStarted(BaseInputEventData eventData)
    {
        var action = eventData.MixedRealityInputAction.Description;
        if (action == "Select")
        {
            if (eventData.InputSource.SourceType == InputSourceType.Hand)
            {
                if (eventData.InputSource.Pointers.Length == 0) return;

                Vector3 hitposition = eventData.InputSource.Pointers[0].BaseCursor.Position;
                GameObject target = eventData.InputSource.Pointers[0].Result.CurrentPointerTarget;

                hitposition += (Camera.main.transform.position - hitposition).normalized * 0.1f;

                if (target && target.layer == LayerMask.NameToLayer("Spatial Awareness"))
                {
                    if (GameManager.Instance.currAppID == 0) // Home
                    {
                        if (fakeObjects.Count > 0)
                        {
                            string nextFake = fakeObjects[0];
                            fakeObjects.RemoveAt(0); 
                            ObjectRegistry.Instance.Register(nextFake, hitposition);
                        }
                    }
                    else if (GameManager.Instance.currAppID == 3) // Remind
                    {
                        if (fakeNotes.Count > 0)
                        {
                            string nextNote = fakeNotes[0];
                            fakeNotes.RemoveAt(0);

                            GameObject note = GameObject.Instantiate(stickyNotePrefab);
                            notesApp.GetComponent<App>().Link(note);

                            note.transform.position = hitposition;
                            note.transform.LookAt(Camera.main.transform.position);
                            note.transform.Find("Slate/ContentQuad/Notepad").GetComponent<TextMeshPro>().text = nextNote;
                        }
                    }

                }
            }
        }
    }

    public void OnActionEnded(BaseInputEventData eventData)
    {
        // Debug.Log(eventData.MixedRealityInputAction.Description);
    }
}
