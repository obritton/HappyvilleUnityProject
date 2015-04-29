using UnityEngine;
using System.Collections;

public class SlingFruit : MonoBehaviour {

	public SlingshotFingerFollower fingerFollower;
	public GameObject crumbsPrefab;

	void OnCollisionEnter( Collision collision){
//		print ("OnCollisionEnter: " + collision.collider.tag);
		switch (collision.collider.tag) {
		case "WaterTarget":
		{
			SkeletonAnimation skelAnim = collision.collider.GetComponent<SkeletonAnimation>();
			skelAnim.state.SetAnimation( 0, "Hit", false);
			skelAnim.state.AddAnimation(0, "Idle_hit", true, 0);
			Vector3 loc = collision.collider.transform.position;
			loc.z -= 0.1f;
			GameObject crumbs = Instantiate(crumbsPrefab, loc, Quaternion.identity ) as GameObject;
			collision.collider.isTrigger = true;
			collision.collider.GetComponent<SlingTarget>().startVelocity();
			fingerFollower.resetFruit ();
		}
			break;
		case "Fish":
		{
			collision.collider.GetComponent<SlingFish>().doHit();
			fingerFollower.resetFruit ();
		}
			break;
		case "SlingshotWalls":
			fingerFollower.resetFruit ();
			break;
		}
	}
}
