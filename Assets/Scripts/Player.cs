using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Mode {
        HUNTER, WEREWOLF, RACECAR, DEER, GOLDEN_STAG
}

public class Player : MonoBehaviour {
    
    public CameraControl playerCam;

    public PlayerMode hunterMode;
    public PlayerMode wolfMode;
    public PlayerMode carMode;
    public PlayerMode activeMode; //let me know if this approach is trash. -Adam
	public Mode currentModeEnum;
    public int testModeTrack = 0;
	public Text transformationText;
	public GameObject textCanvas;

	float transformationTextTimer;

	public int playerNum;

	bool aWasPressed;
	bool bWasPressed;

    private Rigidbody2D rb2d; //Store a reference to the Rigidbody2D component required to use 2D Physics.
    


    // Use this for initialization
    void Start () {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();
        setMode(Mode.HUNTER);
    }

    public void setMode(Mode newMode) {
        //NOTE: this code has no consideration for transitions between certain modes.... yet

		transformationTextTimer = 3.5f;

        hunterMode.gameObject.SetActive(false);
        wolfMode.gameObject.SetActive(false);
        carMode.gameObject.SetActive(false);

		if(activeMode != null)
			activeMode.exit (this);
		
        switch (newMode) {
            case Mode.HUNTER:
                playerCam.setHunter();
                activeMode = hunterMode;
				currentModeEnum = Mode.HUNTER;
				transformationText.text = "Kill Deer!";
            break;
            case Mode.WEREWOLF:
                playerCam.setWerewolf();
                activeMode = wolfMode;
				currentModeEnum = Mode.WEREWOLF;
				transformationText.text = "Kill Players!";
            break;
            case Mode.RACECAR:
                playerCam.setCar();
                activeMode = carMode;
				currentModeEnum = Mode.RACECAR;
				transformationText.text = "Get Checkpoints!";
            break;
        }

		textCanvas.SetActive (true);
        activeMode.enter(this);
        activeMode.gameObject.SetActive(true);
    }

    public Rigidbody2D getRb2d() {
        return this.rb2d;
    }

    private void Update() {
		if (Time.timeScale == 0) //game is paused
			return;

		if (transformationTextTimer > 0)
			transformationTextTimer -= Time.deltaTime;
		else {
			textCanvas.SetActive (false);
		}



        //CRAP SWITCH MODE TEST - Do not use
		/*if (Input.GetButtonDown("Fire2")) {
			if (playerNum != 1)
				return;
			switch (this.testModeTrack) {
			case 0:
				testModeTrack = 1;
				setMode (Mode.WEREWOLF);
				break;
			case 1:
				testModeTrack = 2;
				setMode (Mode.RACECAR);
				break;
			case 2:
				testModeTrack = 0;
				setMode (Mode.HUNTER);
				break;
			}
		}*/

		if (getBButton()) {
			activeMode.bButtonDown(this);
		} else if (bWasPressed) {
			activeMode.bButtonUp (this);
		}

		if(getAButton()) {
            activeMode.aButtonDown(this);
		} else if( aWasPressed) {
            activeMode.aButtonUp(this);
        }

		aWasPressed = getAButton();
		bWasPressed = getBButton();

		float moveHorizontal;
		float moveVertical;
		if (playerNum == 0) {
			moveHorizontal = Input.GetAxis ("Horizontal");
			moveVertical = Input.GetAxis("Vertical");
		} else {
			moveHorizontal = Input.GetAxis ("P" + playerNum + "X2");
			moveVertical = -Input.GetAxis("P" + playerNum + "Y2");
		}
		activeMode.secondaryAxisInput(moveHorizontal, moveVertical, this);
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate() {                       
        //Store the current horizontal input in the float moveHorizontal.
		float moveHorizontal;
		float moveVertical;
		if (playerNum == 0) {
			moveHorizontal = Input.GetAxis ("Horizontal");
			moveVertical = Input.GetAxis("Vertical");
		} else {
			moveHorizontal = Input.GetAxis ("P" + playerNum + "X");
			moveVertical = -Input.GetAxis("P" + playerNum + "Y");
		}

        activeMode.mainAxisInput(moveHorizontal, moveVertical, this);

    }

	bool getAButton() {
		switch (playerNum) {
		case 1:
			return Input.GetKey (KeyCode.Joystick1Button0) ||
				Input.GetKey (KeyCode.Joystick1Button5);
			break;
		case 2:
			return Input.GetKey (KeyCode.Joystick2Button0) ||
				Input.GetKey (KeyCode.Joystick2Button5);
			break;
		case 3:
			return Input.GetKey (KeyCode.Joystick3Button0) ||
				Input.GetKey (KeyCode.Joystick3Button5);
			break;
		case 4:
			return Input.GetKey (KeyCode.Joystick4Button0) ||
				Input.GetKey (KeyCode.Joystick4Button5);
			break;
		}

		return Input.GetButton("Fire1");
	}

	bool getBButton() {
		switch (playerNum) {
		case 1:
			return Input.GetKey (KeyCode.Joystick1Button1) ||
				Input.GetKey (KeyCode.Joystick1Button4);
			break;
		case 2:
			return Input.GetKey (KeyCode.Joystick2Button1) ||
				Input.GetKey (KeyCode.Joystick2Button4);
			break;
		case 3:
			return Input.GetKey (KeyCode.Joystick3Button1) ||
				Input.GetKey (KeyCode.Joystick3Button4);
			break;
		case 4:
			return Input.GetKey (KeyCode.Joystick4Button1) ||
				Input.GetKey (KeyCode.Joystick4Button4);
			break;
		}

		return Input.GetButton("Fire2");
	}

	public void Shot(int player) {
		Debug.Log ("shot");
		activeMode.Shot (player);
	}

	public void mauled(int playerThatScored){
		activeMode.Shot (playerThatScored);
	}

	public void RunOver(int player) {
		activeMode.RunOver (player);
	}
}
