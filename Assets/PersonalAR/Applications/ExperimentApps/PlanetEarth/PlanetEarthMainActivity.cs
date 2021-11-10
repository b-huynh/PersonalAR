using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetEarthMainActivity : BaseAppActivity
{
    [Range(0.1f, 3.0f)]
    public float launchDistance;

    [SerializeField] private GameObject entityToLaunch;
    protected GameObject cachedEntity;

    void Reset()
    {
        launchDistance = 1.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        Destroy(cachedEntity);
    }

    public override void StartActivity(ExecutionContext executionContext)
    {
        // Calculate a launch point set distance away from users current forward position.
        Vector3 forwardDelta = Camera.main.transform.forward * launchDistance;
        Vector3 launchPoint = Camera.main.transform.position + forwardDelta;
        launchPoint.y = Mathf.Max(0.35f, launchPoint.y);

        cachedEntity = GameObject.Instantiate(entityToLaunch, this.transform);
        cachedEntity.transform.SetPositionAndRotation(launchPoint, Quaternion.identity);
    }
    public override void StopActivity(ExecutionContext executionContext)
    {
        if (cachedEntity != null) 
        {
            Destroy(cachedEntity);
            cachedEntity = null;
        }
    }
}
