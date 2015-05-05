using UnityEngine;
using System.Collections;

public class SlingFruit : MonoBehaviour {

	public SlingshotFingerFollower fingerFollower;
	public GameObject crumbsPrefab;

	public SlingshotGameManager slingshotFF;

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
			crumbs.transform.parent = collision.transform.parent;
			collision.collider.isTrigger = true;
			collision.collider.GetComponent<SlingTarget>().startVelocity();
			fingerFollower.resetFruit ();
			slingshotFF.hitTarget(collision.transform.position);
		}
			break;
		case "Fish":
		{
			collision.collider.GetComponent<SlingFish>().doHit();
			fingerFollower.resetFruit ();
			slingshotFF.hitFish(collision.transform.position);
		}
			break;
		case "SlingshotFrenzyAnimal":
		{
			fingerFollower.resetFruit ();
			slingshotFF.hitAnimal(collision.transform.position);
			collision.collider.GetComponent<SkeletonAnimation>().state.SetAnimation(0,"Eat",false);
			collision.collider.GetComponent<SkeletonAnimation>().state.AddAnimation(0,"Idle",true,0);
		}
			break;
		case "SlingshotWalls":
			fingerFollower.resetFruit ();
			break;
		}
	}
}
