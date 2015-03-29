using UnityEngine;
using Spine;
using System.Collections;

public class TableGame : MonoBehaviour {
	
	public Table table;

	void OnGUI(){
		if( GUI.Button( new Rect( 0, 0, Screen.width * 0.1f, Screen.height * 0.05f), "Back")){
			MapManager.openPageIndex = (Table.level/3)+1;
			iTween.Stop ();
			SoundManager.Stop();
			StartCoroutine(closeDoors());
		}
	}

	// Use this for initialization
	void Start () {
		Physics.gravity = Vector3.down * 9.81f;
		StartCoroutine(openDoors ());
	}

	public DoorManager doorManager;
	IEnumerator openDoors()
	{
		yield return new WaitForSeconds (0.5f);
		if( doorManager ){
			StartCoroutine(doorManager.openDoors());
		}

		yield return new WaitForSeconds (1 + 0.5f);
		StartCoroutine (startNewGame ());
	}

	IEnumerator closeDoors()
	{
		yield return new WaitForSeconds (0.5f);
		if( doorManager ){
			StartCoroutine(doorManager.closeDoors());
		}
		
		yield return new WaitForSeconds (1 + 0.5f);
		iTween.Stop ();
		Application.LoadLevel("MainMenu Map");
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
		table.jumpAllPlates ();
		table.addIdelForAll ();
		yield return new WaitForSeconds (time);
		table.animateMatchingFoodOn ();
	}
}
