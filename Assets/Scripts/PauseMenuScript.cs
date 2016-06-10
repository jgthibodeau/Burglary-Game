﻿using UnityEngine;
using System.Collections;

public class PauseMenuScript : MenuScript {
	protected override void Start(){
		base.Start ();
//		Close ();
	}

	// Update is called once per frame
	protected override void Update(){
		if (GlobalScript.GetButton ("Pause")){
			if (GlobalScript.currentGameState == GlobalScript.GameState.InGame) {
				Open ();
			} else
				Close ();
		}
		
		base.Update ();
	}

	public override void Open(){
		GlobalScript.currentGameState = GlobalScript.GameState.Paused;
		Time.timeScale = 0f;

		base.Open ();
	}
	public override void Close(){
		base.Close ();

		GlobalScript.currentGameState = GlobalScript.GameState.InGame;
		Time.timeScale = 1f;
	}
}
