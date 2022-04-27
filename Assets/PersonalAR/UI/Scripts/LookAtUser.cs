using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtUser : MonoBehaviour
{
	public Vector3 forwardDirection = Vector3.forward;
	public Vector3 upDirection = Vector3.up;
	public float offsetTowardsUserAmount;

	[ReadOnly] public float signedAngleAxisForward;
	[ReadOnly] public float signedAngleAxisUp;
	[ReadOnly] public float signedAngleCameraUpAroundForward;

	private Vector3 originalPosition;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Calculate position
		Vector3 cameraDirection = (Camera.main.transform.position - originalPosition).normalized;
		Vector3 offset = cameraDirection * offsetTowardsUserAmount;
		transform.position = originalPosition + offset;

		// Calculate rotation
		if (forwardDirection != Vector3.forward && upDirection != Vector3.up)
		{
			signedAngleAxisUp = Vector3.SignedAngle(cameraDirection, forwardDirection, upDirection);

			signedAngleCameraUpAroundForward = Vector3.SignedAngle(Camera.main.transform.up, upDirection, forwardDirection);

			signedAngleAxisForward = Vector3.SignedAngle(cameraDirection, upDirection, forwardDirection);

			Vector3 currentUpDir = upDirection;
			Vector3 currentForwardDir = forwardDirection;

			// Determine up direction based on if camera is above or below the line
			if (Mathf.Abs(signedAngleCameraUpAroundForward) > 90)
			{
				currentUpDir = -upDirection;
			}

			if (currentForwardDir != Vector3.zero && currentUpDir != Vector3.zero)
			{
				transform.rotation = Quaternion.LookRotation(currentForwardDir, currentUpDir);
			}

			if (Mathf.Abs(signedAngleAxisUp) < 90)
			{
				transform.RotateAround(transform.position, transform.up, 180);
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

	// Use this to set position if using offsets
	public void SetOriginalPosition(Vector3 position)
	{
		originalPosition = position;
		transform.position = position;
	}
}
