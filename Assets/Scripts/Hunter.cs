using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : PlayerMode {

	public GameObject bulletPrefab;
	public Transform gunBarrel;
	float BULLET_VELOCITY = 80f;

	float SHOOT_DELAY = 0.5f;
	float shootCountdown;

	bool dead;
	float RESPAWN_TIME = 2.0f;
	float deadCounter;

	public float speed = 5f;

	public AudioClip shootingSound;
	public AudioClip dyingSound;

	public Animator anim;

	public GameObject blood;

	void Start() {
		anim = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {
		if(shootCountdown > 0) shootCountdown -= Time.deltaTime;

		if (dead) {
			if (deadCounter > 0) {
				deadCounter -= Time.deltaTime;
			} else {
				respawn ();
			}
		}
	}

	void FixedUpdate() {
		//float moveHorizontal = Input.GetAxis("Horizontal");
		//float moveVertical = Input.GetAxis("Vertical");
		//Move (moveHorizontal, moveVertical);

		//float aimHorizontal = Input.GetAxis("Horizontal");
		//float aimVertical = Input.GetAxis("Vertical");
		//Aim (aimHorizontal, aimVertical);
	}

    public override void enter(Player affectedPlayer) {
        affectedPlayer.getRb2d().angularDrag = 2f;
        affectedPlayer.getRb2d().drag = 2f;
        affectedPlayer.getRb2d().mass = 1f;
    }

	public override void exit(Player affectedPlayer) {
	}

    public override void mainAxisInput(float moveHorizontal, float moveVertical, Player affectedPlayer) {
		if (dead)
			return;
		Move (moveHorizontal, moveVertical, affectedPlayer.getRb2d());
	}

	public override void secondaryAxisInput(float moveHorizontal, float moveVertical, Player affectedPlayer) {
		if (dead)
			return;
		Aim (moveHorizontal, moveVertical, affectedPlayer.getRb2d());
	}

	public override void aButtonDown(Player affectedPlayer) {
		if (dead)
			return;
		Shoot(affectedPlayer.getRb2d());
	}

	public override void bButtonDown(Player affectedPlayer) {
		if (dead)
			return;
		Shoot(affectedPlayer.getRb2d());
	}

	void Move(float x, float y, Rigidbody2D rb2d) {
		//Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
		float upDown = speed * y;
		float leftRight = speed * x;
		rb2d.velocity = new Vector2 (leftRight, upDown);
	}

	//mauled by a wolf
	public override void mauled(int playerThatScored){
		if (dead)
			return;
		died ();
		Points p = new Points ();
		p.player = playerThatScored;
		p.points = 5;
		EventManager.Instance.PostNotification (EventType.PlayerScored, null, p);
	}

	public void died(){
		if (dead)
			return;
		blood.SetActive (true);
		AudioSource.PlayClipAtPoint (dyingSound, Vector2.zero);
		anim.SetTrigger("Dead");
		dead = true;
		deadCounter = RESPAWN_TIME;
		GetComponent<Collider2D> ().enabled = false;
	}

	void respawn() {
		anim.SetTrigger ("Respawn");
		blood.SetActive (false);
		EventManager.Instance.PostNotification(EventType.RespawnPlayer,this,getHunterTagNumber());
		dead = false;
		GetComponent<Collider2D> ().enabled = true;
	}

	void Aim(float x, float y, Rigidbody2D rb2d) {
		float epsilon = 0.01f;
		if (Mathf.Abs(x) > epsilon || Mathf.Abs(y) > epsilon) {
			Vector2 dir = new Vector2 (x, y);
			dir.Normalize ();
			transform.rotation = Quaternion.LookRotation (Vector3.forward, dir);
			rb2d.angularVelocity = 0;
		}
	}

	int getHunterTagNumber(){

		int playerNum = 0;
		switch (tag) {
		case "Player1":
			playerNum = 1;
			break;
		case "Player2":
			playerNum = 2;
			break;
		case "Player3":
			playerNum = 3;
			break;
		case "Player4":
			playerNum = 4;
			break;
		}
		return playerNum;
	}

	void Shoot(Rigidbody2D rb2d) {
		if (shootCountdown <= 0) {
			AudioSource.PlayClipAtPoint (shootingSound, Vector2.zero);
			anim.SetTrigger("Shoot");
			shootCountdown = SHOOT_DELAY;

//			int playerNum = 0;
//			switch (tag) {
//			case "Player1":
//				playerNum = 1;
//				break;
//			case "Player2":
//				playerNum = 2;
//				break;
//			case "Player3":
//				playerNum = 3;
//				break;
//			case "Player4":
//				playerNum = 4;
//				break;
//			}

			GameObject bullet = Instantiate (bulletPrefab);
			bullet.GetComponent<Bullet> ().player = getHunterTagNumber();
			bullet.layer = gameObject.layer;
			bullet.transform.position = gunBarrel.transform.position;
			bullet.transform.rotation = transform.rotation;
			bullet.GetComponent<Rigidbody2D> ().velocity = (Vector2)(transform.up * BULLET_VELOCITY) + rb2d.velocity;
		}
	}

	public override void Shot(int player) {
	}

	public override void RunOver(int player) {
		if (dead)
			return;
		died ();
	}
}
