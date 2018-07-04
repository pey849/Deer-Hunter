using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    
	public GameObject player1;
	public GameObject player2;
	public GameObject player3;
	public GameObject player4;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player1" || other.tag == "Player2" || other.tag == "Player3" || other.tag == "Player4") {
            other.gameObject.SendMessage("HitCheckpoint", this);
        }
    }

	public void setDisplayHide(int player) {
		switch (player) {
		case 1:
			player1.SetActive (false);
			break;
		case 2:
			player1.SetActive (false);
			break;
		case 3:
			player1.SetActive (false);
			break;
		case 4:
			player1.SetActive (false);
			break;
		}
	}

	public void setDisplayShow(int player) {
		switch (player) {
		case 1:
			player1.SetActive (true);
			break;
		case 2:
			player1.SetActive (true);
			break;
		case 3:
			player1.SetActive (true);
			break;
		case 4:
			player1.SetActive (true);
			break;
		}
	}

}
