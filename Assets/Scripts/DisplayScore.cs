using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScore : MonoBehaviour, IGameListener {

	//the game manager holds the points for all players
	GameManagement PointsHolder;
	//text fields for dislpay score for individual players
	public Text player1, player2, player3, player4;


	// Use this for initialization
	void Start () {
		//subscribe itself to listen for update score events
		EventManager.Instance.AddGameListener(EventType.UpdateScores, this);
		//grab the instance of the GameManagement
		PointsHolder = GameObject.FindObjectOfType<GameManagement>();
	}

	//interface method to implement from IGameListener
	public void OnEvent( EventType eventType, Component sender, object param ){
		//handle the appriopriate events
		switch(eventType) {
		case EventType.UpdateScores:
			UpdateScores ();
			break;
		default:
			//do nothing
			break;
		}
	}

	//update the canvas labels appropriately
	void UpdateScores(){
		player1.text = "Player 1: "+PointsHolder.playerScores [0].ToString();
		player2.text = "Player 2: "+PointsHolder.playerScores [1].ToString();
		player3.text = "Player 3: "+PointsHolder.playerScores [2].ToString();
		player4.text = "Player 4: "+PointsHolder.playerScores [3].ToString();
	}
}
