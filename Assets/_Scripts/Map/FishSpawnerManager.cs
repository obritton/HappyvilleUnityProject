using UnityEngine;
using System.Collections;
using Spine;

public class FishSpawnerManager : MonoBehaviour {

	public GameObject fishPrefab;

	float xMin = -700;
	float xMax = 700;
	float yMin = -200;
	float yMax = 100;

	// Use this for initialization
	void Start () {
		StartCoroutine (loopFishCreation ());	
	}

	IEnumerator loopFishCreation(){
		while (true) {
			yield return new WaitForSeconds( 4 );

			Vector3 newLoc = new Vector3( Random.Range (xMin, xMax), Random.Range (yMin, yMax));
			GameObject fish = Instantiate(fishPrefab, newLoc, Quaternion.identity) as GameObject;
			fish.transform.parent = transform;
			fish.transform.localPosition = newLoc;
			string animStr = (newLoc.x < 0 ? "Swim_Right" : "Swim_Left");
			TrackEntry te = fish.GetComponent<SkeletonAnimation>().state.SetAnimation(0,animStr,false);
			StartCoroutine(lateKill(te.animation.duration, fish));
		}
	}

	IEnumerator lateKill(float delay, GameObject fish){
		yield return new WaitForSeconds(delay);
		Destroy (fish);
	}
}
