using UnityEngine;
using System.Collections;

public class ForParentsBtnManager : MonoBehaviour {

	void Update(){
		if (Input.GetMouseButtonDown (0))
		{
			if( mousePick() == gameObject) {
				initiateParentsGestureListener();
				skelAnim.state.SetAnimation(0, "Tap", false );
			}
		}
	}

	public SkeletonAnimation skelAnim;

	int targetNumFingers = -1;
	FingerGestures.SwipeDirection targetDir = FingerGestures.SwipeDirection.None;

	public SwipeRecognizer swipeRecognizer;
	void initiateParentsGestureListener(){
		targetNumFingers = Random.value < 0.5f ? 3 : 2;
		string skinName = (targetNumFingers == 2 ? "Two" : "Three");
		swipeRecognizer.RequiredFingerCount = targetNumFingers;
		skinName += "_";
		targetDir = FingerGestures.SwipeDirection.Right;
		switch (Random.Range (0, 4)) {
		case 0:
			targetDir = FingerGestures.SwipeDirection.Left;
			skinName += "Left";
			break;
		case 1:
			targetDir = FingerGestures.SwipeDirection.Up;
			skinName += "Up";
			break;
		case 2:
			targetDir = FingerGestures.SwipeDirection.Down;
			skinName += "Down";
			break;
		default:
			targetDir = FingerGestures.SwipeDirection.Right;
			skinName += "Right";
			break;
		}

		skelAnim.skeleton.SetSkin(skinName);

		print ("skinName: " + skinName);
	}

	void endParentsGestureListener(){
		targetNumFingers = -1;
		targetDir = FingerGestures.SwipeDirection.None;
		skelAnim.state.SetAnimation(0, "Idle", true );
	}

	public void OnSwipe( SwipeGesture gesture ){
		int totalFingers = gesture.Fingers.Count;
		FingerGestures.SwipeDirection dir = gesture.Direction;

		print ("ForParents OnSwipe totalFingers: " + totalFingers + ", dir: " + dir);
		if (targetDir == gesture.Direction && targetNumFingers == gesture.Fingers.Count)
			openParentsPage();

		endParentsGestureListener();
	}

	public GameObject forParentsPage;
	void openParentsPage(){
		forParentsPage.SetActive (true);
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
}
