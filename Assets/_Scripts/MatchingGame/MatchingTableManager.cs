using UnityEngine;
using System.Collections;
using Spine;

public class MatchingTableManager : MonoBehaviour {

	public GameObject[] cardFRONTArr;
	int currentTurn = 1;
	GameObject[] cardsInPlayArr;

	public GameObject[] allCardsArr;
	public SkeletonAnimation[] characterAnimArr;

	Vector3 cardScale;

	int[] cardFaceIndexArr;

	bool canTap = true;

	public Transform[] cameraArr;

	int cameraXOffset = 1500;

	void Start(){
		cardsInPlayArr = new GameObject[2];
		cardScale = cardFRONTArr [0].transform.localScale;
		cardFaceIndexArr = new int[allCardsArr.Length];
		fillCardIndex ();
	}

	void fillCardIndex(){
		for (int i = 0; i < cardFaceIndexArr.Length; i++) {
			cardFaceIndexArr[i] = -1;
		}

		for (int i = 0; i < cardFaceIndexArr.Length/2; i++) {
			int randomVal = Random.Range (0, 9);
			while( isContainedIn( randomVal, cardFaceIndexArr)){
				randomVal = Random.Range (0, 9);
			}

			int index1 = getRandomOpenIndexInCards();
			cardFaceIndexArr[index1] = randomVal;
			int index2 = getRandomOpenIndexInCards();
			cardFaceIndexArr[index2] = randomVal;
		}
	}

	int getRandomOpenIndexInCards(){
		int returnIndex = Random.Range (0, cardFaceIndexArr.Length);
		do {
			returnIndex = Random.Range (0, cardFaceIndexArr.Length);
		} while( cardFaceIndexArr[returnIndex] != -1 );

		return returnIndex;
	}

	bool isContainedIn( int testI, int[] arr ){
		foreach (int i in arr) {
			if( testI == i ){
				return true;
			}
		}
		return false;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown (0)) {
			GameObject pickedGO = mousePick ();
			if (pickedGO != null && canTap && pickedGO.tag == "MatchCard" && !pickedGO.GetComponent<MatchingCard>().isInUse) {
				flipCard( pickedGO );
			}
		}
	}
	
	public void flipCard( GameObject card ){
		card.GetComponent<MatchingCard> ().isInUse = true;

		currentTurn++;
		int frontIndex = currentTurn % 2;
		cardsInPlayArr [frontIndex] = card;
		Transform characterNode = card.transform.Find ("Match_Card SkeletonAnimation/SkeletonUtility-Root/root/Frame_Card");

		cardFRONTArr [frontIndex].gameObject.SetActive (false);
		cardFRONTArr [frontIndex].transform.parent = characterNode;
		cardFRONTArr [frontIndex].transform.localPosition = new Vector3 (0, 4, 0);
		cardFRONTArr [frontIndex].transform.localRotation = new Quaternion (0, 0, 0, 1);
		cardFRONTArr [frontIndex].transform.localScale = new Vector3 (131,184,1);

		int cardIndex = int.Parse( card.name.Split (" " [0])[1]);
		Vector3 position = cameraArr [frontIndex].localPosition;
		position.x = cardFaceIndexArr[cardIndex] * cameraXOffset;
		cameraArr [frontIndex].localPosition = position;

//		SkeletonAnimation skelAnim = card.transform.GetChild (0).GetComponent<SkeletonAnimation> ();
//		TrackEntry te = skelAnim.state.SetAnimation (0, "Tap", false);
		TrackEntry te = playAnimOnAllChildren( card.transform, "Tap" );

		StartCoroutine (delayedGameObjectActivate (te.animation.duration/3.0f, cardFRONTArr [frontIndex].gameObject, frontIndex == 1));
		if (frontIndex == 1) {
			canTap = false;
			StartCoroutine(endTurn(te.animation.duration));
		}
	}

	TrackEntry playAnimOnAllChildren( Transform parent, string animStr ){
		TrackEntry te = null;
		foreach (Transform child in parent) {
			SkeletonAnimation skelAnim = child.GetComponent<SkeletonAnimation> ();
			te = skelAnim.state.SetAnimation (0, animStr, false);
		}

		return te;
	}

	public Transform offscrenNode;
	public void flipCardFacedown(GameObject card){
		card.GetComponent<MatchingCard> ().isInUse = false;
		
//		SkeletonAnimation skelAnim = card.transform.GetChild (0).GetComponent<SkeletonAnimation> ();
//		TrackEntry te = skelAnim.state.SetAnimation (0, "Wrong", false);
		TrackEntry te = playAnimOnAllChildren (card.transform, "Wrong");
		StartCoroutine (delayedMakeTappable (te.animation.duration));
	}

	IEnumerator delayedMakeTappable(float delay){
		yield return new WaitForSeconds (delay);
		canTap = true;
	}

	IEnumerator endTurn( float delay  = 0){
		yield return new WaitForSeconds (delay);

		int card0Index = int.Parse( cardsInPlayArr[0].name.Split (" " [0])[1]);
		int card1Index = int.Parse( cardsInPlayArr[1].name.Split (" " [0])[1]);

		if (cardFaceIndexArr[card0Index] == cardFaceIndexArr[card1Index]) {
			StartCoroutine (animateMatchAndExit ());
		}
		else
			StartCoroutine (animateMismatch ());
	}

	IEnumerator animateMismatch(){
		//Play incorrect animation
		float duration = 0;
		foreach (SkeletonAnimation anim in characterAnimArr) {
			duration = anim.state.SetAnimation( 0, "HeadShake", false ).animation.duration;
			anim.state.AddAnimation( 0, "Idle", true, 0 );
		}
		
		yield return new WaitForSeconds (duration);
		foreach (GameObject card in cardsInPlayArr) {
			flipCardFacedown(card);
		}

		StartCoroutine (deactivateCardFronts (0.25f));
	}

	IEnumerator deactivateCardFronts( float delay ){
		yield return new WaitForSeconds (delay);
		for (int i = 0; i < 2; i++) {
			cardFRONTArr [i].gameObject.SetActive (false);
			cardFRONTArr [i].transform.parent = offscrenNode;
			cardFRONTArr [i].transform.localPosition = new Vector3 (0, 0, 0);
			cardFRONTArr [i].transform.localRotation = new Quaternion (0, 0, 0, 1);
			cardFRONTArr [i].transform.localScale = cardScale;
		}
	}

	IEnumerator animateMatchAndExit(){
		float duration = 0;
		foreach (SkeletonAnimation anim in characterAnimArr) {
			duration = anim.state.SetAnimation( 0, "Cheer", false ).animation.duration;
			anim.state.AddAnimation( 0, "Idle", true, 0 );
		}

		foreach (GameObject card in cardsInPlayArr) {
			card.transform.Translate( 0, 0, -1 );
			iTween.ScaleBy( card, iTween.Hash( "amount", 1.1f * Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeInOutBounce ));
		}

		yield return new WaitForSeconds (duration/2.0f);
		foreach (GameObject card in cardsInPlayArr) {
			iTween.MoveTo( card, iTween.Hash( "position", offscrenNode, "time", 1 ));
		}

		StartCoroutine (deactivateCardFronts (1));
		StartCoroutine (delayedMakeTappable (1));
	}

	IEnumerator delayedGameObjectActivate( float delay, GameObject go, bool isSecondTurn ){
		yield return new WaitForSeconds (delay);
		go.SetActive (true);
		if (!isSecondTurn) {
			yield return new WaitForSeconds (delay);
		}
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
