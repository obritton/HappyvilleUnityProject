using UnityEngine;
using System.Collections;
using Spine;

public class FruitKiller : CrumbColorer {

	public SkeletonAnimation lionAnimation;
//	public GameObject numbersPrefab;
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
				food.GetComponent<Rigidbody>().AddForce( dir * 150 );
				delayedGOKill( food, 4 );
			}
		}
	}

	public void playSplat(){
		string sfxName = "Food_Splat_0" + (1 + Random.Range (0,3));
//		print ("sfxName: " + sfxName);
		SoundManager.PlaySFX(sfxName);
	}

	public float currentPitch = 1;
	public Transform background;
	public void killFruitFrom( GameObject food, string collisionTag ){

		if (collisionTag == "CatchBasket") {
			SoundManager.PlaySFX("Lion_Catch", false, 0, 100, currentPitch);
			currentPitch += 0.1f;
			catchGame.playCatch();
			Vector3 numbersPos = lionAnimation.transform.position + new Vector3( 100, 350, 0 );
			if( catchGame.gameMode == CatchGameManager.CatchGameMode.Frenzy )
				numbersPos.y += 40;
			ScoreShower shower = GetComponent<ScoreShower>();
			if( shower != null )
				shower.showScoreAtPosition( numbersPos, food.tag == "food" ? 5: 10 );
			if( food.tag == "food" ){
				CatchGameManager.totalFruits++;
				print("CatchGameManager.totalFruits: " + CatchGameManager.totalFruits);
			}
			else
				CatchGameManager.totalCandies++;

			timerAndMeter.incrementScore( food.tag == "food" ? 5 : 10, catchGame.gameMode == CatchGameManager.CatchGameMode.Frenzy );
			if( liveFoods.Contains(food))
				liveFoods.Remove(food);
			Destroy (food);
		} else {
			if( food.tag == "food" ){
				playSplat();
				Vector3 pos = food.transform.position;
				pos.z = 2;
				if( liveFoods.Contains(food))
					liveFoods.Remove(food);
				Destroy (food);
				GameObject splat = Instantiate( splatPrefab, pos, Quaternion.identity ) as GameObject;
				splat.transform.parent = background;
				SkeletonAnimation splatAnim = ((SkeletonAnimation)splat.GetComponent<SkeletonAnimation>());
				splatAnim.state.SetAnimation(0, "Splat_Fall", false);
				Color color = getColorForFood( food.name );
				setColorOnObject( splatAnim.skeleton, color );
				StartCoroutine( delayedGOKill( splat, 3 ));
			}
			else{
				food.GetComponent<Collider>().enabled = false;
				food.GetComponent<Rigidbody>().velocity = Vector3.zero;
				int forceSign = food.transform.position.x <= 100 ? 1 : -1;
				food.GetComponent<Rigidbody>().AddForce( Vector3.left * 10000 * forceSign );
				food.GetComponent<Rigidbody>().AddForce( Vector3.up * 10000 );
				food.GetComponent<Rigidbody>().AddRelativeTorque( Vector3.forward * 2000000 * forceSign );
				StartCoroutine( delayedGOKill( food, 5 ));
				playSplat();
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
