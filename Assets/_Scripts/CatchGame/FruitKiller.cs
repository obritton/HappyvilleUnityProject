using UnityEngine;
using System.Collections;
using Spine;

public class FruitKiller : CrumbColorer {

	public SkeletonAnimation lionAnimation;
	public GameObject numbersPrefab;
	public GameObject splatPrefab;
	public TimerAndMeter timerAndMeter;
	public CatchGameManager catchGame;

	public ArrayList liveFoods;

	public void addLiveFruit( GameObject fruit ){
		if( liveFoods == null )
			liveFoods = new ArrayList ();
		liveFoods.Add (fruit);
	}
	
	public void explodeAllLiveFoodAway(){
		if( liveFoods != null )
		foreach (GameObject food in liveFoods) {
			if( food != null ){
				Vector3 dir =  food.transform.position - catchGame.lion.transform.position;
				food.rigidbody.AddForce( dir * 150 );
				delayedGOKill( food, 4 );
			}
		}
	}

	public void killFruitFrom( GameObject food, string collisionTag ){

		if (collisionTag == "CatchBasket") {
			catchGame.playCatch();

			Vector3 numbersPos = lionAnimation.transform.position + new Vector3( 100, 350, 0 );
			if( catchGame.gameMode == CatchGameManager.CatchGameMode.Frenzy )
				numbersPos.y += 40;
			GameObject numbers = Instantiate( numbersPrefab, numbersPos, Quaternion.identity ) as GameObject;
			SkeletonAnimation numbersAnim = ((SkeletonAnimation)numbers.GetComponent<SkeletonAnimation>());
			string skin = food.tag == "food" ? "Five" : "Ten";
			if( food.tag == "food" )
				CatchGameManager.totalFruits++;
			else
				CatchGameManager.totalCandies++;
			numbersAnim.skeleton.SetSkin(skin);
			numbersAnim.state.SetAnimation(0,"animation",false);

			timerAndMeter.incrementScore( food.tag == "food" ? 5 : 10, catchGame.gameMode == CatchGameManager.CatchGameMode.Frenzy );
			if( liveFoods.Contains(food))
				liveFoods.Remove(food);
			Destroy (food);
		} else {
			if( food.tag == "food" ){
				Vector3 pos = food.transform.position;
				pos.z = 2;
				if( liveFoods.Contains(food))
					liveFoods.Remove(food);
				Destroy (food);
				GameObject splat = Instantiate( splatPrefab, pos, Quaternion.identity ) as GameObject;
				SkeletonAnimation splatAnim = ((SkeletonAnimation)splat.GetComponent<SkeletonAnimation>());
				splatAnim.state.SetAnimation(0, "Splat_Fall", false);
				Color color = getColorForFood( food.name );
				setColorOnObject( splatAnim.skeleton, color );
				StartCoroutine( delayedGOKill( splat, 3 ));
			}
			else{
				food.collider.enabled = false;
				food.rigidbody.velocity = Vector3.zero;
				int forceSign = food.transform.position.x <= 100 ? 1 : -1;
				food.rigidbody.AddForce( Vector3.left * 10000 * forceSign );
				food.rigidbody.AddForce( Vector3.up * 10000 );
				food.rigidbody.AddRelativeTorque( Vector3.forward * 2000000 * forceSign );
				StartCoroutine( delayedGOKill( food, 5 ));
			}
		}
	}

	IEnumerator delayedGOKill( GameObject go, float delay ){
		yield return new WaitForSeconds (delay);
		if( go.tag == "food" && liveFoods.Contains(go))
		   liveFoods.Remove(go);

		Destroy (go);
	}

	void setColorOnObject(Skeleton skel, Color color)
	{
		skel.R = color.r;
		skel.G = color.g;
		skel.B = color.b;
	}
}
