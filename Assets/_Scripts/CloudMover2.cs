using UnityEngine;
using System.Collections;

public class CloudMover2 : MonoBehaviour {

	public float cloudSpeed = -1;
	float leftLimit = -5;
	float rightLimitOffset = 5600;

	public Transform[] cloudsArr;
	
	public void Update(){
		//Big clouds are higher around y = 4 and faster
		//Smaller clouds are lower around y = 3 and slower
		foreach (Transform cloud in cloudsArr) {
//			cloud.Translate( cloudSpeed * Time.deltaTime * (cloud.localPosition.y-2), 0, 0 );
			float cloudOffset = cloudSpeed * Time.deltaTime;
			if( cloud.localPosition.y < 3.5 ){
				cloudOffset *= .75f;
			}
			else if( cloud.localPosition.y > 5 ){
				cloudOffset *= 1.25f;
			}
			cloud.Translate( cloudOffset*100, 0, 0 );
			if( cloud.localPosition.x < leftLimit ){
				cloud.Translate( rightLimitOffset, 0, 0 );
			}
		}
	}
}
