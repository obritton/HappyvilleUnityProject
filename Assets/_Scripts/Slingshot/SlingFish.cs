using UnityEngine;
using System.Collections;

public class SlingFish : SlingTarget {

	void Start(){
		ConvFuncs.setRandomSkin (GetComponent<SkeletonAnimation> ());
	}

	bool isHit = false;

	public void doHit(){
		isHit = true;

		SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation>();
		skelAnim.state.SetAnimation( 0, "Hit", false);
		GetComponent<Collider>().isTrigger = true;
		GetComponent<SlingTarget>().startVelocity();
	}

	void resetTarget(){
		print ("SlingFish resetTarget");
		transform.Translate (kNewTargetXOffset*1000, 0, 0, Space.Self);
		startVelocity ();
		GetComponent<Collider> ().isTrigger = true;
		isHit = false;
	}
}
