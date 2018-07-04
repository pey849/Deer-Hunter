using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

public class Racetrack : MonoBehaviour {

    //the public access for the instance of the racetrack
    public static Racetrack Instance {
        get { return instance; }
        set { }
    }

    public Checkpoint[] checkpoints;

    //cruddy background tiling stuff
    public GameObject[] grasstiles;

    public Transform topLeftCorner;
    public Transform bottomRightCorner;
    private Transform bgHolder;

    private float tileWidth = 2.491f;
    private float tileHeight = 1.431f;

    //singleton racetrack instance
    private static Racetrack instance = null;

    void Awake() {
        //if no instance of this exists, then assign this instance
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);            
        } else {
            DestroyImmediate(this);
        }
    }

    public Checkpoint getNearestCheckpoint(Vector3 callerPosition) {
        if (checkpoints.Length == 0) {
            return null;
        }

        int nearestIndex = 0;
        for(int i = 0; i < checkpoints.Length; i++) {
            float thisDistance = (checkpoints[i].transform.position - callerPosition).sqrMagnitude;
            float prevNearestDistance = (checkpoints[nearestIndex].transform.position - callerPosition).sqrMagnitude;
            if (thisDistance < prevNearestDistance) {
                nearestIndex = i;
            }
        }

        return checkpoints[nearestIndex];
    }

    public Checkpoint getNextCheckpoint(Checkpoint lastCheckpoint) {
        for(int i = 0; i < checkpoints.Length; i++) {
            if (checkpoints[i].Equals(lastCheckpoint)) {
                if (i == (checkpoints.Length - 1) ) {
                    return checkpoints[0];
                } else {
                    return checkpoints[i+1];
                }
            }
        }

        return null;
    }

    public void Dooms_day_thing() {
        Debug.Log("intiate the bad code");
        bgHolder = new GameObject("Board").transform;

        //void GrassSetup() {
        //loop and fill area between each corner
        Debug.Log("make stuff from x coord "+ topLeftCorner.position.x+" to "+ bottomRightCorner.position.x);
        for(float xPos = topLeftCorner.position.x; xPos <= bottomRightCorner.position.x; xPos += tileWidth) {
            Debug.Log("xpos loop: " + xPos);
            Debug.Log("make stuff from y coord " + topLeftCorner.position.y + " to " + bottomRightCorner.position.y);
            for(float yPos = topLeftCorner.position.y; yPos >= bottomRightCorner.position.y; yPos -= tileHeight) {
                Debug.Log("yPos loop: " + yPos);
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                GameObject toInstantiate = grasstiles[Random.Range(0, grasstiles.Length)];
                //Instantiate the GameObject
                GameObject instance =
                    Instantiate(toInstantiate, new Vector3(xPos, yPos, 0f), Quaternion.identity) as GameObject;
                //throw into top-left cornerfor the sake of haivng a container
                //instance.transform.SetParent(bgHolder);
            }
        }
    }


}
