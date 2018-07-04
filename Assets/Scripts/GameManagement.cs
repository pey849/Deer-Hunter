using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour, IGameListener {

	#region variables
	//the players in the game
	public Player[] players;

	//the scores for the players
	public int[] playerScores;

	//the spawnpoints for the players
	public GameObject[] spawnPoints;	

	//has the first full moon come?
	bool hasFirstMoonCome = false;

	//has the second full moon come?
	bool hasSecondMoonCome = false;
	#endregion

	#region monobehaviours
	// Use this for initialization
	void Start () {
		//Add self as a listener to relevant game events
		EventManager.Instance.AddGameListener(EventType.StartGame, this);
		EventManager.Instance.AddGameListener(EventType.EndGame, this);
		EventManager.Instance.AddGameListener(EventType.FullMoon, this);
		EventManager.Instance.AddGameListener(EventType.PlayerScored, this);
		EventManager.Instance.AddGameListener(EventType.RespawnPlayer, this);
		
		playerScores = new int[4];
		for(int i = 0; i < 4; i++) {
			playerScores[i] = 0;
		}
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	#endregion

	#region event functions
	//implementation of OnEvent from IGameListener interface
	public void OnEvent( EventType eventType, Component sender, object param) {
		//handle the appriopriate events
		switch(eventType) {
		case EventType.StartGame:
			StartGame();
			break;
		case EventType.EndGame:
			EndGame();
			break;
		case EventType.FullMoon:
			FullMoon();
			break;
		case EventType.PlayerScored:
			PlayerScored((Points)param);
			break;
		case EventType.RespawnPlayer:
			RespawnPlayer((int)param);
			break;
		default:
			//do nothing
			break;
		}
	}

	//called upon the StartGame event
	void StartGame() {
		//load the game scene
		SceneManager.LoadScene("CoolScene", LoadSceneMode.Single);
	}

	//called upon the EndGame event
	void EndGame() {
		//choose the winning player
		//int winningPlayerID = 0;
	}

	//called upon the FullMoon event
	void FullMoon() {
		//pick player(s) and tell them to change mode to hunter/werewolf/car
		//if the first full moon hasn't come force player to change into a werewolf
		int numPlayersToChange = Random.Range(1, 4);
		if(!hasFirstMoonCome) {
			hasFirstMoonCome = true;

			if(numPlayersToChange == 4){
				for(int k = 0; k < players.Length; k++){
					players[k].setMode(Mode.WEREWOLF);
				}
			}
			else {
				//transform the players
				for(int i = 0; i < numPlayersToChange; i++) {
					int playerToChange = Random.Range(0, 3);
					
					while(players[playerToChange].currentModeEnum == Mode.WEREWOLF) {
						playerToChange++;
						if(playerToChange == 4)
							playerToChange = 0;
					}
					players[playerToChange].setMode(Mode.WEREWOLF);
				}
			}
			
		}
		//if the second hasn't come force player to change into a car
		else if(!hasSecondMoonCome) {
			hasSecondMoonCome = true;

			if(numPlayersToChange == 4){
				for(int k = 0; k < players.Length; k++){
					players[k].setMode(Mode.RACECAR);
				}
			}
			else {
				//transform the players
				for(int i = 0; i < numPlayersToChange; i++) {
					int playerToChange = Random.Range(0, 3);
					while(players[playerToChange].currentModeEnum == Mode.RACECAR) {
						playerToChange++;
						if(playerToChange == 4)
							playerToChange = 0;
					}
					players[playerToChange].setMode(Mode.RACECAR);
				}
			}		
		}
		else {
			numPlayersToChange = 4;
			//transform the players
			for(int i = 0; i < numPlayersToChange; i++) {
				Mode newMode = GetRandomMode();
				while(players[i].currentModeEnum == newMode) {
					newMode = GetRandomMode();
				}
				players[i].setMode(newMode);
			}
		}
		
	}

	void PlayerScored(Points playerID) {
		playerScores[playerID.player - 1] += playerID.points;
		EventManager.Instance.PostNotification(EventType.UpdateScores, this, playerScores[playerID.player - 1]);
	}

	void RespawnPlayer(int playerNumber) {
		int spawnNumber = Random.Range(0, spawnPoints.Length - 1);
		players[playerNumber - 1].transform.position = spawnPoints[spawnNumber].transform.position;
	}
	#endregion

	Mode GetRandomMode() {
		int modeNum = Random.Range(0, 5);
		if(modeNum == 0) {
			return Mode.HUNTER;
		}
		else if(modeNum == 1) {
			return Mode.WEREWOLF;
		}
		else {
			return Mode.RACECAR;
		}
	}

	//wait for the scene to be loaded, then setup the game object references and such
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		//grab the scene name and check if it is the main game scene
		string sceneName = scene.name;
		
		if(sceneName == "CoolScene") {
			//grab all the players
			players = new Player[4];
			PlayerHolder playerHolder = GameObject.FindGameObjectWithTag("PlayerHolder").GetComponent<PlayerHolder>();
			players[0] = playerHolder.players[0];
			players[1] = playerHolder.players[1];
			players[2] = playerHolder.players[2];
			players[3] = playerHolder.players[3];
	
			//grab all the spawn points
			spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		
			//reset the scores
			for(int i = 0; i < playerScores.Length; i++) {
				playerScores[i] = 0;
			}

			//reset the bools
			hasFirstMoonCome = false;
			hasSecondMoonCome = false;
		}

        if(Racetrack.Instance != null) {
            Racetrack.Instance.Dooms_day_thing();
        }
    }
}
