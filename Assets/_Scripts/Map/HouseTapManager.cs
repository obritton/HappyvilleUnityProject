using UnityEngine;
using System.Collections;

public class HouseTapManager : SingleSoundBase {

	public string[] tapNames;
	public bool looped;

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
				currentTapIndex = (currentTapIndex+1)%tapNames.Length;
				SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation>();
				if( skelAnim != null ){
					skelAnim.state.SetAnimation( 0, tapNames[currentTapIndex], looped );
					playSound(currentTapIndex);
				}
			}
		}
	}

	void playSound(int soundIndex){
		switch (tag) {
		case "LionHouse":
		case "FoxHouse":
		case "BunnyHouse":
//			SoundManager.PlaySFX ("LionHouse_Tap");
			playSingleSound("LionHouse_Tap");
			break;
		}
	}
}
