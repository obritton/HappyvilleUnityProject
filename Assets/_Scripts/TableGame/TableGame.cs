using UnityEngine;
using Spine;
using System.Collections;

public class TableGame : MonoBehaviour {
	
	public Table table;

	// Use this for initialization
	void Start () {
		Physics.gravity = Vector3.down * 9.81f;
		StartCoroutine(openDoors ());
	}

	IEnumerator openDoors()
	{
		yield return new WaitForSeconds (0.5f);
		DoorManager.openDoors ();

		yield return new WaitForSeconds (1.5f);
		StartCoroutine (startNewGame ());
	}

	IEnumerator closeDoors()
	{
		yield return new WaitForSeconds (0.5f);
		DoorManager.closeDoors ();
		
		yield return new WaitForSeconds (2f);
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
		table.createMatchingFood ();
		StartCoroutine (table.animateFoodOn ());
	}
}
