using UnityEngine;
using System.Collections;

public class NavManager : MonoBehaviour {

	void OnGUI(){
		if( GUI.Button( new Rect( 0, 0, Screen.width * 0.1f, Screen.height * 0.05f), "Back")){
			MapManager.openPageIndex = 1;
			Application.LoadLevel("MainMenu Map");
		}
	}
}
