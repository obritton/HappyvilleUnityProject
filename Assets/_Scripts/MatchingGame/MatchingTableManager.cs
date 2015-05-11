using UnityEngine;
using System.Collections;
using Spine;

public class MatchingTableManager : MonoBehaviour {

	public GameObject[] cardFRONTArr;
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

	public void flipCard( GameObject card ){
		currentTurn++;
		int frontIndex = currentTurn % 2;
		Transform characterNode = card.transform.Find ("Card SkeletonAnimation/SkeletonUtility-Root/root/Frame_Card");

		cardFRONTArr [frontIndex].gameObject.SetActive (false);
		cardFRONTArr [frontIndex].transform.parent = characterNode;
		cardFRONTArr [frontIndex].transform.localPosition = new Vector3 (0, 0, 0);
		cardFRONTArr [frontIndex].transform.localRotation = new Quaternion (0, 0, 0, 1);

		SkeletonAnimation skelAnim = card.transform.GetChild (0).GetComponent<SkeletonAnimation> ();
		TrackEntry te = skelAnim.state.SetAnimation (0, "animation", false);


		StartCoroutine (delayedGameObjectActivate (te.animation.duration/3.0f, cardFRONTArr [frontIndex].gameObject));
	}

	IEnumerator delayedGameObjectActivate( float delay, GameObject go ){
		yield return new WaitForSeconds (delay);
		go.SetActive (true);
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
