using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtUser : MonoBehaviour
{
	public Vector3 forwardDirection = Vector3.forward;
	public Vector3 upDirection = Vector3.up;

	public float signedAngleAxisForward;
	public float signedAngleAxisUp;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (forwardDirection != Vector3.forward && upDirection != Vector3.up)
		{
			Vector3 cameraDirection = Camera.main.transform.position - transform.position;

			signedAngleAxisUp = Vector3.SignedAngle(cameraDirection, forwardDirection, upDirection);
			signedAngleAxisForward = Vector3.SignedAngle(cameraDirection, upDirection, forwardDirection);

			if (Vector3.Angle(cameraDirection, forwardDirection) > 90)
			{
				transform.rotation = Quaternion.LookRotation(forwardDirection, upDirection);
			}
			else
			{
				transform.rotation = Quaternion.LookRotation(-forwardDirection, upDirection);
			}
		}
		else if (upDirection != Vector3.up)
		{
			transform.LookAt(Camera.main.transform, upDirection);
		}
		else 
		{
			transform.LookAt(Camera.main.transform);
		}
	}
}
