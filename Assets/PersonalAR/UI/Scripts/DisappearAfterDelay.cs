using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearAfterDelay : MonoBehaviour
{
    // In seconds;
    float timePassed;
    float delay;

    // Start is called before the first frame update
    void Start()
    {
        timePassed = 0;
        if (delay <= 0) 
        {
            delay = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > delay)
        {
            Destroy(gameObject);
        }
    }

    public void SetDelay(float timeSeconds)
    {
        timePassed = 0;
        delay = timeSeconds;
    }
}
