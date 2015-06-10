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
//			if (pickedGO != null && canTap && pickedGO.tag == "MatchCard" && !pickedGO.GetComponent<MatchingCard>().isInUse) {
			if (pickedGO != null && pickedGO.tag == "MatchCard" && !pickedGO.GetComponent<MatchingCard>().isInUse) {
				if( canTap )
				{
					flipCard( pickedGO );
				}
				else
				{
					StartCoroutine(flipTwoBack());
				}
			}
		}
	}

	IEnumerator flipTwoBack(){
		yield return new WaitForSeconds(0);
	}

	public void flipCard( GameObject card ){
		card.GetComponent<MatchingCard> ().isInUse = true;
		card.transform.Translate (0, 0, -1);
		StartCoroutine(delayedCardSettleDown(card, 1));

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
		card.transform.Translate (0, 0, -1);
		TrackEntry te = playAnimOnAllChildren( card.transform, "Tap" );

		StartCoroutine (delayedGameObjectActivate (te.animation.duration/3.0f, cardFRONTArr [frontIndex].gameObject, frontIndex == 1));
		if (frontIndex == 1) {
			canTap = false;
			StartCoroutine(endTurn(te.animation.duration));
		}
	}

	IEnumerator delayedCardSettleDown( GameObject card, float delay ){
		yield return new WaitForSeconds (delay);
		card.transform.Translate (0, 0, 1);
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

	public SkeletonAnimation[] burstArr;
	public SkeletonAnimation[] wordArr;

	IEnumerator animateMatchAndExit(){
		float duration = 0;
		foreach (SkeletonAnimation anim in characterAnimArr) {
			duration = anim.state.SetAnimation( 0, "Cheer", false ).animation.duration;
			anim.state.AddAnimation( 0, "Idle", true, 0 );
		}

		foreach (GameObject card in cardsInPlayArr) {
			card.transform.Translate( 0, 0, -5 );
			iTween.ScaleBy( card, iTween.Hash( "amount", 1.1f * Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeInOutBounce ));
		}

		yield return new WaitForSeconds (0);
		for (int i = 0; i < 2; i++) {

			int cardIndex = int.Parse( cardsInPlayArr[0].name.Split (" " [0])[1]);
			int characterIndex = cardFaceIndexArr[cardIndex];
			string skinName = "";
			switch( characterIndex ){
			case 0:	skinName = "Bear";		break;
			case 1:	skinName = "Bunny";		break;
			case 2:	skinName = "Monkey";	break;
			case 3:	skinName = "Lion";		break;
			case 4:	skinName = "Frog";		break;
			case 5:	skinName = "Fox";		break;
			case 6:	skinName = "Dog";		break;
			case 7:	skinName = "Cat";		break;
			case 8:	skinName = "Bird";		break;
			}
			wordArr[i].skeleton.SetSkin(skinName);

			burstArr[i].state.SetAnimation (0, "Correct" + Random.Range(1,4), false);
			wordArr[i].state.SetAnimation (0, "Correct", false);

			burstArr[i].transform.position = cardsInPlayArr[i].transform.position;
			burstArr[i].transform.Translate(0, -10, -1);
			wordArr[i].transform.position = cardsInPlayArr[i].transform.position;
			wordArr[i].transform.Translate(0, -10, -2);

			foreach( Transform child in cardsInPlayArr[i].transform ){
				child.GetComponent<SkeletonAnimation>().state.SetAnimation(0, "Correct", false );
			}
			
		}

		StartCoroutine (deactivateCardFronts (2));
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
