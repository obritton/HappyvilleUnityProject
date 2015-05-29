﻿using UnityEngine;
using System.Collections;

public class DogHouseTapManager : SingleSoundBase {

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
			if( mousePick() == gameObject ){
				SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation>();
				if( skelAnim != null ){
					switch(currentTapIndex){
					case 0:
						skelAnim.state.SetAnimation( 0, "Tap_One", false );
//						SoundManager.PlaySFX ("DogHouse_TapOne");
						playSingleSound("DogHouse_TapOne");
						skelAnim.state.AddAnimation(0, "Start", true, 0 );
						break;
					case 1:
						skelAnim.state.SetAnimation( 0, "Tap_Two", false );
//						SoundManager.PlaySFX ("DogHouse_TapTwo");
						playSingleSound("DogHouse_TapTwo");
						skelAnim.state.AddAnimation(0, "Tap_Two_Idle", true, 0 );
						break;
					case 2:
						skelAnim.state.SetAnimation( 0, "Tap_Three", false );
//						SoundManager.PlaySFX ("DogHouse_TapThree");
						playSingleSound("DogHouse_TapThree");
						skelAnim.state.AddAnimation(0, "Start", true, 0 );
						break;
					}
				}
				currentTapIndex = (currentTapIndex+1)%3;
			}
		}
	}
}
