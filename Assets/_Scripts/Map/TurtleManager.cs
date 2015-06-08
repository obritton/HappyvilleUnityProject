using UnityEngine;
using System.Collections;

public class TurtleManager : SingleSoundBase {

	// Use this for initialization
	void Start () {
		StartCoroutine(startTurtle ());
	}

	float swimTime = 20;
	IEnumerator startTurtle(){
		Vector3 startPos = new Vector3 (0.6f, -4.5f, -3);
		transform.localPosition = startPos;
		playAnimStr ("Popup", true);
		if (MapManager.currentPage == 6 && MapManager.canSoundsPlay)
//			SoundManager.PlaySFX ("Turtle_Popup");
			playSingleSound ("Turtle_Popup");
		yield return new WaitForSeconds (2);
		iTween.MoveTo (gameObject, iTween.Hash ("position", new Vector3(-14, -4.5f, -3), "time", swimTime, "islocal", true, "easetype", iTween.EaseType.linear ));
		yield return new WaitForSeconds (swimTime);
		playAnimStr ("TouchOne", false);
//		SoundManager.PlaySFX ("Turtle_Touch1");

		if( MapManager.currentPage == 4)	playSingleSound ("Turtle_Touch1");
		yield return new WaitForSeconds (3);
		StartCoroutine (startTurtle ());
	}

	void playAnimStr( string animStr, bool idleFollows ){
		SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation> ();
		if (skelAnim) {
			skelAnim.state.SetAnimation(0, animStr, false );
			if( idleFollows )
				skelAnim.state.AddAnimation(0,"Idle",true,0);
		}
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}
	
	int currentTapIndex = 0;
	void Update(){
		if (Input.GetMouseButtonDown (0)) {
			if (mousePick () == gameObject) {
				SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation> ();
				if (skelAnim != null) {
					playAnimStr("TouchTwo", true);
					if( MapManager.currentPage >= 4 && MapManager.currentPage <= 6 && MapManager.canSoundsPlay)
//					SoundManager.PlaySFX ("Turtle_Touch2");
						playSingleSound ("Turtle_Touch2");
				}
			}
		}
	}
}
