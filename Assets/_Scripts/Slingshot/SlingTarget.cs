using UnityEngine;
using System.Collections;

public class SlingTarget : MonoBehaviour {

	public SlingshotFingerFollower fingerFollower;

	void OnCollisionEnter( Collision collision){
		switch (collision.collider.tag) {
		case "SlingshotLeftWall":
//			print ("OnCollisionEnter: " + collision.collider.tag);
			resetTarget ();
			break;
		}
	}

	void OnTriggerEnter( Collider collider){
//		print ("OnTriggerEnter: " + collider.gameObject.name);
		if( collider.tag == "SlingshotLeftWall" )
			resetTarget ();
	}

	public void startVelocity(){
		GetComponent<Rigidbody> ().velocity = Vector3.left * 40;
	}

	protected float kNewTargetXOffset = 1.279f;
	void resetTarget(){
//		print ("SlingTarget resetTarget");
		transform.Translate (kNewTargetXOffset*1000, 0, 0, Space.Self);
		SkeletonAnimation skelAnim = GetComponent<SkeletonAnimation> ();
		if (gameObject.name.Split (" "[0]) [0] == "Target")
			skelAnim.state.SetAnimation (0, "Idle", true);
		startVelocity ();
		GetComponent<Collider> ().isTrigger = false;
	}
}
