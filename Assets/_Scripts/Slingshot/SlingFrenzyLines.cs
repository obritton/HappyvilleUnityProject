using UnityEngine;
using System.Collections;

public class SlingFrenzyLines : MonoBehaviour {

	public GameObject[] floatingCharacters;

	void Start(){
		int time = 3;
		foreach (GameObject character in floatingCharacters) {
			iTween.MoveBy(character, iTween.Hash( "x", -400, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutCubic, "time", ++time));
		}
	}
}
