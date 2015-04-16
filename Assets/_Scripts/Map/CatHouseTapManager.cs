using UnityEngine;
using System.Collections;

public class CatHouseTapManager : MonoBehaviour {

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
					string animName = currentTapIndex == 0 ? "Tap 1" : "Tap 2";
					string idleName = currentTapIndex == 0 ? "Tap_1_Idle" : "Start";
					skelAnim.state.SetAnimation( 0, animName, false );
					skelAnim.state.AddAnimation(0, idleName, true, 0 );
				}
				currentTapIndex = (currentTapIndex+1)%2;
			}
		}
	}
}
