using UnityEngine;
using System.Collections;

public class GameManagerBase : MonoBehaviour {
	protected void doWin(){
		MapUnlockSystem.wasLastGameCompleted = true;
	}
}
