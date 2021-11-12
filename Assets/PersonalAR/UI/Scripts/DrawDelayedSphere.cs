using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;

public class DrawDelayedSphere : MonoBehaviour
{
    [Range(0.1f, 20f)]
    public float delaySeconds;

    public GameObject delayedSpherePrefab;
    public GameObject lineRendererPrefab;

    private GameObject lastSphereDrawn;
    private Gradient lineColorGradient;

    // Start is called before the first frame update
    void Start()
    {
        lineColorGradient = CreateLineColorGradient();
    }

    // Update is called once per frame
    void Update()
    {
        if (delayedSpherePrefab != null)
        {
            var delayedSphere = GameObject.Instantiate(delayedSpherePrefab);
            Vector3 towardsUserOffset = (Camera.main.transform.position - transform.position).normalized * 0.01f;
            delayedSphere.transform.SetPositionAndRotation(transform.position + towardsUserOffset, transform.rotation);
            delayedSphere.GetComponent<DisappearAfterDelay>().SetDelay(delaySeconds);

            if (lastSphereDrawn != null)
            {
                // Draw a line connecting this sphere with last sphere.
                var lineRenderer = CreateLineObject(lastSphereDrawn.transform.position, delayedSphere.transform.position);
                // lineRenderer.GetComponent<MixedRealityLineRenderer>().LineColor = lineColorGradient;
                lineRenderer.GetComponent<MixedRealityLineRenderer>().WidthMultiplier = 0.02f;
                lineRenderer.AddComponent<DisappearAfterDelay>().SetDelay(delaySeconds);
            }

            lastSphereDrawn = delayedSphere;
        }
    }

    private GameObject CreateLineObject(Vector3 start, Vector3 end)
    {
        GameObject newLine = GameObject.Instantiate(lineRendererPrefab);

        newLine.transform.position = start;
        Vector3 localEnd = newLine.transform.InverseTransformPoint(end);

        newLine.GetComponent<SimpleLineDataProvider>().EndPoint = new MixedRealityPose(localEnd);

        newLine.SetActive(true);

        return newLine;
    }

    private Gradient CreateLineColorGradient()
    {
        Gradient gradient = new Gradient();

        var colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.green;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.red;
        colorKey[1].time = 1.0f;

        var alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);
        return gradient;
    }

}
