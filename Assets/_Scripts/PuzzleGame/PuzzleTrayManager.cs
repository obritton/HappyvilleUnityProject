using UnityEngine;
using System.Collections;

public class PuzzleTrayManager : MonoBehaviour {

	public PuzzleGameManager gameManager;
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
				case "PuzzleCatBtn":
					gameManager.loadNewAnimal(PuzzleGameManager.PuzzleAnimal.Cat);
					gameManager.resetAllPieces();
					slideTray (isTrayDown);
					break;
				case "PuzzleDogBtn":
					gameManager.loadNewAnimal(PuzzleGameManager.PuzzleAnimal.Dog);
					gameManager.resetAllPieces();
					slideTray (isTrayDown);
					break;
				case "PuzzleMonkeyBtn":
					gameManager.loadNewAnimal(PuzzleGameManager.PuzzleAnimal.Monkey);
					gameManager.resetAllPieces();
					slideTray (isTrayDown);
					break;
				case "PuzzleFoxBtn":
					gameManager.loadNewAnimal(PuzzleGameManager.PuzzleAnimal.Fox);
					gameManager.resetAllPieces();
					slideTray (isTrayDown);
					break;
				case "PuzzleBearBtn":
					gameManager.loadNewAnimal(PuzzleGameManager.PuzzleAnimal.Bear);
					gameManager.resetAllPieces();
					slideTray (isTrayDown);
					break;
				case "PuzzleBirdBtn":
					gameManager.loadNewAnimal(PuzzleGameManager.PuzzleAnimal.Bird);
					gameManager.resetAllPieces();
					slideTray (isTrayDown);
					break;
				case "PuzzleBunnyBtn":
					gameManager.loadNewAnimal(PuzzleGameManager.PuzzleAnimal.Bunny);
					gameManager.resetAllPieces();
					slideTray (isTrayDown);
					break;
				case "PuzzleFrogBtn":
					gameManager.loadNewAnimal(PuzzleGameManager.PuzzleAnimal.Frog);
					gameManager.resetAllPieces();
					slideTray (isTrayDown);
					break;
				case "PuzzleLionBtn":
					gameManager.loadNewAnimal(PuzzleGameManager.PuzzleAnimal.Lion);
					gameManager.resetAllPieces();
					slideTray (isTrayDown);
					break;
				case "LeftArrow":
					slideThumbs(true);
					break;
				case "RightArrow":
					slideThumbs(false);
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

	public GameObject slidingThumbs;
	void slideThumbs( bool isLeft ){
		int direction = isLeft ? -1 : 1;
		currentPosition += direction;
		iTween.MoveTo (slidingThumbs, iTween.Hash ("x", currentPosition * 700, "time", 1, "easetype", iTween.EaseType.easeOutBack));
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
