﻿using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

[System.Serializable]
public class PlayerData : SaveData{
	public PlayerData () : base () {}
	public PlayerData (SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
}

public class PlayerScript : SavableScript {
	public PlayerData playerdata;
	
	// Input variables
	private Vector2 moveInput;
	private Vector2 lookInput;
	private bool interact;
	private bool cancel;

	public float speed = 10;
	public Vector3 movement;
	public Vector3 look;

	private Animator animator;
	public Transform sprite;

	public enum State{Moving, Interacting};
	public State currentState = State.Moving;

	// Use this for initialization
	void Start () {
		animator = sprite.GetComponent<Animator>();

		savedata = playerdata;
	}
	
	void Update () {
		if (GlobalScript.currentGameState == GlobalScript.GameState.InGame)
			InGame ();
	}

	void InGame () {
		playerdata = (PlayerData)savedata;

		//TODO ensure globalscript isnt paused for all scripts

		GetInput ();

		// Only do movement updates if in movement playerstate
		switch (currentState) {
		case State.Moving:
			movement = new Vector3 (speed * moveInput.x, 0, speed * moveInput.y);
			movement = Vector3.ClampMagnitude (movement, speed);

			look = new Vector3 (lookInput.x, 0, lookInput.y);
			
			if (interact) {
				InteractorScript interactor = GetComponent<InteractorScript> ();
				if (interactor != null) {
					interactor.Interact ();
				}
			}
			// Up = 2
			// Down = 0
			// Left = 1
			// Right = 3

			bool idle = false;
			if (moveInput.x > 0)
				animator.SetInteger ("Direction", 3);
			else if (moveInput.x < 0)
				animator.SetInteger ("Direction", 1);
			else if (moveInput.y > 0)
				animator.SetInteger ("Direction", 2);
			else if (moveInput.y < 0)
				animator.SetInteger ("Direction", 0);
			else
				idle = true;

			animator.SetBool ("Idle", idle);
			break;
		case State.Interacting:
			break;
		}
	}

	void GetInput(){
		interact = GlobalScript.GetButton(GlobalScript.Interact);
		cancel = GlobalScript.GetButton(GlobalScript.Cancel);
		moveInput = GlobalScript.GetAxis(GlobalScript.LeftStick);
		lookInput = GlobalScript.GetAxis (GlobalScript.RightStick);
	}
	
	void FixedUpdate()
	{
		// 5 - Move the game object
		GetComponent<Rigidbody>().velocity = movement;

		Vector3 actualDirectionOfMotion = movement.normalized;

		float angle;
		if (look.magnitude > 0) {
			angle = Vector3.Angle (look, Vector3.forward) * Mathf.Sign (look.x);
			this.transform.rotation = Quaternion.Euler (90, angle, 0);
		} else if (actualDirectionOfMotion.magnitude > 0) {
			angle = Vector3.Angle (actualDirectionOfMotion, Vector3.forward) * Mathf.Sign (actualDirectionOfMotion.x);
			this.transform.rotation = Quaternion.Euler (90, angle, 0);
		}

	}

	void OnDestroy()
	{
		// Game Over.
		// Add the script to the parent because the current game
		// object is likely going to be destroyed immediately.
		// transform.parent.gameObject.AddComponent<GameOverScript>();
	}

	public void ChangeState(State state){
		currentState = state;
		switch(currentState){
		case State.Moving:
			break;
		case State.Interacting:
			break;
		}
	}
}
