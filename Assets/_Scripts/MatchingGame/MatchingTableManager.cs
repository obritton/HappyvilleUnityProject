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

	void Start(){
		cardsInPlayArr = new GameObject[2];
		cardScale = cardFRONTArr [0].transform.localScale;
		cardFaceIndexArr = new int[allCardsArr.Length];
	}

	void fillCardIndex(){
		for (int i = 0; i < cardFaceIndexArr.Length; i++) {
			cardFaceIndexArr[i] = -1;
		}

		for (int i = 0; i < cardFaceIndexArr.Length/2; i++) {
			int randomI = -1;
			do{
				randomI = Random.Range(0, 9);
			}while( isContainedIn(randomI, cardFaceIndexArr));
			cardFaceIndexArr[i] = randomI;
			cardFaceIndexArr[i*2] = randomI;
		}
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
		cardFRONTArr [frontIndex].transform.localPosition = new Vector3 (0, 0, 0);
		cardFRONTArr [frontIndex].transform.localRotation = new Quaternion (0, 0, 0, 1);

		SkeletonAnimation skelAnim = card.transform.GetChild (0).GetComponent<SkeletonAnimation> ();
		TrackEntry te = skelAnim.state.SetAnimation (0, "Tap", false);

		StartCoroutine (delayedGameObjectActivate (te.animation.duration/3.0f, cardFRONTArr [frontIndex].gameObject, frontIndex == 1));
		if (frontIndex == 1) {
			canTap = false;
			StartCoroutine(endTurn(te.animation.duration));
		}
	}

	public Transform offscrenNode;
	public void flipCardFacedown(GameObject card){
		card.GetComponent<MatchingCard> ().isInUse = false;
		
		SkeletonAnimation skelAnim = card.transform.GetChild (0).GetComponent<SkeletonAnimation> ();
		TrackEntry te = skelAnim.state.SetAnimation (0, "Wrong", false);
		StartCoroutine (delayedMakeTappable (te.animation.duration));
	}

	IEnumerator delayedMakeTappable(float delay){
		yield return new WaitForSeconds (delay);
		canTap = true;
	}

	IEnumerator endTurn( float delay  = 0){
		yield return new WaitForSeconds (delay);

		StartCoroutine (animateMatchAndExit ());
//		StartCoroutine (animateMismatch ());
	}

	IEnumerator animateMismatch(){
		//Play incorrect animation
		float duration = 0;
		foreach (SkeletonAnimation anim in characterAnimArr) {
			duration = anim.state.SetAnimation( 0, "Wrong", false ).animation.duration;
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
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
