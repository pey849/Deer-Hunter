using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : PlayerMode {

	//the movement speed of the wolf
	public float movementSpeed;

	//Store a reference to the Rigidbody2D component required to use 2D Physics.
	public Rigidbody2D rb2d;
    Animator wolfAnim;

	public AnimationClip attack;
	public AudioClip attackSound;
	public AudioClip dyingSound;

    public Wolf_Attack attackRadius;

    //print debug messages
    bool debugMessagesMovement = false;
	//print debug messages
	bool debugMessagesAttack = true;

	bool dead;
	float RESPAWN_TIME = 2.0f;
	float deadCounter;

	float ATTACK_ACTIVE_TIME;
	float attackCountdown;

	bool ToSetCoolDown;

	float ATTACK_COOLDOWN_TIME = 0.5f;
	float attackCooldown;
		

	int MAX_HEALTH = 5;
	public float health;
	float STUN_COOLDOWN_TIME = 5f;
	public float stunCooldown;

	public GameObject blood;

	// Use this for initialization
	void Start () {
        wolfAnim = GetComponent<Animator>();
		ATTACK_ACTIVE_TIME = attack.length;
		health = MAX_HEALTH;
    }

	void onAwake(){
		wolfAnim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		//get the horizontal and vertical movement
		//		float moveVertical = Input.GetAxis("Vertical");
		//		float moveHorizontal = Input.GetAxis("Horizontal");
		//
		//		//set the animation states
		//		wolfAnim.SetFloat ("HorizontalSpeed", moveHorizontal);
		//		wolfAnim.SetFloat ("VerticalSpeed", moveVertical);
		//
		//		//move the wolf
		//		moveWolf (moveHorizontal, moveVertical);
		if (stunCooldown > 0) {
			stunCooldown -= Time.deltaTime;
		} else if(health <= 0) {
			health = MAX_HEALTH;
		}

		if (attackCountdown > 0) {
			attackCountdown -= Time.deltaTime;
		}

		if (attackCountdown <= 0 && ToSetCoolDown) {
			attackCooldown = ATTACK_COOLDOWN_TIME;
			ToSetCoolDown = false;
		}

		if (attackCooldown <= 0)
			attackRadius.Enabled ();
		else {
			attackRadius.Disable ();
			attackCooldown -= Time.deltaTime;
		}

		if (dead) {
			if (deadCounter > 0) {
				deadCounter -= Time.deltaTime;
			} else {
				respawn ();
			}
		}
	}

    //@Peter: change these values to ones you want. -Adam
    public override void enter(Player affectedPlayer) {
        affectedPlayer.getRb2d().angularDrag = 2f;
        affectedPlayer.getRb2d().drag = 2f;
        affectedPlayer.getRb2d().mass = 1f;
		health = MAX_HEALTH;
    }

	public override void exit(Player affectedPlayer) {
	}

    public override void mainAxisInput(float moveHorizontal, float moveVertical, Player affectedPlayer) {
		if (dead || stunCooldown > 0)
			return;
		wolfAnim.SetFloat ("HorizontalSpeed", moveHorizontal);
		wolfAnim.SetFloat ("VerticalSpeed", moveVertical);
		moveWolf (moveHorizontal, moveVertical, affectedPlayer.getRb2d());
		Aim (moveHorizontal, moveVertical, affectedPlayer.getRb2d ());
	}

	void moveWolf(float x, float y, Rigidbody2D rb2d){
		//Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
		float upDown = movementSpeed * y;
		float leftRight = movementSpeed * x;
		rb2d.velocity = new Vector2 (leftRight, upDown);
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

	void changeDirection(float x, float y) {
		if (dead || stunCooldown > 0)
			return;
		float epsilon = 0.01f;
		if (Mathf.Abs(x) > epsilon || Mathf.Abs(y) > epsilon) {
			//lock the vertical for now
			Vector2 dir = new Vector2 (x, 0);
			dir.Normalize ();
			transform.rotation = Quaternion.LookRotation (Vector3.forward, dir);
		}
	}

    public override void aButtonDown(Player affectedPlayer) {
		if (dead  || stunCooldown > 0 || !attackRadius.enabled || attackCountdown > 0 || attackCooldown > 0)
			return;

        attackRadius.setIsAButtonDown(true);
		attackRadius.player = getWolfTagNumber();
		wolfAnim.SetTrigger ("WolfAttack");
		attackCountdown = ATTACK_ACTIVE_TIME;
		AudioSource.PlayClipAtPoint (attackSound, Vector2.zero);
		ToSetCoolDown = true;
    }

    public override void aButtonUp(Player affectedPlayer) {
        attackRadius.setIsAButtonDown(false);
		if (wolfAnim == null) {
			wolfAnim = GetComponent<Animator> ();
		}
		//wolfAnim.SetBool ("WolfAttack",false);
    }

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
		wolfAnim.SetTrigger ("Dead");
		blood.SetActive (true);
		AudioSource.PlayClipAtPoint (dyingSound, Vector2.zero);
		dead = true;
		deadCounter = RESPAWN_TIME;
		GetComponent<Collider2D> ().enabled = false;
		//wolfAnim.SetBool ("WolfAttack",false);
		attackRadius.setIsAButtonDown (false);
	}

	void respawn() {
		wolfAnim.SetTrigger ("Respawn");
		blood.SetActive (false);
		EventManager.Instance.PostNotification(EventType.RespawnPlayer,this,getWolfTagNumber());
		dead = false;
		GetComponent<Collider2D> ().enabled = true;
	}

	public int getWolfTagNumber(){
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

	public override void Shot(int player) {
		health--;
		if (health <= 0) {
			stunCooldown = STUN_COOLDOWN_TIME;
		}
	}

	public override void RunOver(int player) {
		health--;
		if (health <= 0) {
			stunCooldown = STUN_COOLDOWN_TIME;
		}
	}
}
