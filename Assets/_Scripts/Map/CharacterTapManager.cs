using UnityEngine;
using System.Collections;

public class CharacterTapManager : MonoBehaviour {

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	void Update(){
		if (Input.GetMouseButtonDown (0)) {
			if( mousePick() == gameObject ){
				string tapName = Random.value < 0.5f ? "TouchOne" : "TouchTwo";
				SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation>();
				if( skelAnim != null ){
					skelAnim.state.SetAnimation( 0, tapName, false );
					skelAnim.state.AddAnimation( 0, "Idle", true, 0 );

					string soundName = gameObject.tag + "_" + tapName;
					SoundManager.PlaySFX (soundName);
				}
			}
		}
	}
}
