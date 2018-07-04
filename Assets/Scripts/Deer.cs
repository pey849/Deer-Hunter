using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : MonoBehaviour {

	bool dead;
	float DEATH_TIME = 2.0f;
	float deadCounter;

	Vector2 travelTo;
	float MAX_DISTANCE = 2f;
	float PAUSE_MAX = 4f;
	float currentPause;

	float MAX_MOVE = 0.03f;

	float WALK_TIMEOUT = 5f;
	float walkCountdown;

	float RUN_SPEED = 0.1f;
	float SPOOKED_TIMEOUT = 2f;
	float spookCountdown;
	Vector2 spookPoint;

	public AudioClip dyingSound;
	private AudioSource deerSounds;

	delegate void Action();
	Action current;

	public GameObject blood;

	public Animator anim;
	// Use this for initialization
	void Start () {
		setWaiting ();
		//anim.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeScale == 0)
			return;

		if (!dead) {
			current ();
		} else {
			if (deadCounter > 0)
				deadCounter -= Time.deltaTime;
			else
				Destroy (gameObject);
		}
	}

	void Walk() {
		if (transform.position.x != travelTo.x && transform.position.y != travelTo.y && walkCountdown > 0) {
			Vector2 moveDirection = (Vector2)travelTo - (Vector2)gameObject.transform.position;
			walkCountdown -= Time.deltaTime;
			transform.position = Vector2.MoveTowards (transform.position, travelTo, MAX_MOVE);

			if ((Vector2)moveDirection != (Vector2)Vector2.zero) 
			{
				float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
		} else {
			setWaiting ();
		}
	}

	void Wait() {
		if (currentPause > 0) {
			currentPause -= Time.deltaTime;
		} else {
			setWalking ();
		}
	}

	void Spooked ()
	{
		if (spookCountdown > 0) {
			anim.SetBool ("Startle",true);
			Vector2 moveDirection = (Vector2)((Vector2)transform.position - spookPoint);
			moveDirection.Normalize ();

			transform.position += (Vector3)(moveDirection * RUN_SPEED) * (Time.deltaTime * 100);

			if ((Vector2)moveDirection != (Vector2)Vector2.zero) {
				float angle = Mathf.Atan2 (moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
			}

			spookCountdown -= Time.deltaTime;
		} else {
			anim.SetBool ("Startle",false);
			setWaiting ();
		}
	}

	void setWaiting() {
		current = Wait;
		currentPause = Random.Range (0, PAUSE_MAX);
	}

	void setWalking() {
		current = Walk;
		walkCountdown = WALK_TIMEOUT;
		travelTo = getTravelToLocation ();
	}

	Vector2 getTravelToLocation() {
		return new Vector2(transform.position.x + Random.Range(-MAX_DISTANCE,MAX_DISTANCE), transform.position.y + Random.Range(-MAX_DISTANCE,MAX_DISTANCE));
	}

	void setSpooked() {
		current = Spooked;
		spookCountdown = SPOOKED_TIMEOUT;
	}

	void Shot(int player) {
		Die ();
		Points p = new Points ();
		p.player = player;
		p.points = 1;
		EventManager.Instance.PostNotification (EventType.PlayerScored, null, p);
	}

	void RunOver(int player) {
		Die ();
	}

	void mauled(int player) {
		Die ();
	}

	void Die() {
		anim.SetTrigger ("Dead");
		blood.SetActive (true);
		AudioSource.PlayClipAtPoint (dyingSound, Vector2.zero);
		dead = true;
		deadCounter = DEATH_TIME;
		GetComponent<Rigidbody2D> ().Sleep ();
		GetComponent<Collider2D> ().enabled = false;
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player1" || other.tag == "Player2" || other.tag == "Player3" || other.tag == "Player4")
			setSpooked ();
	}
}
