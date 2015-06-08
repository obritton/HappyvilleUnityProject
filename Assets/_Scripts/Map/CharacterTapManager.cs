using UnityEngine;
using System.Collections;

public class CharacterTapManager : SingleSoundBase {

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	void Update(){

		if (Input.GetMouseButtonDown (0)) {
			if( mousePick() == gameObject && (GetComponent<SkeletonAnimation>().state.GetCurrent(0).Animation.Name != "Sleep")){
				string tapName = Random.value < 0.5f ? "TouchOne" : "TouchTwo";
//				string tapName = "Touch" + (Random.value < 0.66f ? "One" : (Random.value < 0.5f ? "Two" : "Three"));
				print ("tapName: " + tapName);
				SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation>();
				if( skelAnim != null ){
					skelAnim.state.SetAnimation( 0, tapName, false );
					skelAnim.state.AddAnimation( 0, "Idle", true, 0 );

					string soundName = gameObject.tag + "_" + tapName;

//					SoundManager.PlaySFX (soundName);
					if( MapManager.canSoundsPlay )
						playSingleSound( soundName );
				}
			}
		}
	}
}
