using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIHandler : MonoBehaviour, IGameListener {

	public GameObject InGameDisplay;
	public GameObject EndGameDisplay;
	public Timer worldTimer;

	public Selectable buttonStart;

	//the game manager 
	GameManagement gameManager;
	//text fields for dislpay score for individual players
	public Text player1, player2, player3, player4;
	public Text timeRemaining;

	public Text[] playersOutcome; 

	// Use this for initialization
	void Start () {
		//grab the instance of the GameManagement
		gameManager = GameObject.FindObjectOfType<GameManagement>();
		worldTimer = gameManager.GetComponentInChildren<Timer> ();
		//subscribe itself to listen for end of game event
		EventManager.Instance.AddGameListener(EventType.EndGame, this);
		//subscribe itself to listen for update score events
		EventManager.Instance.AddGameListener(EventType.UpdateScores, this);

		//toggle which display gets shown
		InGameDisplay.SetActive (true);
		EndGameDisplay.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (worldTimer.timeRemaining > 0) {
			timeRemaining.text = "Time Remaining:\n" + ((int)worldTimer.timeRemaining).ToString ();
		} else {
			timeRemaining.text = "Time Remaining:\n0";
		}
	}

	//interface method to implement from IGameListener
	public void OnEvent( EventType eventType, Component sender, object param ){
		//handle the appriopriate events
		switch(eventType) {
		case EventType.UpdateScores:
			UpdateScores ();
			break;
		case EventType.EndGame:
			EndGame ();
			break;
		default:
			//do nothing
			break;
		}
	}

	//update the canvas labels appropriately
	void UpdateScores(){
		player1.text = "Player 1: "+gameManager.playerScores [0].ToString();
		player2.text = "Player 2: "+gameManager.playerScores [1].ToString();
		player3.text = "Player 3: "+gameManager.playerScores [2].ToString();
		player4.text = "Player 4: "+gameManager.playerScores [3].ToString();
	}

	public void EndGame(){

		InGameDisplay.SetActive (false);
		EndGameDisplay.SetActive (true);
		buttonStart.Select ();
		determineWinner ();
	}

	public void determineWinner(){

		LinkedList<int> listPlayerPosition = new LinkedList<int> ();
		LinkedList<int> listPlayerPositionScore = new LinkedList<int> ();

		listPlayerPosition.AddLast (0);
		listPlayerPositionScore.AddLast (gameManager.playerScores [0]);
		int winningScore = gameManager.playerScores [0];

		for (int i = 1; i < gameManager.playerScores.Length; i++) {
			if (winningScore < gameManager.playerScores [i]) {
				listPlayerPosition.AddFirst (i);
				listPlayerPositionScore.AddFirst (gameManager.playerScores [i]);
				winningScore = gameManager.playerScores [i];
			} else {
				listPlayerPosition.AddLast (i);
				listPlayerPositionScore.AddLast (gameManager.playerScores [i]);
			}
		}

		int counter = 0;
		foreach (int i in listPlayerPosition) {
			if (counter == 0) {
				playersOutcome [counter].text = "Winner\nPlayer " + i.ToString ();
			} else {
				playersOutcome [counter].text = "Player " + i.ToString ();
			}
			counter++;
		}

		counter = 0;
		foreach (int i in listPlayerPositionScore) {
			playersOutcome [counter].text += ": " + i.ToString();
			counter++;
		}
			
	}

	public void On_Click_PlayAgain() {
		EventManager.Instance.PostNotification(EventType.StartGame, this, null);
	}

	public void On_Click_MainMenu() {
		SceneManager.LoadScene ("MainMenuScene");
	}
}
