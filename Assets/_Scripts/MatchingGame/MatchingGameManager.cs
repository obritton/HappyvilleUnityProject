﻿using UnityEngine;
using System.Collections;

public class MatchingGameManager : GameManagerBase {

	public GameObject[] cardFRONTArr;

	// Use this for initialization
	void Start () {
		DoorManager.openDoors ();
	}

	void doWin(){
		base.doWin ();
	}
}
