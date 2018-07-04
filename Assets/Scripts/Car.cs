using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//temporary/bad version of the very basic car movement.
public class Car : PlayerMode {

    public float speed;
    public float handlingMod;

    public Transform arrowTransform;
    public Transform messageTransform;

    public float arrowPositionOffset;
    public float messagePositionOffset;

	public AudioClip dyingSound;
	private AudioSource carSound;

    float MSG_DECAY_TIME = 4.0f;
	float msgDecayCounter;
    private Quaternion messageLockRotation;
    private Checkpoint targetPoint;

	bool dead;
	float RESPAWN_TIME = 2.0f;
	float deadCounter;

    private Camera playerCam;
	private Rigidbody2D rb;

	public GameObject blood;

    // Use this for initialization
    protected override void Start() {
        messageLockRotation = messageTransform.rotation;
		carSound = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update() {
        if(msgDecayCounter > 0) {
            msgDecayCounter -= Time.deltaTime;
        } else {
            SpriteRenderer msgSpriteRenderer = messageTransform.GetComponent<SpriteRenderer>();
            if(msgSpriteRenderer != null) {
                msgSpriteRenderer.enabled = false;
            }
        }

		if (dead) {
			if (deadCounter > 0) {
				deadCounter -= Time.deltaTime;
			} else {
				respawn ();
			}
		}
    }

    private void LateUpdate() {               

        if (targetPoint != null) {

            Vector3 targetScreenPos = playerCam.WorldToViewportPoint(targetPoint.transform.position); //get viewport position of checkpoint
            
            bool onscreenHorizontally = (targetScreenPos.x >= 0 && targetScreenPos.x <= 1);
            bool onscreenVertically = (targetScreenPos.y >= 0 && targetScreenPos.y <= 1);

            float xArrowOffset = 0f;
            float yArrowOffset = 0f;
            float xMessageOffset = 0f;
            float yMessageOffset = 0f;

            if (!onscreenHorizontally) {
                xArrowOffset = (targetPoint.transform.position.x > transform.position.x ? -1 : 1) * arrowPositionOffset;
                xMessageOffset = (targetPoint.transform.position.x > transform.position.x ? -1 : 1) * messagePositionOffset;
            }
            if (!onscreenVertically) {
                yArrowOffset = (targetPoint.transform.position.y > transform.position.y ? -1 : 1) * arrowPositionOffset;
                yMessageOffset = (targetPoint.transform.position.y > transform.position.y ? -1 : 1) * messagePositionOffset;
            }
            if(onscreenHorizontally && onscreenVertically) {
                arrowTransform.position = targetPoint.transform.position + new Vector3(xArrowOffset, yArrowOffset, 0f);
                messageTransform.position = targetPoint.transform.position + new Vector3(xMessageOffset, yMessageOffset, 0f);
            } else {
                Vector2 onScreenPos = new Vector2(targetScreenPos.x - 0.5f, targetScreenPos.y - 0.5f) * 2; //2D version, new mapping
                float max = Mathf.Max(Mathf.Abs(onScreenPos.x), Mathf.Abs(onScreenPos.y)); //get largest offset
                onScreenPos = (onScreenPos / (max * 2)) + new Vector2(0.5f, 0.5f); //undo mapping
                Vector3 onScreenVector = playerCam.ViewportToWorldPoint(onScreenPos);
                arrowTransform.position = playerCam.ViewportToWorldPoint(onScreenPos) + new Vector3(xArrowOffset, yArrowOffset, (onScreenVector.z*-1));
                messageTransform.position = playerCam.ViewportToWorldPoint(onScreenPos) + new Vector3(xMessageOffset, yMessageOffset, (onScreenVector.z * -1));
            }

            arrowTransform.rotation = Quaternion.LookRotation(Vector3.forward, targetPoint.transform.position - transform.position);
        }

        
        messageTransform.rotation = messageLockRotation;
    }

    public override void enter(Player affectedPlayer) {
        affectedPlayer.getRb2d().angularDrag = 2f;
        affectedPlayer.getRb2d().drag = 2f;
        affectedPlayer.getRb2d().mass = 2f;

        //capture camera from player
        playerCam = affectedPlayer.playerCam.getCam();
		rb = affectedPlayer.getRb2d ();

        //setup message timer stuff
        SpriteRenderer msgSpriteRenderer = messageTransform.GetComponent<SpriteRenderer>();
        if (msgSpriteRenderer != null) {
            msgSpriteRenderer.enabled = true;
        }
        msgDecayCounter = MSG_DECAY_TIME;

        //request target
        if (Racetrack.Instance != null) {
            targetPoint = Racetrack.Instance.getNearestCheckpoint(transform.position);
			targetPoint.setDisplayShow (getPlayerNum());
            Debug.Log("got a target!");
        }
    }

	public override void exit(Player affectedPlayer) {
		targetPoint.setDisplayHide (getPlayerNum ());
	}

    public override void mainAxisInput(float moveHorizontal, float moveVertical, Player affectedPlayer) {
		if (dead)
			return;
        //only turn if going forward or in reverse
        if(affectedPlayer.getRb2d().velocity.sqrMagnitude > float.Epsilon) {
			float moveForward = transform.InverseTransformDirection (affectedPlayer.getRb2d().velocity).y > 0 ? 1 : -1;
			affectedPlayer.transform.Rotate(Vector3.forward * moveHorizontal * handlingMod * -1 * moveForward * Time.deltaTime * 50);
			affectedPlayer.getRb2d().velocity = Quaternion.AngleAxis(moveHorizontal * handlingMod * -1 * moveForward * Time.deltaTime * 10, Vector3.forward) * affectedPlayer.getRb2d().velocity;
        }

        //Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        //affectedPlayer.getRb2d().AddForce(affectedPlayer.transform.up * speed * moveVertical);
    }

    public override void aButtonDown(Player affectedPlayer) {
		if (dead)
			return;
		affectedPlayer.getRb2d().AddForce(affectedPlayer.transform.up * speed);
		carSound.enabled = true;
		carSound.loop = true;
		//carSound.mute = false;

    }

    public override void bButtonDown(Player affectedPlayer) {
		if (dead)
			return;
		affectedPlayer.getRb2d().AddForce(affectedPlayer.transform.up * speed * -1);
		carSound.enabled = true;
		carSound.loop = true;
    }

	public override void aButtonUp(Player affectedPlayer) {
		if (dead)
			return;
		carSound.enabled = false;
		carSound.loop = false;
	}

	public override void bButtonUp(Player affectedPlayer) {
		if (dead)
			return;
		carSound.enabled = false;
		carSound.loop = false;
	}

    public void HitCheckpoint(Checkpoint hitCheckpoint) {
        if (hitCheckpoint.Equals(targetPoint)) {
            //update score
			int playerNum = getPlayerNum();
			targetPoint.setDisplayHide (playerNum);
            Debug.Log("attempt to give points to player with id "+playerNum);
			Points p = new Points ();
			p.player = playerNum;
			p.points = 1;
			EventManager.Instance.PostNotification (EventType.PlayerScored, null, p);
            //quest next checkpoint 
            targetPoint = Racetrack.Instance.getNextCheckpoint(targetPoint);
			targetPoint.setDisplayShow (playerNum);
        }
    }

	void OnCollisionEnter2D(Collision2D coll) {
		if (dead)
			return;
		if(rb != null && rb.velocity.magnitude > 2.5f)
			coll.gameObject.SendMessage("RunOver", getPlayerNum(),SendMessageOptions.DontRequireReceiver);
	}

	int getPlayerNum() {
		switch(tag) {
		case "Player1":
			return 1;
		case "Player2":
			return 2;
		case "Player3":
			return 3;
		case "Player4":
			return 4;
		default:
			return 0;
		}
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
		blood.SetActive (true);
		AudioSource.PlayClipAtPoint (dyingSound, Vector2.zero);
		dead = true;
		deadCounter = RESPAWN_TIME;
		GetComponent<Collider2D> ().enabled = false;
	}

	void respawn() {
		blood.SetActive (false);
		EventManager.Instance.PostNotification(EventType.RespawnPlayer,this,getTagNumber());
		dead = false;
		GetComponent<Collider2D> ().enabled = true;
	}

	int getTagNumber(){
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
		Debug.Log ("car Shot");
	}
}
