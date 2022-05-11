using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public RandomPinCodes codeSet;
    [SerializeField] private TMPro.TextMeshPro scoreText;

    private ExperimentManager experimentManagerRef;

    // Start is called before the first frame update
    void Start()
    {
        experimentManagerRef = ExperimentManager.Instance;   
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = $"Score: {experimentManagerRef.Score}";
    }
}
