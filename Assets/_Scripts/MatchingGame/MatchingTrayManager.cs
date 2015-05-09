using UnityEngine;
using System.Collections;

public class MatchingTrayManager : MonoBehaviour {

	public MatchingTableManager[] tableManagerArr;
	MatchingTableManager currentTableManager;
	
	public void setNewTable( int totalCards ){
		foreach (MatchingTableManager tm in tableManagerArr) {
			tm.transform.localPosition = Vector3.right * 1000;
		}

		currentTableManager = tableManagerArr [totalCards / 4 - 1];
		currentTableManager.transform.localPosition = Vector3.zero;
	}

	public MatchingGameManager gameManager;
	public SkeletonAnimation signSA;
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			GameObject pickedGO = mousePick();
			if( pickedGO != null ){
				switch( pickedGO.tag ){
				case "TabBtn":
					slideTray (isTrayDown);
					break;
				case "6CardsBtn":
					setNewTable(6);
					slideTray (isTrayDown);
					break;
				case "8CardsBtn":
					setNewTable(8);
					slideTray (isTrayDown);
					break;
				case "12CardsBtn":
					setNewTable(12);
					slideTray (isTrayDown);
					break;
				case "16CardsBtn":
					setNewTable(16);
					slideTray (isTrayDown);
					break;
				case "PuzzleSign":
					signSA.state.SetAnimation(0,"Tap",false);
					signSA.state.AddAnimation(0,"Idle",false,0);
					break;
				}
			}
		}
	}

	int currentPosition = 0;
	bool isTrayDown = true;
	public GameObject handleArrow;
	void slideTray( bool slideUp ){
		int newYPos = slideUp ? 950 : 56;
		iTween.MoveTo (gameObject, iTween.Hash ("y", newYPos, "time", 1, "easetype", iTween.EaseType.easeOutBounce));

		int newHandleRot = slideUp ? 0 : 180;
		iTween.RotateTo (handleArrow, iTween.Hash ("z", newHandleRot, "time", 1, "delay", 0.5f, "easetype", iTween.EaseType.easeInOutCubic));

		isTrayDown = !isTrayDown;
		PuzzleGameManager.hasGameStarted = slideUp;
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
