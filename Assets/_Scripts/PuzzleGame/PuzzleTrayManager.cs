using UnityEngine;
using System.Collections;

public class PuzzleTrayManager : MonoBehaviour {

	public PuzzleGameManager gameManager;
	public SkeletonAnimation signSA;
	public SkeletonAnimation borderAnim;
	public GameObject confetti;
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
					startNewGame(PuzzleGameManager.PuzzleAnimal.Cat);
					break;
				case "PuzzleDogBtn":
					startNewGame(PuzzleGameManager.PuzzleAnimal.Dog);
					break;
				case "PuzzleMonkeyBtn":
					startNewGame(PuzzleGameManager.PuzzleAnimal.Monkey);
					break;
				case "PuzzleFoxBtn":
					startNewGame(PuzzleGameManager.PuzzleAnimal.Fox);
					break;
				case "PuzzleBearBtn":
					startNewGame(PuzzleGameManager.PuzzleAnimal.Bear);
					break;
				case "PuzzleBirdBtn":
					startNewGame(PuzzleGameManager.PuzzleAnimal.Bird);
					break;
				case "PuzzleBunnyBtn":
					startNewGame(PuzzleGameManager.PuzzleAnimal.Bunny);
					break;
				case "PuzzleFrogBtn":
					startNewGame(PuzzleGameManager.PuzzleAnimal.Frog);
					break;
				case "PuzzleLionBtn":
					startNewGame(PuzzleGameManager.PuzzleAnimal.Lion);
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

	void startNewGame( PuzzleGameManager.PuzzleAnimal animal ){
		gameManager.loadNewAnimal(animal);
		gameManager.resetAllPieces();
		slideTray (isTrayDown);
		borderAnim.gameObject.SetActive(false);
		confetti.SetActive (false);
	}

	int currentPosition = 0;
	bool isTrayDown = true;
	public GameObject handleArrow;
	void slideTray( bool slideUp ){
		int newYPos = slideUp ? 950 : 56;
		iTween.MoveTo (gameObject, iTween.Hash ("y", newYPos, "time", 1, "easetype", iTween.EaseType.easeOutBounce,
		                                        "oncomplete", "trayIsDoneAnimating", "oncompletetarget", gameObject));

		int newHandleRot = slideUp ? 0 : 180;
		iTween.RotateTo (handleArrow, iTween.Hash ("z", newHandleRot, "time", 1, "delay", 0.5f, "easetype", iTween.EaseType.easeInOutCubic));

		isTrayDown = !isTrayDown;
		PuzzleGameManager.hasGameStarted = slideUp;
	}
	
	void trayIsDoneAnimating(){
		Vector3 pos = transform.localPosition;
		pos.z = pos.x < 100 ? -12 : -1;
		transform.localPosition = pos;
	}

	public GameObject slidingThumbs;
	void slideThumbs( bool isLeft ){
		int direction = isLeft ? -1 : 1;
		currentPosition += direction;
		iTween.MoveTo (slidingThumbs, iTween.Hash ("x", currentPosition * 700, "time", 1, "easetype", iTween.EaseType.easeOutBack));
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
