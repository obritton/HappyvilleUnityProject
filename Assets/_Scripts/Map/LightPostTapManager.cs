using UnityEngine;
using System.Collections;

public class LightPostTapManager : SingleSoundBase {

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
				int touchIndex = Random.Range (0,4);
				string touchIndexStr = "";
				if( touchIndex > 1 )
					touchIndexStr = "" + touchIndex;
				string tapName = "TouchOne" + touchIndexStr;
				SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation>();
				if( skelAnim != null ){
					print ("tapName: " + tapName);
//					SoundManager.PlaySFX("Lightpole_Touch4");
					playSingleSound("Lightpole_Touch4");
					skelAnim.state.SetAnimation( 0, tapName, false );
					skelAnim.state.AddAnimation( 0, "Idle", true, 0 );
				}
			}
		}
	}
}
