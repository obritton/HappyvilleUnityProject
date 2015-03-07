using UnityEngine;
using System.Collections;

public class NavManager : MonoBehaviour {

	void OnGUI(){
		if( GUI.Button( new Rect( 0, 0, Screen.width * 0.1f, Screen.height * 0.05f), "Back")){
			MapManager.openPageIndex = ((Application.loadedLevelName == "TableGame") ? (Table.level/3) + 1 : MapManager.openPageIndex);
			iTween.Stop ();

			GameObject doors = GameObject.Find ("Doors");
			DoorManager doorManager = doors.GetComponent<DoorManager>();
			if( doorManager )
			{
				StartCoroutine(doorManager.closeDoors());
			}

			StartCoroutine(delayedMenuLoad());
		}
	}

	IEnumerator delayedMenuLoad()
	{
		yield return new WaitForSeconds (1);
		Application.LoadLevel("MainMenu Map");
	}
}
