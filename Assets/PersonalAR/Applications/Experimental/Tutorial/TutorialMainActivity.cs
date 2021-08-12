﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TutorialMainActivity : BaseAppActivity
{
    private List<TutorialItem> tutorialItems;
    private TutorialItem currentItem;

    // Start is called before the first frame update
    void Start()
    {
        tutorialItems = TutorialItem.GetTutorialItems();
        foreach(TutorialItem item in tutorialItems)
        {
            item.OnTweenInComplete.AddListener(delegate
            {
                AudioSource audio = item.gameObject.GetComponent<AudioSource>();
                audio.clip = item.dialogue.audioClip;
                audio.Play();
            });

            item.OnTweenOutComplete.AddListener(delegate
            {
                AudioSource audio = item.gameObject.GetComponent<AudioSource>();
                audio.Stop();
            });
        }
    }

    // Update is called once per frame
    void Update() {}

    public override void StartActivity(ExecutionContext executionContext)
    {
        tutorialItems = TutorialItem.GetTutorialItems();
        currentItem = tutorialItems[0];
        currentItem.Open();
    }

    public override void StopActivity(ExecutionContext executionContext)
    {
        tutorialItems = TutorialItem.GetTutorialItems();
        foreach(var tutorialItem in tutorialItems)
        {
            tutorialItem.Close();
            currentItem = null;
        }
    }

    public void NextItem()
    {
        // Close current step
        currentItem.Close();
        tutorialItems.Remove(currentItem);

        // Start next step
        if (tutorialItems.Count > 0)
        {
            currentItem = tutorialItems[0];
            if (currentItem.CloseCondition != null)
            {
                currentItem.CloseCondition.OnValueChanged += CloseConditionHandler;
            }
            currentItem.Open();
        }
    }

    public void CloseConditionHandler(bool newValue)
    {
        if (newValue == true)
        { 
            currentItem.CloseCondition.OnValueChanged -= CloseConditionHandler;
            NextItem();
        }
    }

    // public void OnItemOpen()
    // {
    //     AudioSource audio = currentItem.gameObject.GetComponent<AudioSource>();
    //     audio.clip = currentItem.dialogue.audioClip;
    //     audio.Play();
    // }

    // public void OnItemClose()
    // {
    //     AudioSource audio = currentItem.gameObject.GetComponent<AudioSource>();
    //     audio.Stop();
    // }
}
