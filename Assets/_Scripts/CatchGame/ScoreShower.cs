using UnityEngine;
using System.Collections;
using Spine;

public class ScoreShower : MonoBehaviour {

	public GameObject numbersPrefab;

	public void showScoreAtPosition( Vector3 pos, int score = 5 ){
		GameObject numbers = Instantiate( numbersPrefab, pos, Quaternion.identity ) as GameObject;
		SkeletonAnimation numbersAnim = ((SkeletonAnimation)numbers.GetComponent<SkeletonAnimation>());
		string skin = score == 5 ? "Five" : "Ten";
		numbersAnim.skeleton.SetSkin(skin);
		TrackEntry te = numbersAnim.state.SetAnimation(0,"animation",false);
		StartCoroutine(delayedDestroy (numbersAnim.gameObject, te.animation.duration));
	}

	IEnumerator delayedDestroy( GameObject toDie, float delay ){
		yield return new WaitForSeconds( delay );
		Destroy (toDie);
	}
}
