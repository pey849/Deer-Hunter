using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMode : MonoBehaviour {

    //overridable init. will we have any sort of common setup?
    protected virtual void Start() {

    }

    //if you hate this and can improve upon it, please do. -Adam

    public abstract void enter(Player affectedPlayer);

	public abstract void exit(Player affectedPlayer);

    public abstract void mainAxisInput(float moveHorizontal, float moveVertical, Player affectedPlayer);

    public virtual void secondaryAxisInput(float moveHorizontal, float moveVertical, Player affectedPlayer) {

    }

    public virtual void aButtonDown(Player affectedPlayer) {

    }
    public virtual void aButtonUp(Player affectedPlayer) {

    }

    public virtual void bButtonDown(Player affectedPlayer) {

    }
    public virtual void bButtonUp(Player affectedPlayer) {

    }

	public virtual void Shot(int player) {

	}

	public virtual void mauled(int playerThatScored){

	}

	public virtual void RunOver(int player) {

	}

}
