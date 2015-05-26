using UnityEngine;
using System.Collections;

public class ScoreBoardTESTER : MonoBehaviour {

	public ScoreboardManager sbManager;

	bool pressed = false;
	void OnGUI(){
		if (! pressed && GUI.Button (new Rect (0, 0, Screen.width, Screen.height), "")) {
			pressed = true;
			sbManager.showResults (10, 20, 30);
		}
	}
}
