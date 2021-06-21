using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LoadingRing : MonoBehaviour
{
    // Assigned at runtime
    private AnchorableObject anchor;
    private bool isTracking;

    // Editor-controlled display values
    public float rotationSpeed = 200.0f;
    public Sprite notSavedSprite;
    public Sprite loadingSprite;
    public Sprite savedSprite;

    // Playback Control
    private bool isPlaying;
    public void ShowNotSaved()
    {
        m_image.sprite = notSavedSprite;
        m_image.color = Color.red;
        m_image.rectTransform.localRotation = Quaternion.identity;
    }
    public void ShowLoading()
    {
        m_image.sprite = loadingSprite;
        m_image.color = Color.white;
        m_image.rectTransform.localRotation = Quaternion.identity;
        isPlaying = true;
    }
    public void ShowSaved()
    {
        m_image.sprite = savedSprite;
        m_image.color = Color.green;
        m_image.rectTransform.localRotation = Quaternion.identity;
        isPlaying = false;
    }
    public void Toggle()
    {
        if (m_image.sprite == notSavedSprite)
        {
            ShowLoading();
        }
        else if (m_image.sprite == loadingSprite)
        {
            ShowSaved();
        }
        else
        {
            ShowNotSaved();
        }
    }

    private Image m_image;

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();

        anchor = GetComponentInParent<AnchorableObject>();
        isTracking = anchor.IsTrackingPosition;
        if (isTracking)
        {
            ShowSaved();
        }
        else
        {
            ShowLoading();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // React to chang in tracking position status.
        if (anchor.IsTrackingPosition != isTracking)
        {
            isTracking = anchor.IsTrackingPosition;
            if (isTracking)
            {
                ShowSaved();
            }
            else
            {
                ShowLoading();
            }
        }

        // Play rotating animation (for loading only)
        if (isPlaying)
        {
            m_image.rectTransform.Rotate(0.0f, 0.0f, Time.deltaTime * rotationSpeed);
        }
    }
}
