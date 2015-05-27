using UnityEngine;
using System.Collections;

public class SimpleTreeTapper : MonoBehaviour {

	GameObject mousePick(){
		RaycastHit hit;
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100))
			if( hit.collider )
				return hit.collider.gameObject;
		return null;
	}

	public string treeType = "Point";
	void Update(){
		if (Input.GetMouseButtonDown (0)) {
			if( mousePick() == gameObject ){
				int randomTapIndex = Random.Range (1,6);
				string tapName = "Tap_" + randomTapIndex;
				SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation>();
				if( skelAnim != null ){
					skelAnim.state.SetAnimation( 0, tapName, false );
					skelAnim.state.AddAnimation( 0, "Tree", true, 0 );
					string soundName = treeType+"Tree_Tap" + randomTapIndex;
					print ("soundName: " + soundName);
					SoundManager.PlaySFX (soundName);
				}
			}
		}
	}
}
