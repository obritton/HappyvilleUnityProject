using UnityEngine;
using System.Collections;

public class FloatingFlagTapManager : SingleSoundBase {

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
					currentTapIndex = (currentTapIndex + 1) % 2;
					playAnimStr("Touch" + (currentTapIndex == 0 ? "One" : "Two"), true);

					if( MapManager.currentPage >= 4 && MapManager.currentPage <= 6 )
						playSingleSound("Flag_TouchOne");
				}
			}
		}
	}
}
