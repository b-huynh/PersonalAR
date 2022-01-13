using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public RandomPinCodes codeSet;
    [SerializeField] private TMPro.TextMeshPro scoreText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = $"Score: {codeSet.CodesCompleted}";
    }
}