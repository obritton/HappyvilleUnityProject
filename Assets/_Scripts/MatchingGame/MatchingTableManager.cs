using UnityEngine;
using System.Collections;
using Spine;

public class MatchingTableManager : MonoBehaviour {

	public GameObject[] cardFRONTArr;
	int currentTurn = 1;
	GameObject[] cardsInPlayArr;

	public GameObject[] allCardsArr;

	void Start(){
		cardsInPlayArr = new GameObject[2];
	}

	void Update()
	{
		if (Input.GetMouseButtonDown (0)) {
			GameObject pickedGO = mousePick ();
			if (pickedGO != null && pickedGO.tag == "MatchCard" && !pickedGO.GetComponent<MatchingCard>().isInUse) {
				flipCard( pickedGO );
			}
		}
	}
	
	public void flipCard( GameObject card ){
		card.GetComponent<MatchingCard> ().isInUse = true;

		currentTurn++;
		int frontIndex = currentTurn % 2;
		cardsInPlayArr [frontIndex] = card;
		Transform characterNode = card.transform.Find ("Card SkeletonAnimation/SkeletonUtility-Root/root/Frame_Card");

		cardFRONTArr [frontIndex].gameObject.SetActive (false);
		cardFRONTArr [frontIndex].transform.parent = characterNode;
		cardFRONTArr [frontIndex].transform.localPosition = new Vector3 (0, 0, 0);
		cardFRONTArr [frontIndex].transform.localRotation = new Quaternion (0, 0, 0, 1);

		SkeletonAnimation skelAnim = card.transform.GetChild (0).GetComponent<SkeletonAnimation> ();
		TrackEntry te = skelAnim.state.SetAnimation (0, "animation", false);

		StartCoroutine (delayedGameObjectActivate (te.animation.duration/3.0f, cardFRONTArr [frontIndex].gameObject, frontIndex == 1));
		if (frontIndex == 1) {
			StartCoroutine(endTurn(te.animation.duration));
		}
	}

	IEnumerator endTurn( float delay  = 0){
		yield return new WaitForSeconds (delay);

//		StartCoroutine (animateMatchAndExit ());
		StartCoroutine (animateMismatch ());
	}

	IEnumerator animateMismatch(){
		yield return new WaitForSeconds (1);
	}

	IEnumerator animateMatchAndExit(){
		yield return new WaitForSeconds (1);
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
