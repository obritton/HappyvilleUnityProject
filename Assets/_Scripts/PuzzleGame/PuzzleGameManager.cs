using UnityEngine;
using System.Collections;

public class PuzzleGameManager : GameManagerBase {

	public Transform[] startNodeArr;
	public Transform[] puzzleNodeArr;

	public GameObject[] characterPrefabArr;
	public Texture2D[] textureArr;
	public SkeletonAnimation currentPuzzleCharacter;
	public Renderer camBGRenderer;

	public GameObject[] puzzlePieceArr;
	// Use this for initialization
	void Start () {
		DoorManager.openDoors ();
		resetAllPieces ();
		StartCoroutine (fireRandomAnims ());
	}

	IEnumerator fireRandomAnims(){
		while (true) {
			yield return new WaitForSeconds( 10 );

			string newAnim = "Idle";
			switch( Random.Range( 0, 5 ))
			{
			case 0:
				newAnim = "Pant";
				break;
			case 1:
				newAnim = "Dance";
				break;
			case 2:
				newAnim = "TouchOne";
				break;
			case 3:
				newAnim = "TouchTwo";
				break;
			case 4:
				newAnim = "TouchThree";
				break;
			case 5:
				newAnim = "Wrong";
				break;
			}
			currentPuzzleCharacter.state.SetAnimation( 0, newAnim, false);
			currentPuzzleCharacter.state.AddAnimation( 0, "Idle", true, 0 );
		}
	}

	public static bool hasGameStarted = false;
	GameObject draggingPP = null;
	Vector3 lastMousePos;
	void Update(){
		if (hasGameStarted && Input.GetMouseButtonDown (0)) {
			GameObject pickedGO = mousePick();
			if( pickedGO != null ){
				switch( pickedGO.tag ){
				case "PuzzlePiece":
					draggingPP = pickedGO;
					draggingPP.transform.parent = null;
					iTween.ScaleTo( pickedGO, iTween.Hash ( "scale", 25000*Vector3.one, "easetype", iTween.EaseType.easeOutBack, "time", 0.5f ));
					draggingPP.transform.Translate(0, 1, 0 );
					lastMousePos = Input.mousePosition;
					break;
				}
			}
		}

		if (hasGameStarted && Input.GetMouseButton (0)) {
			if( draggingPP != null ){
				Vector3 delta = Camera.main.ScreenToWorldPoint( Input.mousePosition ) - Camera.main.ScreenToWorldPoint(lastMousePos);
				delta.z = delta.y;
				delta.y = 0;
				lastMousePos = Input.mousePosition;
				draggingPP.transform.Translate(delta);
			}
		}

		if (hasGameStarted && Input.GetMouseButtonUp (0)) {
			if( draggingPP != null ){
				if( Input.mousePosition.y < 124 )
				{
					movePieceIntoStartNodes(draggingPP);
				}
				else
				{
					movePieceIntoPuzzle(draggingPP);
				}
			}
			lastDraggedPiece = draggingPP;
			draggingPP = null;
		}
	}

	void movePieceIntoPuzzle( GameObject piece ){
		movePieceIntoNodes (piece, puzzleNodeArr);
		iTween.ScaleTo (piece, iTween.Hash ("scale", Vector3.one * 18800, "easetype", iTween.EaseType.easeOutBounce ));
	}

	void movePieceIntoStartNodes( GameObject piece, bool isImmediate = false ){
		movePieceIntoNodes (piece, startNodeArr, isImmediate);
		float time = isImmediate ? 0 : 1;
		iTween.ScaleTo (piece, iTween.Hash ("scale", Vector3.one * 10000, "easetype", iTween.EaseType.easeOutBounce, "time", time ));
	}

	public void resetAllPieces()
	{
		foreach (GameObject piece in puzzlePieceArr) {
			piece.transform.parent = null;
		}

		foreach (GameObject piece in puzzlePieceArr) {
			piece.transform.position = Random.onUnitSphere * 1000;
			movePieceIntoStartNodes( piece, true );
		}
	}

	GameObject lastDraggedPiece = null;
	void pushBack()
	{
//		lastDraggedPiece.transform.localPosition = Vector3.zero;
	}

	void movePieceIntoNodes( GameObject piece, Transform[] nodes, bool isImmediate = false )
	{
		ArrayList openNodes = getOpenNodesIn (nodes );

		if (openNodes.Count > 0) {
			Transform closestNode = null;
			float closestDistance = 9999999;
			foreach( Transform node in openNodes ){
				float distance = Vector3.Distance( piece.transform.position, node.position );
				if( distance < closestDistance ){
					closestDistance = distance;
					closestNode = node;
				}
			}
			piece.transform.parent = closestNode;
			float time = isImmediate ? 0 : 1;
			iTween.MoveTo( piece, iTween.Hash ( "position", Vector3.zero, "islocal", true, "easetype", iTween.EaseType.easeOutBounce, "time", time,
			                                   "oncomplete", "pushBack", "oncompletetarget", gameObject));
		}

		if (checkForWin ())
			StartCoroutine(doWin ());
	}

	ArrayList getOpenNodesIn( Transform[] nodes ){
		ArrayList openNodes = new ArrayList ();

		foreach (Transform node in nodes)
		{
			if( node.childCount == 0 )
				openNodes.Add(node);
		}

		return openNodes;
	}

	public enum PuzzleAnimal{ Cat, Dog, Monkey, Fox, Bear, Bird, Bunny, Frog, Lion };
	public void loadNewAnimal( PuzzleAnimal puzzleAnimal)
	{
		int newAnimalIndex = 0;
		switch (puzzleAnimal) {
		case PuzzleAnimal.Cat:
			newAnimalIndex = 0;
			break;
		case PuzzleAnimal.Dog:
			newAnimalIndex = 1;
			break;
		case PuzzleAnimal.Monkey:
			newAnimalIndex = 2;
			break;
		case PuzzleAnimal.Fox:
			newAnimalIndex = 3;
			break;
		case PuzzleAnimal.Bear:
			newAnimalIndex = 4;
			break;
		case PuzzleAnimal.Bird:
			newAnimalIndex = 5;
			break;
		case PuzzleAnimal.Bunny:
			newAnimalIndex = 6;
			break;
		case PuzzleAnimal.Frog:
			newAnimalIndex = 7;
			break;
		case PuzzleAnimal.Lion:
			newAnimalIndex = 8;
			break;
		}

		GameObject newAnimal = Instantiate( characterPrefabArr[newAnimalIndex], currentPuzzleCharacter.transform.position, Quaternion.identity) as GameObject;
		newAnimal.transform.localScale = Vector3.one * 2.8f;
		Destroy (currentPuzzleCharacter.gameObject);
		currentPuzzleCharacter = newAnimal.GetComponent<SkeletonAnimation>();

		camBGRenderer.material.SetTexture ("_MainTex", textureArr [newAnimalIndex]);
	}

	bool checkForWin(){
		foreach (Transform node in puzzleNodeArr) {
			if( node.childCount == 0 ){
				return false;
			}else{
				string[] nodeNameArr = node.name.Split("n"[0]);
				string[] childNameArr = node.GetChild(0).name.Split("_"[0]);
				int nodeNumber = int.Parse(nodeNameArr[1]);
				int childNumber = int.Parse (childNameArr[1]);

				if( nodeNumber != 10 - childNumber )
					return false;
			}
		}

		return true;
	}

	public GameObject borderLights;
	public GameObject confettiSystem;
	IEnumerator doWin(){
		base.doWin ();
		hasGameStarted = false;
		confettiSystem.SetActive (true);
		yield return new WaitForSeconds (1);
		borderLights.SetActive (true);
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
