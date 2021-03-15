﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;

using TMPro;

[RequireComponent(typeof(Anchorable))]
public class AnchorView : MonoBehaviour
{
    public GameObject label;
    public GameObject buttons;
    public TextMeshPro textMeshPro;

    void OnEnable()
    {
        GetComponent<Anchorable>().OnAnchorSet += OnAnchorSet;

        var interactable = GetComponent<Interactable>();
        interactable.OnClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        GetComponent<Anchorable>().OnAnchorSet -= OnAnchorSet;

        var interactable = GetComponent<Interactable>();
        interactable.OnClick.RemoveListener(OnClick);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAnchorSet(AnchorableObject anchor)
    {
        transform.parent = anchor.transform;
        transform.localPosition = Vector3.zero;

        textMeshPro.text = anchor.WorldAnchorName;

        label.SetActive(false);
    }

    void OnClick()
    {
        ScaleTween tween = buttons.GetComponent<ScaleTween>();

        if (tween.IsTweenedIn)
            tween.TweenOut();
        else
            tween.TweenIn();

        // bool toggleNext = !buttons.activeSelf;
        // buttons.SetActive(toggleNext);

        // if (toggleNext == true)
        //     buttons.GetComponent<ScaleTween>().TweenIn();
        // else
        //     buttons.GetComponent<ScaleTween>().TweenOut();
    }
}
