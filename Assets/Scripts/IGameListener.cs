using System.Collections;
using UnityEngine;

//the enum that defines all the game events
public enum EventType
{
	StartGame,
    EndGame,
	FullMoon,
	PlayerScored,
	UpdateScores,
	RespawnPlayer,
};

//interface for the objects implementing the game events
public interface IGameListener
{
    //the function called when the event fires
    void OnEvent( EventType eventType, Component sender, object param = null );
}
