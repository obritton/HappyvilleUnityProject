using UnityEngine;
using System.Collections;

public class BearBoatManager : MonoBehaviour {

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
				SoundManager.PlaySFX("OLDFOGGY", false, 0, 200, Random.Range(0.5f, 1.5f));
			}
		}
	}
}
