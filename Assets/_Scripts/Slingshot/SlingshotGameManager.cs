using UnityEngine;
using System.Collections;
using Spine;

public class SlingshotGameManager : FrenziableGame {

	public SlingshotFingerFollower fingerFollower;

	void Start () {
		StartCoroutine (openDoors ());
		StartCoroutine (makeBearPoint ());
	}

	bool isBearWaiting = true;
	IEnumerator makeBearPoint()
	{
		while (isBearWaiting) {
			yield return new WaitForSeconds(3);
			bearAnim.state.SetAnimation(0, "Point", false);
			bearAnim.state.AddAnimation(0, "Idle", true, 0);
		}
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	IEnumerator openDoors()
	{
		yield return new WaitForSeconds (0.5f);
		DoorManager.openDoors ();
	}

	public SkeletonAnimation bearAnim;
	public GameObject backgroundNode;

	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			GameObject pickedGO = mousePick ();
			if( pickedGO )
			switch (pickedGO.tag) {
			case "StartBtn":
				TrackEntry te = pickedGO.GetComponent<SkeletonAnimation> ().state.SetAnimation (0, "Tap", false);
				StartCoroutine (delayedButtonMove (te.animation.duration, pickedGO));
				isBearWaiting = false;
				iTween.MoveBy( backgroundNode, iTween.Hash("x", -765, "time", 2, "easetype", iTween.EaseType.easeInOutQuad));
				StartCoroutine(delayedGameStartSignal(2));
				fingerFollower.startTargetsMoving();
				break;
			}
		}
	}

	IEnumerator delayedGameStartSignal( float delay ){
		yield return new WaitForSeconds (delay);
		SlingshotFingerFollower.hasGameStarted = true;
	}

	IEnumerator delayedButtonMove( float delay, GameObject startButton ){
		yield return new WaitForSeconds(delay);
		startButton.transform.Translate (Vector3.left * 1000);
	}

	public override void startFrenzy(){

	}

	public override void timerComplete ()
	{
	
	}
}
