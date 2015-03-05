using UnityEngine;
using System.Collections;

public class ResTestManuManager : MonoBehaviour {

	void OnGUI(){
		if (GUI.Button (new Rect (0, 0, Screen.width/2, Screen.height / 2), "Somian + WhiteSpace")) {
			iTween.Stop ();
			Application.LoadLevel("ResTestSomianWhitespace");
		}
		if (GUI.Button (new Rect (Screen.width/2, 0, Screen.width/2, Screen.height / 2), "Somian Stripped")) {
			iTween.Stop ();
			Application.LoadLevel("ResTestSomianStripped");
		}

		if (GUI.Button (new Rect (0, Screen.height / 2, Screen.width/2, Screen.height / 2), "Spine + WhiteSpace")) {
			iTween.Stop ();
			Application.LoadLevel("ResTestSpineWhitespace");
		}
		if (GUI.Button (new Rect (Screen.width/2, Screen.height / 2, Screen.width/2, Screen.height / 2), "Spine Stripped")) {
			iTween.Stop ();
			Application.LoadLevel("ResTestSpineStripped");
		}
	}
}
