using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
	Camera cam;
	float sizeStart;
	float sizeTarget;

	float WEREWOLF_SIZE = 9.0f;
	float HUNTER_SIZE = 8.0f;
	float CAR_SIZE = 13.0f;

	float STEP_SIZE = 0.02f;

	float currentStep;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
		sizeTarget = cam.orthographicSize;
		sizeStart = cam.orthographicSize;
		currentStep = 1;
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler (Vector3.up);
		if (cam.orthographicSize != sizeTarget) {
			currentStep += STEP_SIZE;
			cam.orthographicSize = Mathf.Lerp (sizeStart, sizeTarget, currentStep);
		}
	}

	public void setWerewolf() {
		currentStep = 0;
		sizeStart = cam.orthographicSize;
		sizeTarget = WEREWOLF_SIZE;
	}

	public void setHunter() {
		if(cam == null)
			cam = GetComponent<Camera> ();
		currentStep = 0;
		sizeStart = cam.orthographicSize;
		sizeTarget = HUNTER_SIZE;
	}

	public void setCar() {
		currentStep = 0;
		sizeStart = cam.orthographicSize;
		sizeTarget = CAR_SIZE;
	}

    public Camera getCam() {
        return this.cam;
    }
}
