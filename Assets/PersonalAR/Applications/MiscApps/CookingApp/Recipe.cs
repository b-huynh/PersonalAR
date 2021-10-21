using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CookingApp/Recipe")]
public class Recipe : ScriptableObject
{
    [SerializeField] public List<RecipeStep> Steps = new List<RecipeStep>();
    [NonSerialized] public int CurrentIndex = 0;
    public RecipeStep CurrentStep
    {
        get => CurrentIndex < Steps.Count ? Steps[CurrentIndex] : null;
        private set {}
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {

    }

    public void Next()
    {
        if (CurrentIndex < Steps.Count)
        {
            CurrentIndex++;
        }
    }

    public void Reset()
    {
        CurrentIndex = 0;
    }
}
