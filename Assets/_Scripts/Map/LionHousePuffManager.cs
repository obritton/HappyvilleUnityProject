using UnityEngine;
using System.Collections;

public class LionHousePuffManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (randomPuffLoop ());
	}

	int currentPuff = 0;
	IEnumerator randomPuffLoop(){
		SkeletonAnimation anim = GetComponent<SkeletonAnimation> ();
		while (true) {
			yield return new WaitForSeconds(3);
			if( anim ){
				switch( currentPuff){
				case 0:
					anim.skeleton.SetSkin("Puff_One");
					break;
				case 1:
					anim.skeleton.SetSkin("Puff_Two");
					break;
				case 2:
					anim.skeleton.SetSkin("Puff_Three");
					break;
				}

				anim.state.SetAnimation(0, "Path_One", false);
				currentPuff = (currentPuff + 1)%3;
			}
		}
	}
}
