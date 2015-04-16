﻿using UnityEngine;
using System.Collections;

public class OwlTreeTapManager : MonoBehaviour {

	void Start(){
		lastPopoutIndex = Random.Range(0,2);
		StartCoroutine (loopRandomPopups ());
	}

	bool canPopup = true;
	IEnumerator loopRandomPopups(){
		while (true) {
			yield return new WaitForSeconds( Random.Range (4.0f,10.0f));
			if( canPopup )
				doNextPopup();
		}
	}

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	bool isOwlDucked = true;
	int lastPopoutIndex = 0;
	void Update(){
		if (Input.GetMouseButtonDown (0)) {
			if( mousePick() == gameObject ){
				canPopup = false;
				StartCoroutine( delayedAllowPopup());
				doNextPopup();
			}
		}
	}

	IEnumerator delayedAllowPopup(){
		yield return new WaitForSeconds (3);
		canPopup = true;
	}

	void doNextPopup(){
		SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation>();
		if( skelAnim != null ){
			if( isOwlDucked ){
				skelAnim.state.SetAnimation( 0, "Popout_" + (lastPopoutIndex+1), false );
				skelAnim.state.AddAnimation(0, "Idle_" + (lastPopoutIndex+1), true, 0 );
			}
			else{
				skelAnim.state.SetAnimation( 0, "Duck_" + (lastPopoutIndex+1), false );
				lastPopoutIndex = Random.Range(0,2);
			}
			isOwlDucked = !isOwlDucked;
		}
	}
}
