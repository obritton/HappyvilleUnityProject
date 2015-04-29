using UnityEngine;
using System.Collections;

public class PhotoboothManager : GameManagerBase {

	// Use this for initialization
	void Start () {
		DoorManager.openDoors ();
	}

	void doWin(){
		base.doWin ();
	}
}
