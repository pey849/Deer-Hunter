using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
	public GameObject menu;
	public Selectable continueButton;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.JoystickButton7)) {
			Debug.Log ("pause");
			if (Time.timeScale == 0)
				unpause ();
			else
				pause ();
		}

		if (Input.GetKeyDown (KeyCode.JoystickButton1) && Time.timeScale == 0) {
			unpause ();
		}
	}

	void pause() {
		menu.SetActive (true);
		Time.timeScale = 0;
		continueButton.Select ();
	}

	public void unpause() {
		menu.SetActive (false);
		Time.timeScale = 1;
	}

	public void Quit() {
		SceneManager.LoadScene ("MainMenuScene");
	}
}
