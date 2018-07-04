using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void On_Click_Start() {
		EventManager.Instance.PostNotification(EventType.StartGame, this, null);
	}

	public void On_Click_End() {
		//if(Application.isEditor) {
		//	UnityEditor.EditorApplication.isPlaying = false;
		//}
		//else {
			Application.Quit();
		//}
	}
}
