using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public int player;

	float lifeSpan = 0.5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (lifeSpan > 0)
			lifeSpan -= Time.deltaTime;
		else
			Destroy (gameObject);
	}

	void OnCollisionEnter2D(Collision2D coll) {
		coll.gameObject.SendMessage("Shot", player,SendMessageOptions.DontRequireReceiver);

		Destroy (gameObject);
	}
}
