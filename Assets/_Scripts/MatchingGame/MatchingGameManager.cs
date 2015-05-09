using UnityEngine;
using System.Collections;

public class MatchingGameManager : GameManagerBase {

	public GameObject[] cardFRONTArr;

	// Use this for initialization
	void Start () {
		DoorManager.openDoors ();
	}

	int currentTurn = 0;
	void Update()
	{
		if (Input.GetMouseButtonDown (0)) {
			GameObject pickedGO = mousePick ();
			if (pickedGO != null && pickedGO.tag == "MatchCard") {
				flipCard( pickedGO );
			}
		}
	}

	void flipCard( GameObject card ){
		currentTurn++;
		int frontIndex = currentTurn % 2;
		cardFRONTArr [frontIndex].transform.parent = card.transform;
		cardFRONTArr [frontIndex].transform.localPosition = new Vector3 (0, 1, 0);
		cardFRONTArr [frontIndex].transform.localRotation = new Quaternion (0, 180, 0, 1);
		iTween.RotateBy( card, iTween.Hash("y", 0.5f));
	}

	void doWin(){
		base.doWin ();
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
