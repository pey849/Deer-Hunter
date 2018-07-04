using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerSpawner : MonoBehaviour {
	public GameObject deerPrefab;

	public Transform topLeft;
	public Transform bottomRight;

	float coolDown = 0.5f;
	float current;
	// Use this for initialization
	void Start () {
		current = coolDown;
	}
	
	// Update is called once per frame
	void Update () {
		current -= Time.deltaTime;
		if(current <= 0) {
			GameObject g = Instantiate (deerPrefab);
			g.transform.position = new Vector2(Random.Range(topLeft.position.x,bottomRight.position.x),Random.Range(bottomRight.position.y,topLeft.position.y));
			current = coolDown;
		}
	}
}
