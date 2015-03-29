using UnityEngine;
using System.Collections;

public class NavManager : MonoBehaviour {

	bool isBackingOut = false;

	public GUIStyle backStyle;


	void OnGUI(){
		if( !isBackingOut && GUI.Button( new Rect( Screen.width/64, Screen.width/64, Screen.width /6.4f, Screen.width /6.4f), "", backStyle)){
			isBackingOut = true;
			SoundManager.Stop();
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
