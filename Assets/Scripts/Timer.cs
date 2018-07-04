using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour, IGameListener {

	//Time to begin countdown from in seconds.
    public float timeRemaining;

	//the max and min amount of time before a full moon happens used for random time range
	public float fullMoonMaxTime;
	public float fullMoonMinTime;

	//time until the next full moon happens
	public float fullMoonTimeRemaining;
	public AudioClip endGameSound;
	public AudioClip fullMoonSound;

	//has the game ended?
	bool hasGameEnded;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddGameListener(EventType.StartGame, this);
		fullMoonTimeRemaining = Random.Range(fullMoonMinTime, fullMoonMaxTime);
		hasGameEnded = true;
	}
	
	// Update is called once per frame
	void Update () {
		//only use timer if game hasn't ended
		if(!hasGameEnded) {
			//if the time is up, end the game
			if(timeRemaining <= 0f) {
	            EventManager.Instance.PostNotification(EventType.EndGame, this, null);
				hasGameEnded = true;
				AudioSource.PlayClipAtPoint (endGameSound, Vector2.zero);
	        }
			//if in the last minute of the game, change the time ranges for a full moon
			else if(timeRemaining <= 60f) {
				fullMoonMaxTime = 20f;
				fullMoonMinTime = 10f;
			}
	
			//Determine if a full moon needs to happen
			if(fullMoonTimeRemaining <= 0f) {
				//fire the full moon event
				EventManager.Instance.PostNotification(EventType.FullMoon, this, null);
				//reset the full moon time remaining
				fullMoonTimeRemaining = Random.Range(fullMoonMinTime, fullMoonMaxTime);
				AudioSource.PlayClipAtPoint (fullMoonSound, Vector2.zero);
			}
			//else just decrement the remaining time until a full moon happens
			else {
				fullMoonTimeRemaining -= Time.deltaTime;
			}
	
	        //Update time
	        timeRemaining -= Time.deltaTime;
		}
	}

	//implementation of OnEvent from IGameListener interface
	public void OnEvent( EventType eventType, Component sender, object param) {
		//handle the appriopriate events
		switch(eventType) {
		case EventType.StartGame:
			ResetTimer();
			break;
		default:
			//do nothing
			break;
		}
	}

	public void ResetTimer() {
		hasGameEnded = false;
		timeRemaining = 240f;
		fullMoonMaxTime = 30f;
		fullMoonMinTime = 20f;
		fullMoonTimeRemaining = Random.Range(fullMoonMinTime, fullMoonMaxTime);
	}
}
