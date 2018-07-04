using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_Attack : MonoBehaviour {

    private bool aButtonDown = false;
	public int player;

	public bool disabled;

    public void setIsAButtonDown(bool aButtonDown) {
        this.aButtonDown = aButtonDown;
    }

    public bool getIsAButtonDown() {
        return this.aButtonDown;
    }

	public void Disable() {
		disabled = true;
	}

	public void Enabled() {
		disabled = false;
	}

    void OnTriggerEnter2D(Collider2D col){
		if (aButtonDown && !disabled) {
            //Destroy(col.gameObject);
			col.gameObject.SendMessage("mauled", player, SendMessageOptions.DontRequireReceiver);
        }
	}

	void OnTriggerStay2D(Collider2D col){
		if (aButtonDown && !disabled) {
			//Destroy (col.gameObject);
			col.gameObject.SendMessage("mauled", player, SendMessageOptions.DontRequireReceiver);
		}
	}
}
