using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTester : MonoBehaviour {

	public int state = 0;
	public float countdown = 3f;

	public CameraControl cc;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		countdown -= Time.deltaTime;
		if (countdown <= 0) {
			if (state == 0) {
				state++;
				cc.setHunter ();
			} else if (state == 1) {
				state++;
				cc.setCar ();
			} else {
				state = 0;
				cc.setWerewolf ();
			}
			countdown = 3f;
		}
	}
}
