using UnityEngine;
using System.Collections;

public class TableGame : MonoBehaviour {
	
	public Table table;
	public FlySession flySession;

	void OnGUI(){
		if( GUI.Button( new Rect( 0, 0, Screen.width * 0.1f, Screen.height * 0.05f), "Back")){
			MapManager.openPageIndex = 1;
			Application.LoadLevel("MainMenu Map");
		}
	}

	// Use this for initialization
	void Start () {
		StartCoroutine(openDoors ());
	}

	IEnumerator openDoors()
	{
		yield return new WaitForSeconds (0.5f);
		GameObject doors = GameObject.Find ("Doors");
		if (doors) {
			DoorManager doorManager = (DoorManager)doors.GetComponent<DoorManager>();
			if( doorManager ){
				StartCoroutine(doorManager.openDoors());
			}
		}
		
		StartCoroutine (startNewGame ());
	}

	IEnumerator startNewGame(){
		yield return new WaitForSeconds (TGConsts.kFirstPopupDelay);

		float time = 0;
		for (int i = 0; i < 3; i++) {
			time = table.addNewCharacterAtSpot (i);
			yield return new WaitForSeconds (TGConsts.kFirstPopupsStaggar);
		}

		StartCoroutine (animateTableTapAndFoodAfterDelay (time+0.5f));//Monkey anim kind of slow
	}

	IEnumerator animateTableTapAndFoodAfterDelay( float delay = 0)
	{
		yield return new WaitForSeconds(delay);
		float time = table.makeAllTap ();
		table.addIdelForAll ();
		yield return new WaitForSeconds (time);
		table.animateMatchingFoodOn ();
	}
}
